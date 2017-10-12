using Lendsum.Crosscutting.Common;
using Lendsum.Crosscutting.Common.Extensions;
using Lendsum.Crosscutting.Common.Serialization;
using Lendsum.Infrastructure.Core.Queues;
using System.Collections.Generic;
using System.Linq;

namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Class that represents a unit of work queue sender.
    /// </summary>
    public class UnitOfWorkQueueSender : IUnitOfWorkQueueSender
    {
        private IThreadContext context;
        private IQueueFactory factory;

        private Dictionary<string, IQueue> queues = new Dictionary<string, IQueue>();
        private ITextSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkQueueSender" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="serializer">The serializer.</param>
        public UnitOfWorkQueueSender(IThreadContext context, IQueueFactory factory, ITextSerializer serializer)
        {
            this.context = Check.NotNull(() => context);
            this.factory = Check.NotNull(() => factory);
            this.serializer = Check.NotNull(() => serializer);
        }

        /// <summary>
        /// Add a message to buffer queues. The message won't be send until sendbufferedtoqueue is called.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="rawMessage">The raw message.</param>
        public void Enqueue(string queueName, string rawMessage)
        {
            Check.NotNullOrEmpty(() => queueName);
            Check.NotNullOrEmpty(() => rawMessage);
            var queue = this.GetBufferOfQueue(queueName);
            queue.Enqueue(rawMessage);
        }

        /// <summary>
        /// Enqueues the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        public void EnqueueObject<T>(string queueName, T message)
        {
            Check.NotNull(() => message);
            var raw = this.serializer.Serialize<T>(message);
            this.Enqueue(queueName, raw);
        }

        /// <summary>
        /// Sends the buffered to persistable queues, all togethers.
        /// </summary>
        public void Commit()
        {
            foreach (var queueName in messagesBuffer.Keys)
            {
                var buffer = messagesBuffer[queueName];
                while (buffer.Any())
                {
                    var raw = buffer.Dequeue();
                    this.GetQueue(queueName).Push(raw);
                }
            }
        }

        private Dictionary<string, Queue<string>> messagesBuffer
        {
            get
            {
                string key = "UnitOfWorkQueueSenderStorage";
                var storage = context.GetValue<Dictionary<string, Queue<string>>>(key);
                if (storage == null)
                {
                    storage = new Dictionary<string, Queue<string>>();
                    context.Update(storage, key);
                }

                return storage;
            }
        }

        private Queue<string> GetBufferOfQueue(string name)
        {
            var result = this.messagesBuffer.GetOrAdd(name, () => new Queue<string>());
            return result;
        }

        private IQueue GetQueue(string name)
        {
            lock (queues)
            {
                var queue = queues.GetOrAdd(name, () => this.factory.GetQueue(name));

                return queue;
            }
        }
    }
}