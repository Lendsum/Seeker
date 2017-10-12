using Lendsum.Crosscutting.Common;
using Lendsum.Infrastructure.Core.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lendsum.Infrastructure.Core.Projections
{
    /// <summary>
    /// Class to implement a basic query. it also implements the Listener to receive the events and change the query according to that.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ProjectionBase<T> where T : class, IPersistable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionBase{T}"/> class.
        /// </summary>
        protected ProjectionBase(IPersistenceProvider provider, IUnitOfWork work)
        {
            this.Provider = Check.NotNull(() => provider);
            this.Work = Check.NotNull(() => work);
        }

        /// <summary>
        /// Gets the work.
        /// </summary>
        /// <value>
        /// The work.
        /// </value>
        protected IUnitOfWork Work { get; private set; }

        /// <summary>
        /// The provider
        /// </summary>
        protected IPersistenceProvider Provider { get; private set; }

        /// <summary>
        /// Gets the prefix.
        /// </summary>
        /// <value>
        /// The prefix.
        /// </value>
        public virtual string Prefix => this.GetType().Name;

        /// <summary>
        /// Gets or sets a value indicating if this projection will save the reverse index
        /// </summary>
        public virtual bool ReverseEnabled => false;

        /// <summary>
        /// Gets the item specified by element key.
        /// </summary>
        /// <param name="element">The element who contains the key.</param>
        /// <returns></returns>
        protected T Get(T element)
        {
            return this.Get(element, null);
        }

        /// <summary>
        /// Gets the reverse entry of the element
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        protected T GetReverse(T element)
        {
            T reverseItem = ReverseItem(element);
            return this.Get(reverseItem);
        }

        /// <summary>
        /// Gets the by prefix.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        protected IEnumerable<T> GetByPrefix(T element)
        {
            if (element == null) return null;
            var fromDb = this.Provider.GetValuesByKeyPattern<T>(element.DocumentType);
            List<T> result = new List<T>();

            foreach (var item in fromDb)
            {
                // we need to check if the item is already deleted in this session.
                var lastValue = this.Get(item, item);
                if (lastValue != null)
                {
                    result.Add(lastValue);
                }
            }

            var fromContext = this.Work.GetProjectionItems(typeof(T)).Where(x => x.Value.Value.DocumentType == element.DocumentType);

            foreach (var item in fromContext)
            {
                if (item.Value.ProjectionItemAction == ProjectionItemActionEnum.Delete) continue;

                if (result.Contains(item.Value.Value)) continue;

                result.Add(item.Value.Value as T);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Gets the by prefix.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="startKey">The start key.</param>
        /// <param name="endKey">The end key.</param>
        /// <returns></returns>
        protected BatchQuery<T> GetByPrefix(T element, int limit, string startKey = null, string endKey = null)
        {
            if (element == null) return null;

            var fromDb = this.Provider.GetValuesByKeyPattern<T>(limit, startKey ?? element.DocumentType, endKey);
            List<T> result = new List<T>();

            foreach (var item in fromDb.Items)
            {
                // we need to check if the item is already deleted in this session.
                var lastValue = this.Get(item, item);
                if (lastValue != null)
                {
                    result.Add(lastValue);
                }
            }

            var fromContext = this.Work.GetProjectionItems(typeof(T));

            foreach (var item in fromContext)
            {
                if (item.Value.Value.DocumentType != element.DocumentType) continue;

                if (startKey != null && string.Compare(item.Value.Value.DocumentKey, startKey, StringComparison.OrdinalIgnoreCase) <= 0) continue;

                if (endKey != null && string.Compare(item.Value.Value.DocumentKey, endKey, StringComparison.OrdinalIgnoreCase) > 0) continue;

                if (item.Value.ProjectionItemAction == ProjectionItemActionEnum.Delete) continue;

                if (result.Contains(item.Value.Value)) continue;

                result.Add(item.Value.Value as T);
            }

            return new BatchQuery<T>
            {
                NextStartKey = fromDb.NextStartKey,
                EndKey = fromDb.EndKey,
                Items = result
            };
        }

        /// <summary>
        /// Gets the by reverse prefix.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        protected IEnumerable<T> GetByReversePrefix(T element)
        {
            T reverseItem = ReverseItem(element);
            return this.GetByPrefix(reverseItem);
        }

        /// <summary>
        /// Adds the or replace item.
        /// </summary>
        /// <param name="item">The item.</param>
        protected void AddOrReplaceItem(T item)
        {
            var newElement = new ProjectionItem<T>(item, ProjectionItemActionEnum.UpdateOrNew);
            this.Work.AddModifiedProjectionItem(newElement);

            if (this.ReverseEnabled)
            {
                T reverseItem = ReverseItem(item);

                var newReverseElement = new ProjectionItem<T>(reverseItem, ProjectionItemActionEnum.UpdateOrNew);
                this.Work.AddModifiedProjectionItem(newReverseElement);
            }
        }

        /// <summary>
        /// Remotes the item (and the reverse too)
        /// </summary>
        /// <param name="item">The item.</param>
        protected void RemoveItem(T item)
        {
            var newElement = new ProjectionItem<T>(item, ProjectionItemActionEnum.Delete);
            this.Work.AddModifiedProjectionItem(newElement);

            if (this.ReverseEnabled)
            {
                T reverseItem = ReverseItem(item);
                var newReverseElement = new ProjectionItem<T>(reverseItem, ProjectionItemActionEnum.Delete);
                this.Work.AddModifiedProjectionItem(newReverseElement);
            }
        }

        /// <summary>
        /// Consumes the specified incoming event.
        /// </summary>
        /// <param name="incomingEvent">The incoming event.</param>
        public void Consume(AggregateEvent incomingEvent)
        {
            Check.NotNull(() => incomingEvent);

            this.GetType()
                .GetMethod("ApplyGeneric")
                .MakeGenericMethod(incomingEvent.GetType())
                .Invoke(this, new object[] { incomingEvent });
        }

        /// <summary>
        /// Consumes the event. It redirect to the real implementation.
        /// </summary>
        /// <typeparam name="TEvent">An event</typeparam>
        /// <param name="incomingEvent">The incoming event.</param>
        public void ApplyGeneric<TEvent>(TEvent incomingEvent) where TEvent : AggregateEvent
        {
            var applier = this as IApplyEvent<TEvent>;
            if (applier != null)
            {
                // we can consume this event inside this query.
                applier.Apply(incomingEvent);
            }
        }

        /// <summary>
        /// Gets the item specified by element key.
        /// </summary>
        /// <param name="element">The element who contains the key.</param>
        /// <param name="fromDbItem">From database item.</param>
        /// <returns></returns>
        private T Get(T element, T fromDbItem)
        {
            if (element == null) return default(T);

            ProjectionItem<IPersistable> loaded;
            this.Work.GetProjectionItems(typeof(T)).TryGetValue(element.DocumentKey, out loaded);

            if (loaded != null)
            {
                if (loaded.ProjectionItemAction == ProjectionItemActionEnum.Delete)
                {
                    return null;
                }

                return loaded.Value as T;
            }

            T result;
            if (fromDbItem == null)
            {
                result = this.Provider.GetValue<T>(element.DocumentKey);
            }
            else
            {
                result = fromDbItem;
            }

            if (result != null)
            {
                this.Work.AddModifiedProjectionItem(new ProjectionItem<T>(result, ProjectionItemActionEnum.Nothing));
            }

            return result;
        }

        private T ReverseItem(T element)
        {
            if (this.ReverseEnabled == false) throw new LendsumException("The Reverse is disabled in this projection");
            var reverse = element as IReverseRow<T>;
            if (reverse == null) throw new LendsumException("The item doesn't implement the IReverse interface");
            var reverseItem = reverse.Reverse();
            return reverseItem;
        }
    }
}