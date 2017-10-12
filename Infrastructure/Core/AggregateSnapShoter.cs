using Lendsum.Crosscutting.Common;
using Lendsum.Crosscutting.Common.Serialization;
using Lendsum.Infrastructure.Core.Persistence;
using System;
using System.Linq;

namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Saga to split aggregate bundlers  with snapshot technicque.
    /// </summary>
    public class AggregateSnapShoter : IAggregateSnapShoter
    {
        private IAggregateLoader loader;
        private IPersistenceProvider provider;
        private ITextSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateSnapShoter" /> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="loader">The loader.</param>
        /// <param name="serializer">The serializer.</param>
        public AggregateSnapShoter(IPersistenceProvider provider, IAggregateLoader loader, ITextSerializer serializer)
        {
            this.provider = Check.NotNull(() => provider);
            this.serializer = Check.NotNull(() => serializer);
            this.loader = Check.NotNull(() => loader);
        }

        /// <summary>
        /// Consumes the specified incoming event.
        /// </summary>
        /// <param name="aggregateUid">The aggregate uid.</param>
        public void SnapShotIfNeeded(Guid aggregateUid)
        {
            var aggregate = this.loader.Load(aggregateUid);
            if (aggregate != null
                && aggregate.ReadyForSnapShot
                && aggregate.Events.Count() > 0
                && aggregate.Events.All(x => x.AsyncProcessed == true))
            {
                var raw = this.serializer.Serialize(aggregate);
                AggregateSnapshot snapshot = new AggregateSnapshot();
                snapshot.AggregateSerialized = raw;
                snapshot.AggregateVersion = aggregate.Version;
                snapshot.SnapShotTypeVersion = aggregate.SnapShotTypeVersion;

                var currentBundler = this.loader.GetAggregateEventsBundle(aggregateUid);
                AggregateEventsBundle oldBundler = null;

                if (currentBundler.Events.Where(x => x.AggregateVersion <= aggregate.Version).Any())
                {
                    oldBundler = new AggregateEventsBundle();
                    oldBundler.AggregateUid = aggregateUid;
                    oldBundler.PreviousBundleNumber = currentBundler.PreviousBundleNumber;
                    oldBundler.BundleNumber = (currentBundler.PreviousBundleNumber ?? 0) + 1;
                    oldBundler.Events = currentBundler.Events.Where(x => x.AggregateVersion <= aggregate.Version).ToArray();
                }

                var newBundler = new AggregateEventsBundle();
                newBundler.AggregateUid = aggregateUid;
                newBundler.Snapshot = snapshot;

                newBundler.PreviousBundleNumber = oldBundler == null ? currentBundler.PreviousBundleNumber : oldBundler.BundleNumber;
                newBundler.Events = currentBundler.Events.Where(x => x.AggregateVersion > aggregate.Version).ToArray();
                newBundler.Cas = currentBundler.Cas;

                using (var transaction = this.provider.BeginScope())
                {
                    var result = this.provider.UpdateOrInsert(newBundler);
                    if (result == PersistenceResultEnum.Success)
                    {
                        if (oldBundler != null)
                        {
                            this.provider.Insert(oldBundler);
                        }
                    }

                    transaction.Commit();
                }
            }
        }
    }
}