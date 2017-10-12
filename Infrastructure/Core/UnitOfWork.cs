using Lendsum.Crosscutting.Common;
using Lendsum.Crosscutting.Common.Extensions;
using Lendsum.Infrastructure.Core.Dispatcher;
using Lendsum.Infrastructure.Core.Exceptions;
using Lendsum.Infrastructure.Core.Persistence;
using Lendsum.Infrastructure.Core.Projections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Class to handle the changed aggregates.
    /// </summary>
    public partial class UnitOfWork : IUnitOfWork
    {
        private IPersistenceProvider provider;
        private IUnitOfWorkContext workContext;
        private IAggregateEventContext aggregateEventContext;
        private IAggregateLoader loader;
        private IUnitOfWorkQueueSender queueSender;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork" /> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="loader">The loader.</param>
        /// <param name="aggregateEventContext">The aggregate event context.</param>
        /// <param name="workContext">The context.</param>
        /// <param name="queueSender">The queue sender.</param>
        public UnitOfWork(
            IPersistenceProvider provider,
            IAggregateLoader loader,
            IAggregateEventContext aggregateEventContext,
            IUnitOfWorkContext workContext,
            IUnitOfWorkQueueSender queueSender)
        {
            this.provider = Check.NotNull(() => provider);
            this.loader = Check.NotNull(() => loader);
            this.aggregateEventContext = Check.NotNull(() => aggregateEventContext);
            this.workContext = Check.NotNull(() => workContext);
            this.queueSender = Check.NotNull(() => queueSender);
            this.Clean();
        }

        /// <summary>
        /// Gets or sets the dispatcher hub.
        /// </summary>
        /// <value>
        /// The dispatcher hub.
        /// </value>
        public IDispatcherHub DispatcherHub { get; set; }

        /// <summary>
        /// Adds the modified projection item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void AddModifiedProjectionItem<T>(ProjectionItem<T> item) where T : IPersistable
        {
            if (item == null) return;

            this.GetProjectionItems(typeof(T))[item.Value.DocumentKey] = new ProjectionItem<IPersistable>(item.Value, item.ProjectionItemAction);
        }

        /// <summary>
        /// Gets the projection items stored
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Dictionary<string, ProjectionItem<IPersistable>> GetProjectionItems(Type type)
        {
            return this.workContext.ModifiedProjectionItems.GetOrAdd(type, () => new Dictionary<string, ProjectionItem<IPersistable>>());
        }

        /// <summary>
        /// Handles the specified event, the aggregate will receive it too.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event">The event.</param>
        public void Handle<T>(T @event) where T : AggregateEvent
        {
            Check.NotNull(() => @event);
            if (@event.AggregateUid == null || @event.AggregateUid == Guid.Empty)
            {
                throw new ArgumentException("The AggregateUid cannot be empty or not set");
            }

            Aggregate target;
            if (@event.GetType() == typeof(AggregateCreatedEvent))
            {
                AggregateCreatedEvent createdEvent = @event as AggregateCreatedEvent;
                target = this.CreateAggregate(createdEvent.AggregateUid, createdEvent.AggregateType);
            }
            else
            {
                target = this.GetAggregate(@event.AggregateUid);
            }

            target.ApplyEvents(new AggregateEvent[] { @event });
            this.AddEvent(@event);
        }

        /// <summary>
        /// Gets the aggregate from the persistence and load all the events registered under this aggregate. Null if the aggregate doesnt exists.
        /// </summary>
        /// <param name="uid">The uid.</param>
        public Aggregate GetAggregate(Guid uid)
        {
            if (this.workContext.AggregatedLoaded.ContainsKey(uid))
            {
                return this.workContext.AggregatedLoaded[uid];
            }

            var result = this.loader.Load(uid);
            if (result != null)
            {
                this.workContext.AggregatedLoaded.Add(uid, result);
            }

            return result;
        }

        /// <summary>
        /// Gets the aggregate from the persistence and load all the events registered under this aggregate. Null if the aggregate doesnt exists.
        /// </summary>
        /// <param name="uid">The uid.</param>
        public T GetAggregate<T>(Guid uid) where T : Aggregate
        {
            var result = this.GetAggregate(uid);
            if (result == null)
            {
                return null;
            }

            if (result.GetType() != typeof(T))
            {
                throw new EventSourcingException("The type of the agregate {0} was expected to be {1}, but was {2}".InvariantFormat(uid, typeof(T).Name, result.GetType().Name));
            }

            return result as T;
        }

        /// <summary>
        /// Handles the specified event.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <typeparam name="TAggregatetype">The type of the aggregatetype.</typeparam>
        /// <param name="event">The event.</param>
        public void HandleAndCreateIfNeeded<TEvent, TAggregatetype>(TEvent @event) where TEvent : AggregateEvent
                                                                                   where TAggregatetype : Aggregate
        {
            if (@event.AggregateUid == Guid.Empty) throw new EventSourcingException("The guid is empty");

            var aggregate = this.GetAggregate(@event.AggregateUid);
            if (aggregate == null)
            {
                this.Handle(new AggregateCreatedEvent(@event.AggregateUid, DateTime.UtcNow, typeof(TAggregatetype)));
            }

            this.Handle(@event);
        }

        /// <summary>
        /// Marks the event as asynchronous processed.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="event">The event.</param>
        public void MarkEventAsAsyncProcessed<TEvent>(TEvent @event) where TEvent : AggregateEvent
        {
            this.workContext.MarkedAsyncProcessed.AddIfNotExist(@event, (x, y) => x.AggregateUid == y.AggregateUid && x.AggregateVersion == y.AggregateVersion);
        }

        /// <summary>
        /// Commits this instance.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We want to throw exception with info about original exception and rollback exception")]
        public void Commit()
        {
            List<ProjectionItem<IPersistable>> projectionChanges = this.workContext.ModifiedProjectionItems.Values.SelectMany(x => x.Values).ToList();

            var anyDatabaseModifications = this.workContext.Events.Any() || this.workContext.MarkedAsyncProcessed.Any() || projectionChanges.Any(x => x.ProjectionItemAction != ProjectionItemActionEnum.Nothing);
            if (anyDatabaseModifications)
            {
                var newConstraints = projectionChanges.Where(x => (x.Value as ConstraintItem) != null && x.ProjectionItemAction == ProjectionItemActionEnum.UpdateOrNew).ToList();
                var releasedConstraints = projectionChanges.Where(x => (x.Value as ConstraintItem) != null && x.ProjectionItemAction == ProjectionItemActionEnum.Delete).ToList();

                // the rest of query items that are not related with constraints.
                var anotherQueryItems = projectionChanges.Where(x => !newConstraints.Contains(x) && !releasedConstraints.Contains(x)).ToList();

                using (var transaction = this.provider.BeginScope())
                {
                    Exception processException = null;
                    try
                    {
                        SaveProjections(transaction, newConstraints);
                        SaveEvents(transaction);
                        SaveProjections(transaction, releasedConstraints);
                        SaveProjections(transaction, anotherQueryItems);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        processException = ex;
                    }

                    this.workContext.Clear();

                    if (processException != null)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception ex)
                        {
                            var lendsumEx = new LendsumException(S.Invariant($"Error in the transaction rollback. {processException.Message}"), processException);
                            lendsumEx.Data.Add("RollbackException", ex);
                            throw lendsumEx;
                        }

                        throw processException;
                    }
                }
            }

            this.queueSender.Commit();
            this.Clean();
        }

        /// <summary>
        /// Gets the aggregate from the persistence and load all the events registered under this aggregate.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <param name="aggregateType">Type of the aggregate.</param>
        /// <returns></returns>
        /// <exception cref="EventSourcingException">
        /// The aggregate type is not as expected
        /// or
        /// The type is not an aggregate
        /// </exception>
        private Aggregate CreateAggregate(Guid uid, Type aggregateType)
        {
            Aggregate result;
            if (this.workContext.AggregatedLoaded.ContainsKey(uid))
            {
                throw new ConcurrencyException("The aggregate is already created, we cannot create it");
            }

            var bundle = this.GetAggregateEventsBundle(uid);

            if (bundle != null)
            {
                throw new ConcurrencyException("The aggregate is already created, we cannot create it");
            }

            result = Activator.CreateInstance(aggregateType) as Aggregate;
            if (result == null)
            {
                throw new EventSourcingException("The type is not an aggregate");
            }

            this.workContext.AggregatedLoaded.Add(uid, result);
            return result;
        }

        private void AddEvent(AggregateEvent @event)
        {
            Check.NotNull(() => @event);
            this.workContext.Events.Enqueue(@event);
            this.DispatcherHub?.Dispatch(@event);
        }

        private static void SaveProjections(IPersistenceProvider transaction, IEnumerable<ProjectionItem<IPersistable>> changes)
        {
            foreach (var item in changes)
            {
                switch (item.ProjectionItemAction)
                {
                    case ProjectionItemActionEnum.Nothing:
                        break;

                    case ProjectionItemActionEnum.New:
                        var result = transaction.Insert(item.Value);
                        if (result != PersistenceResultEnum.Success)
                        {
                            throw new ConcurrencyException("The item with id {0} is already inserted".InvariantFormat(item.Value.DocumentKey));
                        }

                        break;

                    case ProjectionItemActionEnum.UpdateOrNew:

                        result = transaction.UpdateOrInsert(item.Value);
                        if (result != PersistenceResultEnum.Success)
                        {
                            throw new ConcurrencyException("The item with id {0} has been updated before this update".InvariantFormat(item.Value.DocumentKey));
                        }

                        break;

                    case ProjectionItemActionEnum.Delete:

                        result = transaction.Delete(item.Value);
                        if (result != PersistenceResultEnum.Success)
                        {
                            throw new ConcurrencyException("The item with id {0} cannot be deleted.".InvariantFormat(item.Value.DocumentKey));
                        }

                        break;

                    default:
                        throw new EventSourcingException("The type of the item is invalid");
                }
            }
        }

        private void SaveEvents(IPersistenceProvider transaction)
        {
            var allAggregateUis = this.workContext.Events.Select(x => x.AggregateUid)
                .Union(this.workContext.MarkedAsyncProcessed.Select(x => x.AggregateUid))
                .Distinct();

            foreach (var aggregateUid in allAggregateUis)
            {
                var bundles = GetAndUpdateAggregateBundles(aggregateUid);

                foreach (var bundle in bundles)
                {
                    var insertResult = transaction.UpdateOrInsert(bundle);

                    if (insertResult != PersistenceResultEnum.Success)
                    {
                        throw new ConcurrencyException(S.Invariant($"The Aggregate Bundle with id {aggregateUid} cannot be saved becasue the document is always out of date"));
                    }
                }
            }
        }

        private IEnumerable<AggregateEventsBundle> GetAndUpdateAggregateBundles(Guid aggregateUid)
        {
            var newEvents = this.workContext.Events.Where(x => x.AggregateUid == aggregateUid);
            var markedAsyn = this.workContext.MarkedAsyncProcessed.Where(x => x.AggregateUid == aggregateUid);

            int minVersion = 0;
            var allEvents = newEvents.Union(markedAsyn);
            if (allEvents.Count() > 0) minVersion = allEvents.Min(x => x.AggregateVersion);

            var currentBundle = this.loader.GetAggregateEventsBundle(aggregateUid);
            if (currentBundle == null) currentBundle = new AggregateEventsBundle() { AggregateUid = aggregateUid };

            List<AggregateEventsBundle> allBundles = GetAllNecessaryBundles(aggregateUid, minVersion, currentBundle);

            // save new events
            foreach (var @event in newEvents)
            {
                if (allBundles.Any(bundle => bundle.Events.Any(x => x.AggregateVersion == @event.AggregateVersion)))
                {
                    throw new ConcurrencyException("The item with id {0} is already inserted".InvariantFormat(@event.DocumentKey));
                }
                else
                {
                    @event.SystemVersion = this.provider.ReserveCounter();
                    this.aggregateEventContext.AttachToEvent(@event);
                    currentBundle.Events = currentBundle.Events.FluentAdd(@event);
                }
            }

            // save events marked async.
            foreach (var @event in markedAsyn)
            {
                var bundle = allBundles.Where(x => x.Events.Any(y => y.AggregateVersion == @event.AggregateVersion)).FirstOrDefault();
                if (bundle == null)
                {
                    throw new LendsumException(S.Invariant($"The event version {@event.AggregateVersion} of aggregate {aggregateUid} cannot be marked as async processed because cannot be found"));
                }

                bundle.Events.Where(x => x.AggregateVersion == @event.AggregateVersion).First().AsyncProcessed = true;
            }

            return allBundles;
        }

        private List<AggregateEventsBundle> GetAllNecessaryBundles(Guid aggregateUid, int minVersion, AggregateEventsBundle currentBundle)
        {
            List<AggregateEventsBundle> allBundles = new List<AggregateEventsBundle>();
            allBundles.Add(currentBundle);
            var previousBundleNumber = currentBundle.PreviousBundleNumber;

            // find all necessary bundles
            bool found = false;

            while (found == false)
            {
                if (allBundles.Any(b => b.Events.Any(e => e.AggregateVersion >= minVersion)))
                {
                    found = true;
                }
                else
                {
                    // take next bundle
                    if (previousBundleNumber.HasValue)
                    {
                        var previousBundle = this.loader.GetAggregateEventsBundle(aggregateUid, previousBundleNumber.Value);
                        if (previousBundle == null)
                        {
                            throw new LendsumException(S.Invariant($"The bundle {previousBundleNumber.Value} of aggregate {aggregateUid} cannot be found"));
                        }

                        allBundles.Add(previousBundle);
                        previousBundleNumber = previousBundle.PreviousBundleNumber;
                    }
                    else
                    {
                        found = true;
                    }
                }
            }

            return allBundles;
        }

        /// <summary>
        /// Gets the aggregate events bundle.
        /// </summary>
        /// <param name="aggregateUid">The aggregate uid.</param>
        /// <returns></returns>
        public AggregateEventsBundle GetAggregateEventsBundle(Guid aggregateUid)
        {
            return this.loader.GetAggregateEventsBundle(aggregateUid);
        }

        /// <summary>
        /// Cleans this instance.
        /// </summary>
        private void Clean()
        {
            this.workContext.Clear();
        }
    }
}