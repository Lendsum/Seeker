namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Interface of aggregate event context.
    /// </summary>
    public interface IAggregateEventContext
    {
        /// <summary>
        /// Attaches to event.
        /// </summary>
        /// <param name="event">The event.</param>
        void AttachToEvent(AggregateEvent @event);

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        string GetValue(string key);

        /// <summary>
        /// Loads from event.
        /// </summary>
        /// <param name="event">The event.</param>
        void LoadFromEvent(AggregateEvent @event);

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void SetValue(string key, string value);
    }
}