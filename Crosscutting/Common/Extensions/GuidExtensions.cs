using System;

namespace Lendsum.Crosscutting.Common.Extensions
{
    /// <summary>
    /// Methots for Guid extensions
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        /// To the base64.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <returns></returns>
        public static string ToBase64(this Guid uid)
        {
            if (uid == null) return null;

            return Convert.ToBase64String(uid.ToByteArray());
        }

        /// <summary>
        /// To the unique identifier.
        /// </summary>
        /// <param name="base64Str">The base64 string.</param>
        /// <returns></returns>
        public static Guid ToGuid(this string base64Str)
        {
            if (base64Str == null) return new Guid();

            return new Guid(Convert.FromBase64String(base64Str));
        }
    }
}