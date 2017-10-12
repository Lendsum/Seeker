using System;
using System.Collections.Generic;
using System.Linq;

namespace Lendsum.Crosscutting.Common.Extensions
{
    /// <summary>
    /// Extension of list.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Adds if the expression returns true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="expression">The expression.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public static void AddIf<T>(this List<T> list, T newValue, Func<List<T>, bool> expression)
        {
            Check.NotNull(() => list);
            Check.NotNull(() => expression);
            if (expression(list))
            {
                list.Add(newValue);
            }
        }

        /// <summary>
        /// Adds if not exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="value">The value.</param>
        /// <param name="equalExpression">The equal expression.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public static void AddIfNotExist<T>(this List<T> list, T value, Func<T, T, bool> equalExpression)
        {
            list.AddIf(value, (items) => !items.Any(x => equalExpression(x, value)));
        }

        /// <summary>
        /// Replaces if exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="value">The value.</param>
        /// <param name="equalExpression">The equal expression.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public static void ReplaceIfExist<T>(this List<T> list, T value, Func<T, T, bool> equalExpression)
        {
            Check.NotNull(() => list);
            list.RemoveIfExist(value, equalExpression);
            list.Add(value);
        }

        /// <summary>
        /// remove if if exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="value">The value.</param>
        /// <param name="equalExpression">The equal expression.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public static void RemoveIfExist<T>(this List<T> list, T value, Func<T, T, bool> equalExpression)
        {
            Check.NotNull(() => list);
            var item = list.Where(x => equalExpression(x, value)).FirstOrDefault();
            if (item != null)
            {
                list.Remove(item);
            }
        }
    }
}