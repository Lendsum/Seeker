namespace Lendsum.Crosscutting.Common
{
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Parser of boolean values.
    /// This class must be replaced by the mechanism implemented in the PAF-244 story.
    /// </summary>
    public static class BooleanFieldParser
    {
        private static string[] validTrueValues = { "T", "Y", "1", "TRUE" };
        private static string[] validFalseValues = { "F", "N", "0", "FALSE" };

        /// <summary>
        /// Tries the parsing of the given boolean value.
        /// </summary>
        /// <param name="value">The original value.</param>
        /// <param name="result">The parsed value.</param>
        /// <returns>if set to <c>true</c> the parsin was successful</returns>
        public static bool TryParseBooleanValue(string value, out bool result)
        {
            bool success = false;
            result = false;

            if (string.IsNullOrWhiteSpace(value))
            {
                return success;
            }

            if (validTrueValues.Contains(value.ToUpper(CultureInfo.InvariantCulture)))
            {
                result = true;
                success = true;
            }
            else if (validFalseValues.Contains(value.ToUpper(CultureInfo.InvariantCulture)))
            {
                result = false;
                success = true;
            }

            return success;
        }
    }
}