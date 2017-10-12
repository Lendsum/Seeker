using System.Globalization;
using System.Linq;

namespace Lendsum.Crosscutting.Common.Extensions
{
    /// <summary>
    /// Int extensions
    /// </summary>
    public static class IntExtensions
    {
        /// <summary>
        /// Reverses the specified positive number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public static string Reverse(this int number)
        {
            return number.ToString(CultureInfo.InvariantCulture).Reverse().Aggregate("", (s, c) => s + c);
        }

        /// <summary>
        /// To the invariant string.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public static string ToInvariantString(this int number)
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }
    }
}