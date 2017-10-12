using Lendsum.Infrastructure.Core;
using SeekerBasic.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeekerBasic.Domain.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class AddedItem : AggregateEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddedItem" /> class.
        /// </summary>
        public AddedItem() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddedItem" /> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public AddedItem(ScenarioItem item) : base()
        {
            this.Items = new ScenarioItem[] { item };
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IEnumerable<ScenarioItem> Items { get; set; }
    }
}
