using Lendsum.Crosscutting.Common;
using Lendsum.Infrastructure.Core.Persistence;
using System;

namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Class to load a Ipersistable element when its needed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LazyLoader<T> : ILazyLoader<T> where T : IPersistable, new()
    {
        private IPersistenceProvider provider;
        private Lazy<T> loader;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyLoader{T}"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public LazyLoader(IPersistenceProvider provider)
        {
            this.provider = Check.NotNull(() => provider);
            this.loader = new Lazy<T>(() => this.LoadFromStorage());
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public T CurrentValue => this.loader.Value;

        /// <summary>
        /// Saves the new value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SaveNewValue(T value)
        {
            Check.NotNull(() => value);
            this.provider.UpdateOrInsert(value);
            this.loader = new Lazy<T>(() => value);
        }

        /// <summary>
        /// Loads from storage.
        /// </summary>
        /// <returns></returns>
        private T LoadFromStorage()
        {
            T item = new T();
            item = this.provider.GetValue<T>(item.DocumentKey);
            if (item == null)
            {
                item = new T();
                this.SaveNewValue(item);
            }

            return item;
        }
    }
}