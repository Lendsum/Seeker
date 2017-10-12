using System;

namespace Lendsum.Infrastructure.Core.Dispatcher
{
    /// <summary>
    /// Data to delay a call for the event dispatcher hub
    /// </summary>
    public class EventDispatcherData
    {
        /// <summary>
        /// Gets or sets the aggregate uid.
        /// </summary>
        public Guid AggregateUid { get; set; }
    }
}