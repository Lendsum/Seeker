using Lendsum.Infrastructure.Core.Locks;
using System;
using System.Collections.Generic;

namespace Lendsum.Infrastructure.Core.Persistence
{
    /// <summary>
    /// Interface to be implemented by a IProvider
    /// </summary>
    public interface IPersistenceProvider : IDisposable
    {
        /// <summary>
        /// Gets the current system version according to the events stored.
        /// </summary>
        /// <returns></returns>
        ulong CurrentSystemVersion { get; }

        /// <summary>
        /// Reserves a number taking it from the AllEventsCounter
        /// </summary>
        /// <returns>A number not used before</returns>
        ulong ReserveCounter();

        /// <summary>
        /// Reserves the counter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        ulong ReserveCounter(string key);

        /// <summary>
        /// Gets the value stored in the key provided.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        T GetValue<T>(string key);

        /// <summary>
        /// Gets the values stored in the keys provided.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        IEnumerable<T> GetValues<T>(IEnumerable<string> keys);

        /// <summary>
        /// Gets the values by key pattern.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startKey">The start key.</param>
        /// <param name="endKey">The end key.</param>
        /// <param name="includeKeys">if set to <c>true</c> the query will return the starKey and endKey inside the range to search.</param>
        /// <returns></returns>
        IEnumerable<T> GetValuesByKeyPattern<T>(string startKey, string endKey = "", bool includeKeys = false);

        /// <summary>
        /// Gets the values by key pattern.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit">The limit.</param>
        /// <param name="startKey">The start key.</param>
        /// <param name="endKey">The end key.</param>
        /// <param name="includeKeys">if set to <c>true</c> the query will return the starKey and endKey inside the range to search.</param>
        /// <returns></returns>
        BatchQuery<T> GetValuesByKeyPattern<T>(int limit, string startKey, string endKey = "", bool includeKeys = false) where T : IPersistable;

        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        PersistenceResultEnum Insert(IPersistable item);

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        PersistenceResultEnum Delete(IPersistable item);

        /// <summary>
        /// Update or insert the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        PersistenceResultEnum UpdateOrInsert(IPersistable item);

        /// <summary>
        /// Gets the lock.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="maxExecutingInSeconds">The maximum executing in seconds.</param>
        /// <returns></returns>
        LockerInfo GetLock(string itemName, int maxExecutingInSeconds);

        /// <summary>
        /// Releases the lock.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <returns></returns>
        void ReleaseLock(string itemName);

        /// <summary>
        /// Begins the transaction scope, use the returned object to perfom operations over the database.
        /// </summary>
        /// <returns></returns>
        IPersistenceProvider BeginScope();

        /// <summary>
        /// Commits this instance.
        /// </summary>
        void Commit();

        /// <summary>
        /// Rollbacks this instance.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Deletes all keys and values from the persistence layer.
        /// </summary>
        void DeleteAll();
    }
}