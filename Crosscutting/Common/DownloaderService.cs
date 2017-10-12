using Lendsum.Crosscutting.Common.Extensions;
using System;
using System.Net;

namespace Lendsum.Crosscutting.Common
{
    /// <summary>
    /// Service to help dowloading files from internet.
    /// </summary>
    public class DownloaderService : IDownloaderService
    {
        /// <summary>
        /// Download the resource that the url represents.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The value of the resource.</returns>
        public byte[] GetBytes(string url)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    var data = wc.DownloadData(url);
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw new LendsumException("Error downloading from url {0}".InvariantFormat(url), ex);
            }
        }
    }
}