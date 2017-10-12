using System;

namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Base class for request dtos.
    /// </summary>
    public class RequestDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestDto"/> class.
        /// </summary>
        public RequestDto() { this.When = DateTime.UtcNow; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestDto"/> class.
        /// </summary>
        /// <param name="aggregateUid">The customer uid.</param>
        /// <param name="when">The when.</param>
        public RequestDto(Guid aggregateUid, DateTime when)
        {
            this.AggregateUid = aggregateUid;
            this.When = when;
        }

        /// <summary>
        /// Gets the customer uid.
        /// </summary>
        /// <value>
        /// The customer uid.
        /// </value>
        public Guid AggregateUid { get; set; }

        /// <summary>
        /// Gets or sets when the hint was produced
        /// </summary>
        /// <value>
        /// The datetime.
        /// </value>
        public DateTime When { get; set; }
    }
}