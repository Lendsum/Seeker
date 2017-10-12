using System;
using System.Collections.Generic;

namespace Lendsum.Infrastructure.Core.Persistence
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BatchQuery<T>
    {
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Gets or sets the start key.
        /// </summary>
        /// <value>
        /// The start key.
        /// </value>
        public string NextStartKey { get; set; }

        /// <summary>
        /// Gets or sets the next start key unique identifier.
        /// </summary>
        /// <value>
        /// The next start key unique identifier.
        /// </value>
        public Guid NextStartKeyGuid { get; set; }

        /// <summary>
        /// Gets or sets the end key.
        /// </summary>
        /// <value>
        /// The end key.
        /// </value>
        public string EndKey { get; set; }
    }
}