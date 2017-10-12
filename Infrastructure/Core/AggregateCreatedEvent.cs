using Lendsum.Crosscutting.Common;
using System;

namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Generic event to create a new aggregate
    /// </summary>
    public class AggregateCreatedEvent : AggregateEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateCreatedEvent"/> class.
        /// This constructor only has to be used by Activator
        /// </summary>
        public AggregateCreatedEvent()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateCreatedEvent" /> class.
        /// </summary>
        /// <param name="aggregateUid">The new unique identifier.</param>
        /// <param name="when">The when.</param>
        /// <param name="aggregateType">Type of the aggregate.</param>
        public AggregateCreatedEvent(Guid aggregateUid, DateTime when, Type aggregateType)
        {
            Check.NotNull(() => aggregateType);
            this.AggregateUid = aggregateUid;
            this.When = when;
            this.AggregateType = aggregateType;
        }

        /// <summary>
        /// Gets or sets the type of the aggregate.
        /// </summary>
        /// <value>
        /// The type of the aggregate.
        /// </value>
        public Type AggregateType { get; set; }
    }
}