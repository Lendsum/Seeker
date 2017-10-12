using Lendsum.Crosscutting.Common;
using Lendsum.Crosscutting.Common.Extensions;
using Lendsum.Infrastructure.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Base class for aggregate roots
    /// </summary>
    public abstract class Aggregate :
        IApplyEvent<AggregateCreatedEvent>
    {
        private static Dictionary<Type, Dictionary<Type, PropertyInfo[]>> propagateCache = new Dictionary<Type, Dictionary<Type, PropertyInfo[]>>();

        private int version;
        private List<AggregateEvent> events = new List<AggregateEvent>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Aggregate"/> class.
        /// </summary>
        protected Aggregate()
        {
        }

        /// <summary>
        /// Gets the events.
        /// </summary>
        /// <value>
        /// The events.
        /// </value>
        protected internal IEnumerable<AggregateEvent> Events { get { return this.events.ToArray(); } }

        /// <summary>
        /// Gets or sets the uid that represents the aggregate.
        /// </summary>
        /// <value>
        /// The uid.
        /// </value>
        public Guid Uid { get; set; }

        /// <summary>
        /// Gets the version of this aggregate, every events creates a new version
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public int Version { get { return this.version; } set { this.version = value; } }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        /// <value>
        /// The creation date.
        /// </value>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets a value indicating if this aggregate can be snapshoted.
        /// </summary>
        public virtual bool ReadyForSnapShot { get { return false; } }

        /// <summary>
        /// Gets the version of the snapshot that the aggregate can handle.
        /// </summary>
        /// <value>
        /// The code version.
        /// </value>
        public virtual int SnapShotTypeVersion => 0;

        /// <summary>
        /// Applies the specified event.
        /// </summary>
        /// <param name="incomingEvent">The event.</param>
        /// <exception cref="EventSourcingException">The aggregate type {0} is not equal than the new event can handle with type {1}
        ///                     .InvariantFormat(this.GetType().ToString(), @event.AggregateType)</exception>
        public void Apply(AggregateCreatedEvent incomingEvent)
        {
            Check.NotNull(() => incomingEvent);

            if (incomingEvent.AggregateType != this.GetType())
            {
                throw new EventSourcingException(
                    S.Invariant($"The aggregate type is {this.GetType().ToString()} but the incoming aggregate created event type is {incomingEvent.AggregateType}"));
            }

            if (this.events.Count > 0)
            {
                throw new NewAggregateEventException("The aggregate {0} cannot receive the CreatedEvent more than once".InvariantFormat(incomingEvent.AggregateUid));
            }

            this.Uid = incomingEvent.AggregateUid;
            this.CreationDate = incomingEvent.When;
        }

        /// <summary>
        /// Enuerates the supplied events and applies them in order to the aggregate.
        /// </summary>
        /// <param name="events"></param>
        public void ApplyEvents(IEnumerable<AggregateEvent> events)
        {
            if (events != null)
            {
                foreach (var e in events)
                    GetType().GetMethod("ApplyOneEvent")
                        .MakeGenericMethod(e.GetType())
                        .Invoke(this, new object[] { e });
            }
        }

        /// <summary>
        /// Applies a single event to the aggregate.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="ev"></param>
        public void ApplyOneEvent<TEvent>(TEvent ev) where TEvent : AggregateEvent
        {
            var applier = this as IApplyEvent<TEvent>;
            if (applier != null)
            {
                applier.Apply(ev);
            }

            this.AddEvent(ev);
            this.Propagate<TEvent>(ev);
        }

        private void Propagate<TEvent>(TEvent e) where TEvent : AggregateEvent
        {
            foreach (var subAggregate in this.GetSubAggregatesToPropagate<TEvent>())
            {
                (subAggregate.GetValue(this) as IApplyEvent<TEvent>).Apply(e);
            }
        }

        private PropertyInfo[] GetSubAggregatesToPropagate<TEvent>() where TEvent : AggregateEvent
        {
            if (propagateCache.ContainsKey(this.GetType()) && propagateCache[this.GetType()].ContainsKey(typeof(TEvent)))
            {
                return propagateCache[this.GetType()][typeof(TEvent)];
            }
            else
            {
                lock (propagateCache)
                {
                    var aggregateCache = propagateCache.GetOrAdd(this.GetType(), () => new Dictionary<Type, PropertyInfo[]>());
                    var result = aggregateCache.GetOrAdd(typeof(TEvent), () => this.GetType()
                          .GetProperties()
                          .Where(x =>
                            typeof(IApplyEvent<TEvent>).IsAssignableFrom(x.PropertyType))
                          .ToArray());

                    return result;
                }
            }
        }

        /// <summary>
        /// Adds the event.
        /// </summary>
        /// <param name="event">The event.</param>
        private void AddEvent(AggregateEvent @event)
        {
            Check.NotNull(() => @event);

            if (@event.AggregateVersion > 0)
            {
                if (this.version >= @event.AggregateVersion)
                {
                    throw new EventSourcingException(
                   S.Invariant($"The aggregate {Uid} has already consumed an event with version {this.version} before and the event has version {@event.AggregateVersion}"));
                }

                this.version = @event.AggregateVersion;
            }
            else
            {
                @event.AggregateVersion = Interlocked.Increment(ref this.version);
            }

            @event.AggregateUid = this.Uid;
            this.events.Add(@event);
        }
    }
}