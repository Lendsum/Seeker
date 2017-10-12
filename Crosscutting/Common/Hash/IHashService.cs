namespace Lendsum.Crosscutting.Common.Hash
{
    /// <summary>
    /// Service to calculate hash
    /// </summary>
    public interface IHashService
    {
        /// <summary>
        /// Hashes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        string Hash(string value);
    }
}