using System;
using System.Collections.Generic;

namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Service to load aggregates from events
    /// </summary>
    public interface IAggregateLoader
    {
        /// <summary>
        /// Loads the aggregate with the specified uid.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <returns></returns>
        Aggregate Load(Guid uid);

        /// <summary>
        /// Gets the aggregate events bundle.
        /// </summary>
        /// <param name="aggregateUid">The aggregate uid.</param>
        /// <param name="bundleNumber">The bundle number.</param>
        /// <returns></returns>
        AggregateEventsBundle GetAggregateEventsBundle(Guid aggregateUid, int? bundleNumber = null);

        /// <summary>
        /// Gets all aggregate events bundles.
        /// </summary>
        /// <param name="aggregateUid">The aggregate uid.</param>
        /// <returns></returns>
        IEnumerable<AggregateEventsBundle> GetAllAggregateEventsBundles(Guid aggregateUid);
    }
}