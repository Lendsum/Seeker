namespace Lendsum.Infrastructure.Core.Dispatcher
{
    /// <summary>
    /// Interface to be implemented by Dispatcher hub
    /// </summary>
    public interface IDispatcherHub
    {
        /// <summary>
        /// Dispatches the specified event across the listeners.
        /// </summary>
        /// <param name="incomingEvent">The event.</param>
        void Dispatch(AggregateEvent incomingEvent);
    }
}