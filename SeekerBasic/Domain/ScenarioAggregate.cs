using Lendsum.Crosscutting.Common;
using Lendsum.Infrastructure.Core;
using SeekerBasic.Domain.Dtos;
using SeekerBasic.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeekerBasic.Domain
{
    public class ScenarioAggregate : Aggregate, 
        IApplyEvent<AddedItem>, 
        IApplyEvent<PlayerMoved>, 
        IApplyEvent<ScenarioReset>
    {
        List<ScenarioItem> items=new List<ScenarioItem>();
        private List<ScenarioItem> itemsBackup;

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IEnumerable<ScenarioItem> Items
        {
            get { return this.items.ToArray(); }
            set { this.items = new List<ScenarioItem>(value); }
        }

        private void SaveInitialScenario()
        {
            this.itemsBackup = new List<ScenarioItem>(this.items);
        }

        private void RemoveTargetIfNeeded(ScenarioItem player)
        {
            var currentItem = this.GetByCoord(player.X, player.Y);
            if (currentItem != null)
            {
                if (currentItem.ItemType == ScenarioItemType.Target)
                {
                    this.items.Remove(currentItem);
                    this.Score++;
                }
            }
        }

        /// <summary>
        /// Determines if in this game the player can go to that direction because is not blocked
        /// </summary>
        /// <param name="direction">The direction.</param>
        public bool CanPlayerGo(DirectionEnum direction)
        {
            if (this.Player == null) throw new SeekerException("The player is not defined");
            var newPosition = this.Player.MoveTo(direction);

            var block = this.items.Where(item => item.X == newPosition.X && item.Y == newPosition.Y && item.ItemType == ScenarioItemType.Block).FirstOrDefault();
            if (block == null) return true;
            else return false;
        }

        /// <summary>
        /// Gets the by coord.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public ScenarioItem GetByCoord(int x, int y)
        {
            return items.Where(item => item.X == x && item.Y == y).FirstOrDefault();
        }

        public void Apply(AddedItem incomingEvent)
        {
            foreach (var item in incomingEvent.Items)
            {
                if (this.GetByCoord(item.X, item.Y) != null)
                {
                    throw new SeekerException(S.Invariant($"There is already a item in position {item.X}-{item.Y} "));
                }

                this.items.Add(item);
            }
        }

        public void Apply(PlayerMoved incomingEvent)
        {
            if (!this.CanPlayerGo(incomingEvent.Direction)) { throw new SeekerException("the player cannot go there"); }

            if (this.Movements == 0) this.SaveInitialScenario();

            var newPlayerPosition = this.Player.MoveTo(incomingEvent.Direction);
            this.RemoveTargetIfNeeded(newPlayerPosition);

            this.items.Remove(this.Player);
            this.items.Add(newPlayerPosition);
            this.Movements++;
        }

        public void Apply(ScenarioReset incomingEvent)
        {
            this.items = new List<ScenarioItem>(this.itemsBackup);
        }

        /// <summary>
        /// Gets the player.
        /// </summary>
        /// <value>
        /// The player.
        /// </value>
        public ScenarioItem Player => this.items.Where(x => x.ItemType == ScenarioItemType.Player).FirstOrDefault();

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
