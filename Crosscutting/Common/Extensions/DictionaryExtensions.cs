﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Lendsum.Crosscutting.Common.Extensions
{
    /// <summary>
    /// Class with extensions for dictionary
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Add the new value and return it if the key doesnt exist, or return the current value associate with the key
        /// </summary>
        /// <param name="target"></param>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> target, TKey key, Func<TValue> newValue)
        {
            Check.NotNull(() => target);
            Check.NotNull(() => newValue);

            TValue result;
            if (target.Keys.Contains(key))
            {
                result = target[key];
            }
            else
            {
                result = newValue.Invoke();
                target.Add(key, result);
            }

            return result;
        }

        /// <summary>
        /// Add the new value as generated by the factory and return it if the key doesnt exist, or return the current value associate with the key
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="key">The key.</param>
        /// <param name="factory">The factory.</param>
        /// <returns></returns>
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> target, TKey key, Func<TKey, TValue> factory)
        {
            Check.NotNull(() => target);
            Check.NotNull(() => factory);

            TValue result;
            if (target.Keys.Contains(key))
            {
                result = target[key];
            }
            else
            {
                result = factory(key);
                target.Add(key, result);
            }

            return result;
        }

        /// <summary>
        /// Add the new value if the key doesnt exist, or replace the current value associate with the key with the new value.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        public static void AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> target, TKey key, TValue newValue)
        {
            Check.NotNull(() => target);

            if (target.Keys.Contains(key))
            {
                target[key] = newValue;
            }
            else
            {
                target.Add(key, newValue);
            }
        }

        /// <summary>
        /// Formats the dictionary to a string like "{key1=value1,key2=value2,...}"
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns></returns>
        public static string ToDebugString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return "{" + string.Join(",", dictionary.Select(kv => kv.Key.ToString() + "=" + ((kv.Value != null) ? kv.Value.ToString() : "")).ToArray()) + "}";
        }

        /// <summary>
        /// Merges the other dictionary into this one. Repeated keys' values will be overwriten.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> target, Dictionary<TKey, TValue> other)
        {
            other.ToList().ForEach(x => target[x.Key] = x.Value);
            return target;
        }
    }
}