namespace Lendsum.Crosscutting.Common
{
    /// <summary>
    /// Interface of DownloadService
    /// </summary>
    public interface IDownloaderService
    {
        /// <summary>
        /// Gets the bytes that the url point to.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        byte[] GetBytes(string url);
    }
}