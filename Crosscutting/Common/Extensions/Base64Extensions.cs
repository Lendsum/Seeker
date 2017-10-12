using System;

namespace Lendsum.Crosscutting.Common.Extensions
{
    /// <summary>
    ///
    /// </summary>
    public static class Base64
    {
        /// <summary>
        /// To the base64.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string ToBase64(this System.Text.Encoding encoding, string text)
        {
            Check.NotNull(() => encoding);
            Check.NotNull(() => text);

            if (text == null)
            {
                return null;
            }

            byte[] textAsBytes = encoding.GetBytes(text);
            return Convert.ToBase64String(textAsBytes);
        }

        /// <summary>
        /// Tries the parse base64.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="encodedText">The encoded text.</param>
        /// <returns></returns>
        public static string ParseFromBase64(this System.Text.Encoding encoding, string encodedText)
        {
            Check.NotNull(() => encoding);

            if (encodedText == null)
            {
                return null;
            }

            byte[] textAsBytes = System.Convert.FromBase64String(encodedText);
            return encoding.GetString(textAsBytes);
        }
    }
}