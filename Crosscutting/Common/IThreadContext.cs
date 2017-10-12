namespace Lendsum.Crosscutting.Common
{
    /// <summary>
    /// Thread context interface
    /// </summary>
    public interface IThreadContext
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        T GetValue<T>(string key = "default");

        /// <summary>
        /// Updates, or insert if the key value doesnt exist, the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="key">The key.</param>
        void Update<T>(T value, string key = "default");
    }
}