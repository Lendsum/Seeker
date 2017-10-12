using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Class to encapsulate an event produced in an aggregate.
    /// </summary>
    public class AggregateEvent
    {
        /// <summary>
        /// The prefix of an aggregate.
        /// </summary>
        private const string Prefix = "Event:";

        /// <summary>
        /// The maximum aggregate version supported.
        /// </summary>
        public const int MaxAggregateVersion = 99999999;

        /// <summary>
        /// Gets the key with aggregate uid.
        /// </summary>
        /// <param name="aggregateUid">The aggregate uid.</param>
        /// <returns></returns>
        public static string GetKeyWithAggregateUid(Guid aggregateUid)
        {
            return Prefix + aggregateUid.ToString() + ":";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateEvent"/> class.
        /// </summary>
        public AggregateEvent()
        {
            this.When = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets or sets a value indicating if this event has been processed asyncronously
        /// </summary>
        public virtual bool AsyncProcessed { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating if this message must be sent to listeners, asyncs or not.
        /// </summary>
        public virtual bool ControlEvent { get; set; } = false;

        /// <summary>
        /// Gets the type of the document.
        /// </summary>
        public string DocumentType => GetKeyWithAggregateUid(this.AggregateUid);

        /// <summary>
        /// Gets the document key.
        /// </summary>
        public string DocumentKey => this.DocumentType + this.AggregateVersion.ToString("D8", CultureInfo.InvariantCulture);

        /// <summary>
        /// Gets or sets the aggregate uid where this events will go.
        /// </summary>
        public Guid AggregateUid { get; set; }

        /// <summary>
        /// Gets or sets the date when this events is generated for the first time.
        /// </summary>
        public DateTime When { get; set; }

        /// <summary>
        /// Gets or sets the version of the aggregate before the event is handled by the agregate
        /// </summary>
        public int AggregateVersion { get; set; }

        /// <summary>
        /// Gets or sets the system version. To set by the event sourcing provider
        /// </summary>
        public ulong SystemVersion { get; set; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Necessary for serialization")]
        public IDictionary<string, string> Context { get; set; }
    }
}