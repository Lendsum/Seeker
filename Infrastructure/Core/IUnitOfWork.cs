using Lendsum.Infrastructure.Core.Dispatcher;
using Lendsum.Infrastructure.Core.Projections;
using System;
using System.Collections.Generic;

namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Interface to be implemented by unit of work.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Adds the modified projection item.
        /// </summary>
        /// <param name="item">The item.</param>
        void AddModifiedProjectionItem<T>(ProjectionItem<T> item) where T : IPersistable;

        /// <summary>
        /// Gets the projection items stored
        /// </summary>
        /// <returns></returns>
        Dictionary<string, ProjectionItem<IPersistable>> GetProjectionItems(Type type);

        /// <summary>
        /// Gets or sets the dispatcher hub.
        /// </summary>
        /// <value>
        /// The dispatcher hub.
        /// </value>
        IDispatcherHub DispatcherHub { get; set; }

        /// <summary>
        /// Commits this instance.
        /// </summary>
        void Commit();

        /// <summary>
        /// Gets the aggregate, null if it doesnt not exists
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <returns></returns>
        Aggregate GetAggregate(Guid uid);

        /// <summary>
        /// Gets the aggregate from the persistence and load all the events registered under this aggregate. Null if the aggregate doesnt exists.
        /// </summary>
        /// <param name="uid">The uid.</param>
        T GetAggregate<T>(Guid uid) where T : Aggregate;

        /// <summary>
        /// Gets the aggregate events bundle.
        /// </summary>
        /// <param name="aggregateUid">The aggregate uid.</param>
        /// <returns></returns>
        AggregateEventsBundle GetAggregateEventsBundle(Guid aggregateUid);

        /// <summary>
        /// Handles the specified event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event">The event.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "event", Justification = "I like the name")]
        void Handle<T>(T @event) where T : AggregateEvent;

        /// <summary>
        /// Marks the event as asynchronous processed.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="event">The event.</param>
        void MarkEventAsAsyncProcessed<TEvent>(TEvent @event) where TEvent : AggregateEvent;

        /// <summary>
        /// Handles the specified event.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <typeparam name="TAggregatetype">The type of the aggregatetype.</typeparam>
        /// <param name="event">The event.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = " We need the type of the aggregate")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "event", Justification = "Its already the name")]
        void HandleAndCreateIfNeeded<TEvent, TAggregatetype>(TEvent @event) where TEvent : AggregateEvent
                                                                                   where TAggregatetype : Aggregate;
    }
}