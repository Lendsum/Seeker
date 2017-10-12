namespace Lendsum.Infrastructure.Core.Persistence
{
    /// <summary>
    ///
    /// </summary>
    public interface IBlobService
    {
        /// <summary>
        /// Updates the or insert BLOB.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="value">The value.</param>
        /// <returns>The key to recover the blob later</returns>
        string UpdateOrInsertBlob(string fileName, byte[] value);

        /// <summary>
        /// Reads the BLOB storing with the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        byte[] ReadBlob(string key);

        /// <summary>
        /// Gets the name of the file stored in the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        string GetFileName(string key);

        /// <summary>
        /// Deletes the BLOB.
        /// </summary>
        /// <param name="key">The key.</param>
        void DeleteBlob(string key);
    }
}