using Lendsum.Crosscutting.Common;
using Lendsum.Crosscutting.Common.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;

namespace Lendsum.Infrastructure.Core.Persistence.BlobFileStorage
{
    /// <summary>
    ///
    /// </summary>
    public class AzureBlobWrapper : IBlobService
    {
        private BlobFileConfiguration config;
        private CloudFileClient fileClient;
        private CloudFileShare share;

        private IDateTimeProvider dateTimeProvider;
        private ILogger logger;
        private Dictionary<string, CloudFileDirectory> directories = new Dictionary<string, CloudFileDirectory>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobWrapper"/> class.
        /// </summary>
        public AzureBlobWrapper(IDateTimeProvider dateTimeProvider, ILogger log, IOptions<BlobFileConfiguration> configuration)
        {
            this.dateTimeProvider = Check.NotNull(() => dateTimeProvider);
            this.logger = Check.NotNull(() => log);
            this.config = configuration.Value;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(config.AzureConnectionString);
            this.fileClient = storageAccount.CreateCloudFileClient();
            this.share = fileClient.GetShareReference(config.AzureSharedReference);
        }

        /// <summary>
        /// Deletes the BLOB.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <exception cref="NotImplementedException"></exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void DeleteBlob(string key)
        {
            try
            {
                var cloudFile = GetCloudFileFromKey(key);
                cloudFile.DeleteIfExistsAsync();
            }
            catch (Exception)
            {
                logger.LogWarning(S.Invariant($"error deleting {key}, the blob continue existing"));
            }
        }

        /// <summary>
        /// Reads the BLOB storing with the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public byte[] ReadBlob(string key)
        {
            CloudFile cloudFile = GetCloudFileFromKey(key);

            try
            {
                if (!cloudFile.ExistsAsync().Result) return null;
                cloudFile.FetchAttributesAsync().Wait();

                byte[] content = new byte[cloudFile.Properties.Length];
                cloudFile.DownloadToByteArrayAsync(content, 0).Wait();

                return content;
            }
            catch (Exception ex)
            {
                throw new LendsumException(S.Invariant($"The filename {key} cannot be read in {config.AzureSharedReference} "), ex);
            }
        }

        private CloudFile GetCloudFileFromKey(string key)
        {
            Check.NotNullOrEmpty(() => key);

            var path = Encoding.UTF8.ParseFromBase64(key);
            path = Path.Combine(config.AzureFolderRoot, path);
            path = RemoveDirectorySlashFromStart(path);
            var file = Path.GetFileName(path);
            var dir = Path.GetDirectoryName(path);
            var cloudDir = this.GetDirectoryForToday(dir);
            var cloudFile = cloudDir.GetFileReference(file);
            return cloudFile;
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
            return Path.GetFileName(path);
        }

        /// <summary>
        /// Updates the or insert BLOB.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The key to recover the blob later
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "theFolders")]
        public string UpdateOrInsertBlob(string fileName, byte[] value)
        {
            if (value == null) throw new LendsumException(S.Invariant($"The fileName {fileName} cannot be inserted because value has no data"));

            try
            {
                var fullPath = this.FullPath;
                var dir = GetDirectoryForToday(fullPath);
                var file = dir.GetFileReference(fileName);
                file.UploadFromByteArrayAsync(value, 0, value.Length).Wait();

                fullPath = Path.Combine(fullPath, fileName);
                var result = fullPath.Replace(config.AzureFolderRoot, string.Empty);
                result = RemoveDirectorySlashFromStart(result);

                return Encoding.UTF8.ToBase64(result);
            }
            catch (Exception ex)
            {
                throw new LendsumException(S.Invariant($"The filename {fileName} cannot be update or insert in {config.AzureSharedReference} "), ex);
            }
        }

        private string FullPath
        {
            get
            {
                var currentDate = this.dateTimeProvider.UtcNow;
                string year = currentDate.Year.ToString("D4", CultureInfo.InvariantCulture);
                string month = currentDate.Month.ToString("D2", CultureInfo.InvariantCulture);
                string day = currentDate.Day.ToString("D2", CultureInfo.InvariantCulture);

                return Path.Combine(config.AzureFolderRoot, year, month, day);
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

        private CloudFileDirectory GetDirectoryForToday(string fullPath)
        {
            if (directories.ContainsKey(fullPath))
            {
                return directories[fullPath];
            }
            else
            {
                lock (directories)
                {
                    var result = this.directories.GetOrAdd(fullPath, () =>
                    {
                        return this.CreateDirectory(fullPath);
                    });

                    return result;
                }
            }
        }

        private CloudFileDirectory CreateDirectory(string fullPath)
        {
            return Retrier.Retry<CloudFileDirectory>(5, () =>
            {
                var root = this.share.GetRootDirectoryReference();
                var segments = fullPath.Split('\\');
                CloudFileDirectory lastDir = null;
                foreach (var segment in segments)
                {
                    if (lastDir == null)
                    {
                        lastDir = root.GetDirectoryReference(segment);
                    }
                    else
                    {
                        lastDir = lastDir.GetDirectoryReference(segment);
                    }

                    if (!lastDir.ExistsAsync().Result)
                    {
                        lastDir.CreateAsync().Wait();
                    }
                }

                return lastDir;
            });
        }
    }
}