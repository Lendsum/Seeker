namespace Lendsum.Infrastructure.Core.Locks
{
    /// <summary>
    ///
    /// </summary>
    public class LockerInfo : IPersistable
    {
        /// <summary>
        /// Gets or sets the name of the job.
        /// </summary>
        /// <value>
        /// The name of the job.
        /// </value>
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [in progress].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [in progress]; otherwise, <c>false</c>.
        /// </value>
        public bool InProgress { get; set; }

        /// <summary>
        /// Gets or sets the cas. This value must be used only by persistance layer.
        /// </summary>
        /// <value>
        /// The cas.
        /// </value>
        public ulong Cas { get; set; }

        /// <summary>
        /// The document key
        /// </summary>
        public string DocumentKey => this.DocumentType + this.ItemName;

        /// <summary>
        /// The document type
        /// </summary>
        public string DocumentType => "LockInfo:";
    }
}