using System;

namespace Lendsum.Crosscutting.Common
{
    /// <summary>
    /// Item of NumberOfItemFixedList
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NumberOfItemFixedListItem<T>
    {
        /// <summary>
        /// Gets or sets the date when the item was added.
        /// </summary>
        /// <value>
        /// The when.
        /// </value>
        public DateTime When { get; set; }

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        public T Item { get; set; }
    }
}