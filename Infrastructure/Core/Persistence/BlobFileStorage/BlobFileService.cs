using Lendsum.Crosscutting.Common;
using Lendsum.Crosscutting.Common.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text;

namespace Lendsum.Infrastructure.Core.Persistence.BlobFileStorage
{
    /// <summary>
    /// Blob storage
    /// </summary>
    public class BlobFileService : IBlobService
    {
        private IDateTimeProvider dateTimeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobFileService"/> class.
        /// </summary>
        public BlobFileService(IDateTimeProvider dateTimeProvider, IOptions<BlobFileConfiguration> configuration)
        {
            this.Config = configuration.Value;
            this.dateTimeProvider = Check.NotNull(() => dateTimeProvider);
        }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public BlobFileConfiguration Config { get; set; }

        /// <summary>
        /// Deletes the BLOB.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void DeleteBlob(string key)
        {
            Check.NotNullOrEmpty(() => key);
            var path = Encoding.UTF8.ParseFromBase64(key);
            path = RemoveDirectorySlashFromStart(path);
            var fullPath = Path.Combine(this.Config.Folder, path);

            if (File.Exists(fullPath)) File.Delete(fullPath);
        }

        /// <summary>
        /// Reads the BLOB storing with the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public byte[] ReadBlob(string key)
        {
            Check.NotNullOrEmpty(() => key);
            var path = Encoding.UTF8.ParseFromBase64(key);

            path = RemoveDirectorySlashFromStart(path);

            var fullPath = Path.Combine(this.Config.Folder, path);

            return File.ReadAllBytes(fullPath);
        }

        /// <summary>
        /// Gets the name of the file stored in the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string GetFileName(string key)
        {
            Check.NotNullOrEmpty(() => key);
            var path = Encoding.UTF8.ParseFromBase64(key);
            var fullPath = Path.Combine(this.Config.Folder, path);
            return Path.GetFileName(fullPath);
        }

        /// <summary>
        /// Updates the or insert BLOB.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The key to recover the blob later
        /// </returns>
        public string UpdateOrInsertBlob(string fileName, byte[] value)
        {
            try
            {
                var fullPath = PathHelper.GetFullPathForDayClassification(this.dateTimeProvider.UtcNow, fileName, this.Config.Folder);
                File.WriteAllBytes(fullPath, value);

                var result = fullPath.Replace(this.Config.Folder, string.Empty);
                result = RemoveDirectorySlashFromStart(result);

                return Encoding.UTF8.ToBase64(result);
            }
            catch (Exception ex)
            {
                throw new LendsumException(S.Invariant($"The filename {fileName} cannot be saved in {this.Config.Folder}"), ex);
            }
        }

        private static string RemoveDirectorySlashFromStart(string path)
        {
            while (path.Length > 0 && path[0] == '\\')
            {
                path = path.Substring(1, path.Length - 1);
            }

            return path;
        }
    }
}