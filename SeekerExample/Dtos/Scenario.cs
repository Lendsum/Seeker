using System;
using System.Collections.Generic;
using System.Text;

namespace SeekerExample.Dtos
{
    class Scenario
    {
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IEnumerable<ScenarioItem> Items { get; set; }

        /// <summary>
        /// Gets the player.
        /// </summary>
        /// <value>
        /// The player.
        /// </value>
        public ScenarioItem Player { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets the movements.
        /// </summary>
        /// <value>
        /// The movements.
        /// </value>
        public int Movements { get; set; }

    }
}
