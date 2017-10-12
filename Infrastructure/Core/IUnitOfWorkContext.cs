using Lendsum.Infrastructure.Core.Projections;
using System;
using System.Collections.Generic;

namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    ///
    /// </summary>
    public interface IUnitOfWorkContext
    {
        /// <summary>
        /// Gets the aggregated loaded.
        /// </summary>
        /// <value>
        /// The aggregated loaded.
        /// </value>
        Dictionary<Guid, Aggregate> AggregatedLoaded { get; }

        /// <summary>
        /// Gets the events.
        /// </summary>
        /// <value>
        /// The events.
        /// </value>
        Queue<AggregateEvent> Events { get; }

        /// <summary>
        /// Gets the marked asynchronous processed.
        /// </summary>
        /// <value>
        /// The marked asynchronous processed.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        List<AggregateEvent> MarkedAsyncProcessed { get; }

        /// <summary>
        /// Gets the modified projection items.
        /// </summary>
        /// <value>
        /// The modified projection items.
        /// </value>
        Dictionary<Type, Dictionary<string, ProjectionItem<IPersistable>>> ModifiedProjectionItems { get; }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();
    }
}