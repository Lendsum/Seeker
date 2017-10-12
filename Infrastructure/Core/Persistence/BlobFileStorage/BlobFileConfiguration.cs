using System.Configuration;

namespace Lendsum.Infrastructure.Core.Persistence.BlobFileStorage
{
    /// <summary>
    /// Class to wrap rabbitMQ configuration
    /// </summary>
    public class BlobFileConfiguration
    {
        public BlobFileConfiguration()
        {
            this.AzureFolderRoot = @"/blobs";
        }
        /// <summary>
        /// Gets or sets the hostname.
        /// </summary>
        /// <value>
        /// The hostname.
        /// </value>
        public string Folder  
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the azure credentials.
        /// </summary>
        /// <value>
        /// The azure credentials.
        /// </value>
        public string AzureConnectionString
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the azure folder root.
        /// </summary>
        /// <value>
        /// The azure folder root.
        /// </value>
        public string AzureFolderRoot
        {
            get;set;
        }

        /// <summary>
        /// Gets or sets the azure shared reference.
        /// </summary>
        /// <value>
        /// The azure shared reference.
        /// </value>
        public string AzureSharedReference
        {
            get; set;
        }
    }
}