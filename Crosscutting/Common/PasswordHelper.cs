using Lendsum.Crosscutting.Common.Extensions;
using System.Text.RegularExpressions;

namespace Lendsum.Crosscutting.Common
{
    /// <summary>
    /// Helper to work with passwords.
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>
        /// Gives a number with the streng of the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static int CalculatePasswordStrength(string password)
        {
            if (password == null) password = string.Empty;
            int score = 1;

            if (password.Length >= 9)
                score++;
            if (Regex.Match(password, @"[\d]", RegexOptions.ECMAScript).Success)
                score++;
            if (Regex.Match(password, @"[a-z]", RegexOptions.ECMAScript).Success &&
              Regex.Match(password, @"[A-Z]", RegexOptions.ECMAScript).Success)
                score++;
            if (Regex.Match(password, @".[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]", RegexOptions.ECMAScript).Success)
                score++;

            return score;
        }

        /// <summary>
        /// Determines whether if a password is valid or not.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) { return false; }
            if (password.Length < 6) return false;

            if (password.RemoveNumbers() == password) return false;
            if (password.RemoveNumbers().Length == 0) return false;

            return true;
        }
    }
}