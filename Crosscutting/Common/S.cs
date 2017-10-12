using Lendsum.Crosscutting.Common.Providers;
using System;
using System.Globalization;

namespace Lendsum.Crosscutting.Common
{
    /// <summary>
    /// Class to work with $ and format very quickly
    /// </summary>
    public static class S
    {
        /// <summary>
        /// Easy way to use $
        /// string text = Invariant($"{p.Name} was born on {p.DateOfBirth:D}");
        /// </summary>
        /// <param name="formattable">The formattable.</param>
        /// <returns></returns>
        public static string Invariant(FormattableString formattable)
        {
            if (formattable == null) return null;
            return formattable.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// URLs the specified formattable.
        /// </summary>
        /// <param name="formattable">The formattable.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Uri is hard to serialize")]
        public static string Url(FormattableString formattable)
        {
            if (formattable == null) return null;
            return formattable.ToString(new UrlFormatProvider());
        }
    }
}