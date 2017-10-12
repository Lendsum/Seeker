using System;

namespace Lendsum.Crosscutting.Common.Providers
{
    /// <summary>
    ///
    /// </summary>
    internal class UrlFormatProvider : IFormatProvider
    {
        private readonly UrlFormatter _formatter = new UrlFormatter();

        /// <summary>
        /// Returns an object that provides formatting services for the specified type.
        /// </summary>
        /// <param name="formatType">An object that specifies the type of format object to return.</param>
        /// <returns>
        /// An instance of the object specified by <paramref name="formatType" />, if the <see cref="T:System.IFormatProvider" /> implementation can supply that type of object; otherwise, null.
        /// </returns>
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return _formatter;
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        private class UrlFormatter : ICustomFormatter
        {
            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                if (arg == null)
                    return string.Empty;
                if (format == "r")
                    return arg.ToString();
                return Uri.EscapeDataString(arg.ToString());
            }
        }
    }
}