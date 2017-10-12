namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Interrface to be implemented by aggregates that wants to receive events of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IApplyEvent<T> where T : AggregateEvent
    {
        /// <summary>
        /// Applies the specified event over the aggregate
        /// </summary>
        /// <param name="incomingEvent">The event.</param>
        void Apply(T incomingEvent);
    }
}