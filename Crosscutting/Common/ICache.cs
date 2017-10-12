using System;

namespace Lendsum.Crosscutting.Common
{
    /// <summary>
    /// Interface to be implemented by a cache system.
    /// </summary>
    /// <typeparam name="T">Type of the data stored by this cache</typeparam>
    public interface ICache<T> where T : class
    {
        /// <summary>
        /// Gets the specified value stored using the key in the cache
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Value read from the cache</returns>
        T GetValue(string key);

        /// <summary>
        /// Saves an item in the cache with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="absoluteExpirationInSeconds">The absolute expiration of the cache value in seconds.</param>
        /// <param name="value">The value.</param>
        void Save(string key, double absoluteExpirationInSeconds, T value);

        /// <summary>
        /// Gets the or add value asynchronous.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="absoluteExpirationTime">The absolute expiration time in seconds.</param>
        /// <param name="valueFactory">The value factory.</param>
        /// <returns></returns>
        T GetOrAddValue(string key, double absoluteExpirationTime, Func<T> valueFactory);
    }
}