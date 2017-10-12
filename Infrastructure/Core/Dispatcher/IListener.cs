namespace Lendsum.Infrastructure.Core.Dispatcher
{
    /// <summary>
    /// Interface to be implemented by classes that must listen to some events
    /// </summary>
    public interface IListener
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
    }
}