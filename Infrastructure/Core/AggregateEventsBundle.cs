using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Bundle to keep all events of one aggregate together.
    /// </summary>
    public class AggregateEventsBundle : IPersistable
    {
        /// <summary>
        /// Prefix of this persistable.
        /// </summary>
        public const string AggregateEventsBundlePrefix = "AggregateEventsBundle";

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateEventsBundle"/> class.
        /// </summary>
        public AggregateEventsBundle()
        {
            this.Events = new AggregateEvent[0];
        }

        /// <summary>
        /// Gets or sets the bundle number.
        /// </summary>
        /// <value>
        /// The bundle number.
        /// </value>
        public int? BundleNumber { get; set; }

        /// <summary>
        /// Gets or sets the previous bundle key.
        /// </summary>
        /// <value>
        /// The previous bundle key.
        /// </value>
        public int? PreviousBundleNumber { get; set; }

        /// <summary>
        /// Gets or sets the aggregate uid.
        /// </summary>
        /// <value>
        /// The aggregate uid.
        /// </value>
        public Guid AggregateUid { get; set; }

        /// <summary>
        /// Gets or sets the snapshot.
        /// </summary>
        /// <value>
        /// The snapshot.
        /// </value>
        public AggregateSnapshot Snapshot { get; set; }

        /// <summary>
        /// Gets or sets the events.
        /// </summary>
        /// <value>
        /// The events.
        /// </value>
        public IEnumerable<AggregateEvent> Events { get; set; }

        /// <summary>
        /// Gets or sets the cas. This value must be used only by persistance layer.
        /// </summary>
        /// <value>
        /// The cas.
        /// </value>
        public ulong Cas { get; set; }

        /// <summary>
        /// Gets the document key.
        /// </summary>
        /// <value>
        /// The document key.
        /// </value>
        public string DocumentKey
        {
            get
            {
                var result = this.DocumentType + this.AggregateUid;
                if (this.BundleNumber.HasValue)
                {
                    result = result + ":" + BundleNumber.Value.ToString("D8", CultureInfo.InvariantCulture);
                }

                return result;
            }
        }

        /// <summary>
        /// Gets or sets the type of the document.
        /// </summary>
        /// <value>
        /// The type of the document.
        /// </value>
        public string DocumentType => AggregateEventsBundlePrefix + ":";
    }
}