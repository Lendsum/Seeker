namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// LazyLoader loads an item from persistence layer if its not already loaded.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILazyLoader<T> where T : IPersistable, new()
    {
        /// <summary>
        /// Gets the current value.
        /// </summary>
        /// <value>
        /// The current value.
        /// </value>
        T CurrentValue { get; }

        /// <summary>
        /// Saves the new value.
        /// </summary>
        /// <param name="value">The value.</param>
        void SaveNewValue(T value);
    }
}