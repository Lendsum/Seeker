namespace Lendsum.Infrastructure.Core.Dispatcher
{
    /// <summary>
    /// Interface to be implemented by classes that must listen to some events
    /// in an async way. It means read the events from the queue.
    /// </summary>
    public interface IAsyncListener
    {
        /// <summary>
        /// Dispatches the specified event.
        /// </summary>
        /// <param name="incomingEvent">The event.</param>
        void Consume(AggregateEvent incomingEvent);

        /// <summary>
        /// Gets a value indicating whether this <see cref="IListener"/> is rebuildable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if rebuildable; otherwise, <c>false</c>.
        /// </value>
        bool Rebuildable { get; }

        /// <summary>
        /// Gets a value indicating whether [consume only last event].
        /// </summary>
        /// <value>
        /// <c>true</c> if [consume only last event]; otherwise, <c>false</c>.
        /// </value>
        bool ConsumeOnlyLastEvent { get; }
    }
}