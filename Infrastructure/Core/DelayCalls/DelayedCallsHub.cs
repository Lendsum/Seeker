using Lendsum.Crosscutting.Common;
using Lendsum.Crosscutting.Common.Serialization;
using Lendsum.Infrastructure.Core.Exceptions;
using Lendsum.Infrastructure.Core.Locks;
using Lendsum.Infrastructure.Core.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Lendsum.Infrastructure.Core.DelayCalls
{
    /// <summary>
    /// Class to execute the delayed calls
    /// </summary>
    public class DelayedCallsHub : IDelayedCallsHub
    {
        /// <summary>
        /// The queue prefix name
        /// </summary>
        private const string QueuePrefixName = "Delayer_";

        private IEnumerable<IDelayed> methods;
        private DelayedCallsQueueConfigurations config;
        private Dictionary<Type, IDelayed> methodsByType = new Dictionary<Type, IDelayed>();
        private IQueueFactory queueFactory;
        private ILogger log;
        private ITextSerializer serializer;
        private ILocker locker;
        private IUnitOfWork work;

        private static NumberOfItemFixedList<string> lockerBarrier = new NumberOfItemFixedList<string>(100);

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayCalls.DelayedCallsHub"/> class.
        /// </summary>
        public DelayedCallsHub(
            IEnumerable<IDelayed> delayeds,
            IQueueFactory queueFactory,
            ITextSerializer serializer,
            ILocker locker,
            IUnitOfWork work,
            IOptions<DelayedCallsQueueConfigurations> options,
            ILogger log)
        {
            this.queueFactory = Check.NotNull(() => queueFactory);
            this.log = Check.NotNull(() => log);
            this.serializer = Check.NotNull(() => serializer);
            this.locker = Check.NotNull(() => locker);
            this.work = Check.NotNull(() => work);

            this.methods = Check.NotNull(() => delayeds).GroupBy(x => x.GetType())
                       .Select(group => group.First());
            this.config = Check.NotNull(() => options).Value;

            FillMethodsByType();
        }

        /// <summary>
        /// Gets the type of the queue name by.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string GetQueueNameByType(Type type)
        {
            Check.NotNull(() => type);
            return QueuePrefixName + type.FullName;
        }

        /// <summary>
        /// Gets the type of the queue name for errors by.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string GetQueueNameForErrorsByType(Type type)
        {
            Check.NotNull(() => type);
            return QueuePrefixName + type.FullName + "_Error";
        }

        /// <summary>
        /// Starts the processing.
        /// </summary>
        public void StartProcessing()
        {
            var queueConfig = this.config;
            foreach (var supportedType in methodsByType.Keys)
            {
                // open the queue and error queue.
                var queueName = GetQueueNameByType(supportedType);
                var errorQueueName = GetQueueNameForErrorsByType(supportedType);
                var errorQueue = this.queueFactory.GetQueue(errorQueueName);
                var threads = queueConfig.GetThreadsPerName(queueName);

                for (int i = 0; i < threads; i++)
                {
                    var queue = this.queueFactory.GetQueue(queueName);
                    queue.ReadNoAck((x, y) => this.Process(x, y, queue, errorQueue), this.log);
                }
            }
        }

        /// <summary>
        /// Processes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ack">The <see cref="BasicDeliverEventArgs" /> instance containing the event data.</param>
        /// <param name="queue">The queue.</param>
        /// <param name="errorQueue">The error queue.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Its a public method that cannot raise exception from here")]
        public void Process(string message, BasicDeliverEventArgs ack, IQueue queue, IQueue errorQueue)
        {
            Check.NotNull(() => queue);
            Check.NotNull(() => errorQueue);

            object data = this.serializer.Deserialize<object>(message);
            if (data == null) return;
            var dataType = data.GetType();
            if (!this.methodsByType.ContainsKey(dataType))
            {
                this.log.LogError(S.Invariant($"The datatType {dataType.ToString()} dosen't have any handler"));
                return;
            }

            bool sendToError = false;
            LockerInfo currentLock = null;
            try
            {
                using (var context = new ThreadContextContainer())
                {
                    var method = this.methodsByType[dataType];
                    var lockName = method.GetLockName(data);

                    if (!string.IsNullOrWhiteSpace(lockName))
                    {
                        currentLock = this.locker.TryLockKey(lockName, 300);
                        if (currentLock == null)
                        {
                            queue.SendNoAck(ack);
                            this.log.LogDebug(S.Invariant($"{lockName} is locked so the delayed calls hub cannot procces it right now"));
                            CheckLockerBarrier(lockName);
                            return;
                        }
                    }

                    method.CompleteCall(data);
                    this.work.Commit();
                }
            }
            catch (ConcurrencyException)
            {
                this.log.LogWarning(S.Invariant($"There is a concurrency excpetion in the DelayedCall with message {message}"));
                queue.SendNoAck(ack);
                return;
            }
            catch (Exception ex)
            {
                this.log.LogError(
                    S.Invariant($"There is an error executing the DelayedCall with message {message}, the message will be sent to the error queue")
                    , ex);

                sendToError = true;
            }

            if (currentLock != null)
            {
                this.locker.ReleaseKey(currentLock.ItemName);
            }

            if (sendToError)
            {
                SendToError(data, errorQueue);
            }

            queue.SendAck(ack);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "we dont know what expetion will be raised")]
        private void SendToError(object data, IQueue errorQueue)
        {
            var raw = this.serializer.Serialize(data);
            try
            {
                errorQueue.Push(raw);
            }
            catch (Exception ex)
            {
                this.log.LogError(S.Invariant($"Error sending data {raw} to the error queue"), ex);
            }
        }

        private void FillMethodsByType()
        {
            foreach (var method in methods)
            {
                foreach (var typeSupported in method.TypesSupported)
                {
                    if (methodsByType.ContainsKey(typeSupported))
                        throw new LendsumException(S.Invariant($"The type {typeSupported.ToString()} has more than one method to be executed delayed. That is not allowed"));

                    methodsByType.Add(typeSupported, method);
                }
            }
        }

        private static void CheckLockerBarrier(string lockerName)
        {
            bool sleep = false;
            lock (lockerBarrier)
            {
                lockerBarrier.Add(lockerName);
                var lastTryLockers = lockerBarrier.AllItems.Where(x => x.Item == lockerName && x.When > DateTime.UtcNow.AddSeconds(-2));
                if (lastTryLockers != null && lastTryLockers.Count() > 5)
                {
                    sleep = true;
                }
            }

            if (sleep)
            {
                Thread.Sleep(2000);
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.queueFactory.Dispose();
                    this.queueFactory = null;
                }

                this.disposedValue = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}