using System;
using System.Security.Cryptography;
using System.Text;

namespace Lendsum.Crosscutting.Common.Hash
{
    /// <summary>
    /// Hash service
    /// </summary>
    public class HashService : IHashService
    {
        /// <summary>
        /// Hashes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public string Hash(string value)
        {
            using (SHA512CryptoServiceProvider sha = new SHA512CryptoServiceProvider())
            {
                byte[] dataToHash = Encoding.UTF8.GetBytes(value);
                byte[] hashed = sha.ComputeHash(dataToHash);
                return Convert.ToBase64String(hashed);
            }
        }
    }
}