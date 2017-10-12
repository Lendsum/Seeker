//namespace Lendsum.Crosscutting.Common.Providers
//{
//    using System;
//    using System.Runtime.Caching;

//    /// <summary>
//    /// Wrapper to runtime cache
//    /// </summary>
//    /// <typeparam name="T">Type of the items to be stored in the cache</typeparam>
//    public class Cache<T> : ICache<T> where T : class
//    {
//        /// <summary>
//        /// Default container of cache.
//        /// </summary>
//        private readonly ObjectCache cache;

//        /// <summary>
//        /// The prefix to be added to the keys in the cache.
//        /// </summary>
//        private readonly string prefix;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="Cache{T}"/> class.
//        /// </summary>
//        public Cache()
//        {
//            this.cache = MemoryCache.Default;
//            this.prefix = typeof(T).ToString();
//        }

//        /// <inheritdoc />
//        public void Save(string key, double absoluteExpirationInSeconds, T value)
//        {
//            Check.NotNullOrEmpty(() => key);

//            if (absoluteExpirationInSeconds <= 0 || value == null)
//            {
//                // we don't save the data
//                return;
//            }

//            CacheItemPolicy policy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(absoluteExpirationInSeconds) };
//            this.cache.Set(this.GetFullKey(key), value, policy);
//        }

//        /// <inheritdoc />
//        public T GetValue(string key)
//        {
//            Check.NotNullOrEmpty(() => key);

//            return this.cache[this.GetFullKey(key)] as T;
//        }

//        /// <summary>
//        /// Gets the full key adding as a prefix the type of the object managed by this cache
//        /// </summary>
//        /// <param name="key">The key.</param>
//        /// <returns>Key resulting from adding the <see cref="prefix"/> to the <paramref name="key"/></returns>
//        private string GetFullKey(string key)
//        {
//            return this.prefix + "_" + key;
//        }

//        /// <summary>
//        /// Gets the or add value.
//        /// </summary>
//        /// <param name="key">The key.</param>
//        /// <param name="absoluteExpirationTime">The absolute expiration time.</param>
//        /// <param name="valueFactory">The value factory.</param>
//        /// <returns></returns>
//        public T GetOrAddValue(string key, double absoluteExpirationTime, Func<T> valueFactory)
//        {
//            if (valueFactory == null)
//            {
//                throw new ArgumentNullException("valueFactory");
//            }

//            T value = this.GetValue(key);
//            if (value != null)
//            {
//                return value;
//            }
//            else
//            {
//                value = valueFactory();
//                this.Save(key, absoluteExpirationTime, value);
//                return value;
//            }
//        }
//    }
//}