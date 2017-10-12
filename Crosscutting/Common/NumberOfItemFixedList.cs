using System;
using System.Collections.Generic;

namespace Lendsum.Crosscutting.Common
{
    /// <summary>
    /// class where can only store a number of item. The oldest item will be replaced.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NumberOfItemFixedList<T>
    {
        private Queue<NumberOfItemFixedListItem<T>> queue;

        /// <summary>
        /// Gets the maximum number of item.
        /// </summary>
        /// <value>
        /// The maximum number of item.
        /// </value>
        public int MaxNumberOfItem { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberOfItemFixedList{T}"/> class.
        /// </summary>
        /// <param name="maxItems">The maximum items.</param>
        public NumberOfItemFixedList(int maxItems)
        {
            this.MaxNumberOfItem = maxItems;
            this.queue = new Queue<NumberOfItemFixedListItem<T>>(this.MaxNumberOfItem + 10);
        }

        /// <summary>
        /// Adds the specified item to the colleciton, the older one will be replaced.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(T item)
        {
            this.queue.Enqueue(new NumberOfItemFixedListItem<T>() { When = DateTime.UtcNow, Item = item });

            while (this.queue.Count > this.MaxNumberOfItem)
            {
                this.queue.Dequeue();
            }
        }

        /// <summary>
        /// Gets all items.
        /// </summary>
        /// <value>
        /// All items.
        /// </value>
        public IEnumerable<NumberOfItemFixedListItem<T>> AllItems
        {
            get
            {
                return this.queue.ToArray();
            }
        }
    }
}