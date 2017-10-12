using Lendsum.Infrastructure.Core.Projections;
using System;
using System.Globalization;

namespace Lendsum.Infrastructure.Core.Scheduler
{
    /// <summary>
    /// Item to wake up schedulers
    /// </summary>
    public class WakeUpRow : IPersistable, IReverseRow<WakeUpRow>
    {
        /// <summary>
        /// Gets the reverse version of the item T
        /// </summary>
        /// <value>
        /// The reverse.
        /// </value>
        public bool ReverseRow { get; set; }

        /// <summary>
        /// Gets or sets the aggregate uid.
        /// </summary>
        /// <value>
        /// The aggregate uid.
        /// </value>
        public Guid AggregateUid { get; set; }

        /// <summary>
        /// Gets or sets the date range start.
        /// </summary>
        /// <value>
        /// The date range start.
        /// </value>
        public long DateRangeStart { get; set; }

        /// <summary>
        /// Gets or sets the name of the scheduler.
        /// </summary>
        /// <value>
        /// The name of the scheduler.
        /// </value>
        public string SchedulerName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="WakeUpRow"/> is processing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if processing; otherwise, <c>false</c>.
        /// </value>
        public bool Processing { get; set; } = false;

        /// <summary>
        /// Gets or sets the lock initialize date.
        /// </summary>
        /// <value>
        /// The lock initialize date.
        /// </value>
        public DateTime LockInitDate { get; set; }

        /// <summary>
        /// Gets or sets the lock end date.
        /// </summary>
        /// <value>
        /// The lock end date.
        /// </value>
        public DateTime LockEndDate { get; set; }

        /// <summary>
        /// Gets or sets the cas. This value must be used only by persistance layer.
        /// </summary>
        /// <value>
        /// The cas.
        /// </value>
        public ulong Cas { get; set; }

        /// <summary>
        /// Gets the document key.
        /// </summary>
        /// <value>
        /// The document key.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual string DocumentKey
        {
            get
            {
                return this.DocumentType + ":" + this.AggregateUid;
            }
        }

        /// <summary>
        /// Gets the type of the document.
        /// </summary>
        /// <value>
        /// The type of the document.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual string DocumentType
        {
            get
            {
                if (this.ReverseRow)
                {
                    return this.StartKey;
                }
                else
                {
                    return this.StartKey + ":" + this.DateRangeStart.ToString("D36", CultureInfo.InvariantCulture);
                }
            }
        }

        /// <summary>
        /// Gets the start key.
        /// </summary>
        /// <value>
        /// The start key.
        /// </value>
        public string StartKey
        {
            get
            {
                return "WakeUpRow:" + this.SchedulerName;
            }
        }

        /// <summary>
        /// Gets the reverse version of the item T
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <value>
        /// The reverse.
        /// </value>
        public WakeUpRow Reverse()
        {
            WakeUpRow result = new WakeUpRow();
            result.ReverseRow = !this.ReverseRow;
            result.AggregateUid = this.AggregateUid;
            result.DateRangeStart = this.DateRangeStart;
            result.SchedulerName = this.SchedulerName;
            return result;
        }
    }
}