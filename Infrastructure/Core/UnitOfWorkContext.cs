using Lendsum.Crosscutting.Common;
using Lendsum.Infrastructure.Core.Projections;
using System;
using System.Collections.Generic;

namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    ///
    /// </summary>
    public class UnitOfWorkContext : IUnitOfWorkContext
    {
        private IThreadContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkContext"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UnitOfWorkContext(IThreadContext context)
        {
            this.context = Check.NotNull(() => context);
        }

        /// <summary>
        /// Gets the events.
        /// </summary>
        /// <value>
        /// The events.
        /// </value>
        public Queue<AggregateEvent> Events
        {
            get
            {
                return this.GetAndInit<Queue<AggregateEvent>>("unitofwork.events");
            }
        }

        /// <summary>
        /// Gets the marked asynchronous processed.
        /// </summary>
        /// <value>
        /// The marked asynchronous processed.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public List<AggregateEvent> MarkedAsyncProcessed
        {
            get
            {
                return this.GetAndInit<List<AggregateEvent>>("unitofwork.markedAsyncProcessed");
            }
        }

        /// <summary>
        /// Gets the modified projection items.
        /// </summary>
        /// <value>
        /// The modified projection items.
        /// </value>
        public Dictionary<Type, Dictionary<string, ProjectionItem<IPersistable>>> ModifiedProjectionItems
        {
            get
            {
                return this.GetAndInit<Dictionary<Type, Dictionary<string, ProjectionItem<IPersistable>>>>("unitofwork.modifiedProjectionItems");
            }
        }

        /// <summary>
        /// Gets the commited projection items.
        /// </summary>
        /// <value>
        /// The commited projection items.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "We need speed so we cannot pass collections")]
        public List<ProjectionItem<IPersistable>> CommitedProjectionItems
        {
            get
            {
                return this.GetAndInit<List<ProjectionItem<IPersistable>>>("unitofwork.commitedProjectionItems");
            }
        }

        /// <summary>
        /// Gets the aggregated loaded.
        /// </summary>
        /// <value>
        /// The aggregated loaded.
        /// </value>
        public Dictionary<Guid, Aggregate> AggregatedLoaded
        {
            get
            {
                return this.GetAndInit<Dictionary<Guid, Aggregate>>("unitofwork.aggregatedLoaded");
            }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            this.Events.Clear();
            this.ModifiedProjectionItems.Clear();
            this.CommitedProjectionItems.Clear();
            this.AggregatedLoaded.Clear();
        }

        private T GetAndInit<T>(string key) where T : new()
        {
            T value = this.context.GetValue<T>(key);
            if (value == null)
            {
                value = new T();
                this.context.Update<T>(value, key);
            }

            return value;
        }
    }
}