using System;

namespace Lendsum.Infrastructure.Core.Queues
{
    /// <summary>
    /// Interface to be implemented by a queue factory.
    /// </summary>
    public interface IQueueFactory : IDisposable
    {
        /// <summary>
        /// Gets the queue.
        /// </summary>
        /// <param name="name">The name of the queue</param>
        /// <returns>The queue ready to be used.</returns>
        IQueue GetQueue(string name);
    }
}