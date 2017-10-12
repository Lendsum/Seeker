namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Class to store and recover an aggregate snapshot
    /// </summary>
    public class AggregateSnapshot
    {
        /// <summary>
        /// Gets or sets the aggregate version.
        /// </summary>
        /// <value>
        /// The aggregate version.
        /// </value>
        public int AggregateVersion { get; set; }

        /// <summary>
        /// Gets or sets the aggregate serialized.
        /// </summary>
        /// <value>
        /// The aggregate serialized.
        /// </value>
        public string AggregateSerialized { get; set; }

        /// <summary>
        /// Gets or sets the snap shot version.
        /// </summary>
        /// <value>
        /// The snap shot version.
        /// </value>
        public int SnapShotTypeVersion { get; set; }
    }
}