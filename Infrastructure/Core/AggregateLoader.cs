using Lendsum.Crosscutting.Common;
using Lendsum.Crosscutting.Common.Serialization;
using Lendsum.Infrastructure.Core.Exceptions;
using Lendsum.Infrastructure.Core.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Class to load aggregates from events
    /// </summary>
    public class AggregateLoader : IAggregateLoader
    {
        private IPersistenceProvider provider;
        private ITextSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateLoader" /> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="serializer">The serializer.</param>
        public AggregateLoader(IPersistenceProvider provider, ITextSerializer serializer)
        {
            this.provider = Check.NotNull(() => provider);
            this.serializer = Check.NotNull(() => serializer);
        }

        /// <summary>
        /// Loads the specified uid from bundlers.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <returns></returns>
        public Aggregate Load(Guid uid)
        {
            AggregateEventsBundle bundle;
            int? previousBundleNumber = null;
            List<AggregateEvent> events = new List<AggregateEvent>();
            Aggregate aggregate = null;
            bool nextBundle = true;
            do
            {
                bundle = this.GetAggregateEventsBundle(uid, previousBundleNumber);

                if (bundle == null) return null;

                previousBundleNumber = bundle.PreviousBundleNumber;
                events.AddRange(bundle.Events);

                // check the snapshot
                if (bundle.Snapshot != null)
                {
                    if (!string.IsNullOrWhiteSpace(bundle.Snapshot.AggregateSerialized))
                    {
                        aggregate = this.serializer.Deserialize<Aggregate>(bundle.Snapshot.AggregateSerialized);
                        if (aggregate.SnapShotTypeVersion == bundle.Snapshot.SnapShotTypeVersion)
                        {
                            nextBundle = false;
                        }
                        else
                        {
                            aggregate = null;
                        }
                    }
                }
            } while (nextBundle == true && previousBundleNumber.HasValue);

            if (aggregate != null)
            {
                events = events.Where(x => x.AggregateVersion > aggregate.Version).ToList();
            }

            if (events == null || events.Count() == 0)
            {
                return aggregate;
            }

            var eventsInOrder = events.OrderBy(x => x.AggregateVersion);
            if (aggregate == null)
            {
                if (eventsInOrder.First().GetType() != typeof(AggregateCreatedEvent))
                {
                    throw new EventSourcingException("The first event must be AggregateCreatedEvent and for this uid is not.");
                }

                aggregate = Activator.CreateInstance((eventsInOrder.First() as AggregateCreatedEvent).AggregateType) as Aggregate;

                if (aggregate == null)
                {
                    throw new EventSourcingException("The type is not an aggregate");
                }
            }

            aggregate.ApplyEvents(eventsInOrder);
            return aggregate;
        }

        /// <summary>
        /// Gets the aggregate events bundle.
        /// </summary>
        /// <param name="aggregateUid">The aggregate uid.</param>
        /// <param name="bundleNumber">The bundle number.</param>
        /// <returns></returns>
        public AggregateEventsBundle GetAggregateEventsBundle(Guid aggregateUid, int? bundleNumber = null)
        {
            AggregateEventsBundle bundle = new AggregateEventsBundle() { AggregateUid = aggregateUid, BundleNumber = bundleNumber };
            bundle = this.provider.GetValue<AggregateEventsBundle>(bundle.DocumentKey);
            return bundle;
        }

        /// <summary>
        /// Gets all aggregate events bundles.
        /// </summary>
        /// <param name="aggregateUid">The aggregate uid.</param>
        /// <returns></returns>
        public IEnumerable<AggregateEventsBundle> GetAllAggregateEventsBundles(Guid aggregateUid)
        {
            AggregateEventsBundle bundle = new AggregateEventsBundle();
            bundle.AggregateUid = aggregateUid;
            var allBundlers = this.provider.GetValuesByKeyPattern<AggregateEventsBundle>(bundle.DocumentKey, string.Empty, true);
            return allBundlers;
        }
    }
}