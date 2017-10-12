using Lendsum.Infrastructure.Core;
using SeekerBasic.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeekerBasic.Domain.Events
{
    public class PlayerMoved : AggregateEvent
    {
        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>
        /// The direction.
        /// </value>
        public DirectionEnum Direction { get; set; }
    }
}
