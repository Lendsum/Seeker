using Lendsum.Crosscutting.Common;
using Lendsum.Crosscutting.Common.Extensions;
using Lendsum.Infrastructure.Core.Persistence;

namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Class to load a Ipersistable element when its needed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LazyInContextLoader<T> : ILazyInContextLoader<T> where T : IPersistable, new()
    {
        private IPersistenceProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyLoader{T}" /> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public LazyInContextLoader(IPersistenceProvider provider)
        {
            this.provider = Check.NotNull(() => provider);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public T CurrentValue => this.LoadFromContextOrStorage();

        /// <summary>
        /// Saves the new value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SaveNewValue(T value)
        {
            Check.NotNull(() => value);
            this.provider.UpdateOrInsert(value);
        }

        /// <summary>
        /// Loads from storage.
        /// </summary>
        /// <returns></returns>
        private T LoadFromContextOrStorage()
        {
            T item = new T();

            if (ThreadContextContainer.CurrentContext.ContainsKey(item.DocumentKey))
            {
                return (T)ThreadContextContainer.CurrentContext[item.DocumentKey];
            }

            item = this.provider.GetValue<T>(item.DocumentKey);
            if (item == null)
            {
                item = new T();
                this.SaveNewValue(item);
            }

            lock (this.provider)
            {
                ThreadContextContainer.CurrentContext.AddOrReplace(item.DocumentKey, item);
            }

            return item;
        }
    }
}