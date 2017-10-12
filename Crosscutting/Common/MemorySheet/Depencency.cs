using System;
using System.Collections.Generic;
using System.Linq;

namespace Lendsum.Crosscutting.Common.MemorySheet
{
    /// <summary>
    ///
    /// </summary>
    public class Depencency
    {
        private object value;
        private List<Position> cellsDependsOnThisValue = new List<Position>();

        /// <summary>
        /// Gets or sets the cells depends on this value.
        /// </summary>
        /// <value>
        /// The cells depends on this value.
        /// </value>
        public IEnumerable<Position> CellsDependsOnThisValue => this.cellsDependsOnThisValue;

        /// <summary>
        /// Marks the dependency.
        /// </summary>
        /// <param name="position">The position.</param>
        public void MarkDependency(Position position)
        {
            if (!cellsDependsOnThisValue.Any(x => x.Row == position.Row && x.Column == position.Column))
            {
                this.cellsDependsOnThisValue.Add(position);
            }
        }

        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        /// <value>
        /// The current value.
        /// </value>
        public object CurrentValue
        {
            get
            {
                if (!Available) throw new InvalidOperationException("The value cannot be obtained because is not setted");
                return this.value;
            }
            set
            {
                this.value = value;
                this.Available = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Depencency"/> is available.
        /// </summary>
        /// <value>
        ///   <c>true</c> if available; otherwise, <c>false</c>.
        /// </value>
        public bool Available { get; set; }

        /// <summary>
        /// Dismisses this instance.
        /// </summary>
        public void Dismiss()
        {
            this.Available = false;
            this.value = null;
        }
    }
}