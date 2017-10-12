using System;
using System.Collections.Generic;
using System.Linq;

namespace Lendsum.Crosscutting.Common.Extensions
{
    /// <summary>
    /// Class with extensions for IEnumerable collections
    /// </summary>
    public static class EnumerableExtensions
    {
        static EnumerableExtensions()
        {
            RandomGenerator = new Random();
        }

        /// <summary>
        /// Gets or sets the random generator.
        /// </summary>
        /// <value>
        /// The random generator.
        /// </value>
        public static Random RandomGenerator { get; set; }

        /// <summary>
        /// Takes an element from a random position in the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The collection.</param>
        /// <returns>A random member of the collection</returns>
        /// <exception cref="System.InvalidOperationException">Sequence was empty</exception>
        public static T RandomElement<T>(this IEnumerable<T> source)
        {
            Check.NotNull(() => source);

            T current = default(T);
            int count = 0;
            foreach (T element in source)
            {
                count++;
                if (RandomGenerator.Next(count) == 0)
                {
                    current = element;
                }
            }
            if (count == 0)
            {
                throw new InvalidOperationException("Sequence was empty");
            }
            return current;
        }

        /// <summary>
        /// Applies the action for each element in the enumeration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeration">The enumeration.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            Check.NotNull(() => enumeration);
            Check.NotNull(() => action);

            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        /// <summary>
        /// Shuffles the specified list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
        {
            Check.NotNull(() => list);

            var result = list.ToList();
            int n = list.Count();
            while (n > 1)
            {
                n--;
                int k = RandomGenerator.Next(n + 1);
                T value = result[k];
                result[k] = result[n];
                result[n] = value;
            }

            return result;
        }

        /// <summary>
        /// Nexts the and get.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        public static T NextAndGet<T>(this IEnumerator<T> list)
        {
            Check.NotNull(() => list);
            list.MoveNext();
            var result = list.Current;
            return result;
        }

        /// <summary>
        /// Fluents the add.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>A new collection with the new value included.</returns>
        public static IEnumerable<T> FluentAdd<T>(this IEnumerable<T> list, T newValue)
        {
            if (list == null) return null;
            var array = new T[list.Count() + 1];

            int i = 0;
            foreach (var value in list)
            {
                array[i] = value;
                i++;
            }

            array[i] = newValue;
            return array;
        }

        /// <summary>
        /// Fluents the add.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="newValues">The new values.</param>
        /// <returns></returns>
        public static IEnumerable<T> FluentAdd<T>(this IEnumerable<T> source, IEnumerable<T> newValues)
        {
            if (newValues == null) return source;

            foreach (var value in newValues)
            {
                source = source.FluentAdd(value);
            }

            return source;
        }

        /// <summary>
        /// Fluents the add.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>A new collection with the new value included.</returns>
        public static IEnumerable<T> FluentAddIfNotExists<T>(this IEnumerable<T> list, T newValue)
        {
            Check.NotNull(() => list);

            if (!list.Contains(newValue)) return list.FluentAdd(newValue);
            return list;
        }

        /// <summary>
        /// Merges the array with new items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <param name="newItems">The new items.</param>
        /// <returns></returns>
        public static IEnumerable<T> Merge<T>(this IEnumerable<T> array, IEnumerable<T> newItems)
        {
            Check.NotNull(() => array);

            if (newItems == null || newItems.Count() == 0) return array;
            return array.Concat(newItems).Distinct().ToArray();
        }
    }
}