using Lendsum.Crosscutting.Common;

namespace Lendsum.Infrastructure.Core.DelayCalls
{
    /// <summary>
    /// Delayer implementation.
    /// </summary>
    /// <typeparam name="T">Type of the parameter passed to the delayed method.</typeparam>
    public class Delayer<T> : IDelayer<T> where T : class
    {
        private IUnitOfWorkQueueSender sender;

        /// <summary>
        /// Values the tuple.
        /// </summary>
        /// <returns></returns>
        public Delayer(IUnitOfWorkQueueSender sender)
        {
            this.sender = Check.NotNull(() => sender);
        }

        /// <summary>
        /// Calls the method who can handle the data passed by parameters
        /// </summary>
        /// <param name="data">The data that will receive the method called delayed.</param>
        public void Call(T data)
        {
            if (data == null) return;
            string queueName = DelayedCallsHub.GetQueueNameByType(typeof(T));
            this.sender.EnqueueObject(queueName, data);
        }
    }
}