namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Service to send messages to queues following the unit of work pattern
    /// </summary>
    public interface IUnitOfWorkQueueSender
    {
        /// <summary>
        /// Add a message to buffer queues. The message won't be send until sendbufferedtoqueue is called.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="rawMessage">The raw message.</param>
        void Enqueue(string queueName, string rawMessage);

        /// <summary>
        /// Add a message to buffer queues. The message won't be send until sendbufferedtoqueue is called.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        void EnqueueObject<T>(string queueName, T message);

        /// <summary>
        /// Sends the buffered to persistable queues, all togethers.
        /// </summary>
        void Commit();
    }
}