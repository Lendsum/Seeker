using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeekerExample.Dtos
{
	/// <summary>
    /// Scenario item like player, target, block...
    /// </summary>
    public class ScenarioItem
    {
		public int X { get; set; }
		public int Y { get; set; }
		public ScenarioItemType ItemType { get; set; }

        public ScenarioItem MoveTo(DirectionEnum direction)
        {
            ScenarioItem result = new ScenarioItem() { X = this.X, Y = this.Y, ItemType = this.ItemType };

            switch (direction)
            {
                case DirectionEnum.Down:
                    result.Y = result.Y + 1;
                    break;
                case DirectionEnum.Up:
                    result.Y = result.Y - 1;
                    break;
                case DirectionEnum.Left:
                    result.X = result.X - 1;
                    break;
                case DirectionEnum.Right:
                    result.X = result.X + 1;
                    break;
            }

            return result;
        }
    }
}
