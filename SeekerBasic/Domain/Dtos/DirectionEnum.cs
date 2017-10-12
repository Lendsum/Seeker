using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeekerBasic.Domain.Dtos
{
    /// <summary>
    /// Posible directions of player movement.
    /// </summary>
    public enum DirectionEnum
    {
        /// <summary>
        /// The not set
        /// </summary>
        NotSet = 0,

        /// <summary>
        /// Y=Y-1
        /// </summary>
        Up = 10,

        /// <summary>
        /// Y=Y+1
        /// </summary>
        Down = 20,

        /// <summary>
        /// X=X-1
        /// </summary>
        Left = 30,

        /// <summary>
        /// X=X+1
        /// </summary>
        Right = 40
    }
}
