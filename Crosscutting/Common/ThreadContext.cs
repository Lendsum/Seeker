using Lendsum.Crosscutting.Common.Extensions;
using System;
using System.Collections.Generic;

namespace Lendsum.Crosscutting.Common
{
    /// <summary>
    /// Thread context to save variables locally to thread.
    /// </summary>
    public class ThreadContext : IThreadContext
    {
        private const string DefaultKey = "default";

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadContext"/> class.
        /// </summary>
        public ThreadContext()
        {
        }

        /// <summary>
        /// Gets the value stored inside the key
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T GetValue<T>(string key = DefaultKey)
        {
            if (!Storage.ContainsKey(ComposeKey(typeof(T), key)))
            {
                return default(T);
            }

            return (T)Storage[ComposeKey(typeof(T), key)];
        }

        /// <summary>
        /// Updates the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="key">The key.</param>
        public void Update<T>(T value, string key = DefaultKey)
        {
            Storage.AddOrReplace(ComposeKey(typeof(T), key), value);
        }

        /// <summary>
        /// Gets the storage.
        /// </summary>
        /// <value>
        /// The storage.
        /// </value>
        private static Dictionary<string, object> Storage
        {
            get
            {
                return ThreadContextContainer.CurrentContext;
            }
        }

        private static string ComposeKey(Type type, string key)
        {
            return type.Name + ":" + key;
        }
    }
}