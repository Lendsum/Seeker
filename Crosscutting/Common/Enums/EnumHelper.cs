using System;
using System.Collections.Generic;
using System.Linq;

namespace Lendsum.Crosscutting.Common.Enums
{
    /// <summary>
    ///
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Gets the values. of the enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}