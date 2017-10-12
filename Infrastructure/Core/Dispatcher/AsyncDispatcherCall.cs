using Lendsum.Crosscutting.Common;
using Lendsum.Infrastructure.Core.DelayCalls;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lendsum.Infrastructure.Core.Dispatcher
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="Lendsum.Infrastructure.Core.DelayCalls.IDelayed" />
    public class AsyncDispatcherCall : IDelayed
    {
        private IEnumerable<IAsyncListener> asyncListeners;
        private IAggregateEventContext eventContext;
        private ILogger log;
        private IAggregateSnapShoter snapShoter;
        private IUnitOfWork work;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDispatcherCall" /> class.
        /// </summary>
        /// <param name="asyncListeners">The asynchronous listeners.</param>
        /// <param name="work">The work.</param>
        /// <param name="eventContext">The event context.</param>
        /// <param name="snapShoter">The snap shoter.</param>
        /// <param name="log">The log.</param>
        public AsyncDispatcherCall(
            IEnumerable<IAsyncListener> asyncListeners,
            IUnitOfWork work,
            IAggregateEventContext eventContext,
            IAggregateSnapShoter snapShoter,
            ILogger log)
        {
            this.asyncListeners = Check.NotNull(() => asyncListeners)
                        .GroupBy(x => x.GetType())
                        .Select(group => group.First());

            this.work = Check.NotNull(() => work); ;
            this.eventContext = Check.NotNull(() => eventContext); ;
            this.snapShoter = Check.NotNull(() => snapShoter); ;
            this.log = Check.NotNull(() => log); ;
        }

        /// <summary>
        /// Gets the type suported of this call.
        /// </summary>
        /// <value>
        /// The type suported.
        /// </value>
        public IEnumerable<Type> TypesSupported => new Type[] { typeof(EventDispatcherData) };

        /// <summary>
        /// Completes the delayed call.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="NotImplementedException"></exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't know the type of the exception raised")]
        public void CompleteCall(object data)
        {
            var typedData = data as EventDispatcherData;
            if (typedData == null)
            {
                throw new LendsumException("The data cannot be parsed to EventDispatcherData");
            }
            var uid = typedData.AggregateUid;
            var aggregate = this.work.GetAggregate(uid);
            if (aggregate == null)
            {
                throw new LendsumException(S.Invariant($"The aggregateUid {uid} doesn't exist"));
            }

            var events = aggregate.Events.Where(x => x.AsyncProcessed == false);
            foreach (var @event in events)
            {
                this.ProcessEvent(@event, @event == events.LastOrDefault());
            }

            try
            {
                this.snapShoter.SnapShotIfNeeded(aggregate.Uid);
            }
            catch (Exception ex)
            {
                this.log.LogError(
                S.Invariant($"There is an error snapshooting aggregate {uid}")
                , ex);
            }
        }

        private void ProcessEvent(AggregateEvent @event, bool theLastOne)
        {
            IEnumerable<IAsyncListener> listeners;
            if (theLastOne)
            {
                listeners = this.asyncListeners;
            }
            else
            {
                listeners = this.asyncListeners.Where(x => !x.ConsumeOnlyLastEvent).ToArray();
            }

            this.ConsumeEventByListeners(@event, listeners);
        }

        private void ConsumeEventByListeners(AggregateEvent @event, IEnumerable<IAsyncListener> listeners)
        {
            using (var context = new ThreadContextContainer())
            {
                this.eventContext.LoadFromEvent(@event);

                foreach (var listener in listeners)
                {
                    try
                    {
                        listener.Consume(@event);
                    }
                    catch
                    {
                        var message = S.Invariant(
                                $@"Error processing the listener {listener.GetType().ToString()}
                                    for aggregate {@event.AggregateUid},
                                    version {@event.AggregateVersion},
                                    event type {@event.GetType().ToString()}");

                        this.log.LogError(message);
                        throw;
                    }
                }

                this.work.MarkEventAsAsyncProcessed(@event);
                work.Commit();
            }
        }

        /// <summary>
        /// Gets the name of the lock or empty/null if no lock is needed
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public string GetLockName(object data)
        {
            var lockName = (data as EventDispatcherData)?.AggregateUid.ToString();
            return lockName;
        }
    }
}