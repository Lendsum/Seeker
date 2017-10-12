namespace Lendsum.Crosscutting.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class to hold extension methods for enumerations
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Maps the type name to enumeration.
        /// </summary>
        /// <typeparam name="T">type of the value to be converted</typeparam>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>enumeration value from type name of the passed value</returns>
        public static TEnum TypeNameToEnum<T, TEnum>(this T value)
            where T : class
            where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be an enumerated type");
            }

            Check.NotNull(() => value);
            return (TEnum)Enum.Parse(typeof(TEnum), value.GetType().Name);
        }

        /// <summary>
        /// Extracts the enumeration members into a dictionary.
        /// </summary>
        /// <param name="enumType">Type of the enumeration.</param>
        /// <returns>
        /// dictionary containing the values of the enumeration
        /// </returns>
        public static IDictionary<int, string> ExtractEnumMembers(this Type enumType)
        {
            Check.NotNull(() => enumType);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("TEnum must be an enumerated type");
            }

            IEnumerable<int> values = Enum.GetValues(enumType).Cast<int>();
            var dictionary = values.ToDictionary(v => v, v => Enum.GetName(enumType, v));
            return dictionary;
        }
    }
}