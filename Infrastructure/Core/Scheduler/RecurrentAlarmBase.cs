using Lendsum.Infrastructure.Core.Persistence;
using Lendsum.Infrastructure.Core.Projections;

namespace Lendsum.Infrastructure.Core.Scheduler
{
    /// <summary>
    ///
    /// </summary>
    public abstract class RecurrentAlarmBase<TRow> : ProjectionBase<TRow>, IRecurrentAlarm where TRow : WakeUpRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecurrentAlarmBase{TRow}" /> class.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="work"></param>
        protected RecurrentAlarmBase(IPersistenceProvider provider, IUnitOfWork work) : base(provider, work)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating if this projection will save the reverse index
        /// </summary>
        public override bool ReverseEnabled => true;

        /// <summary>
        /// Gets the name of the job.
        /// </summary>
        /// <value>
        /// The name of the job.
        /// </value>
        public abstract string JobName { get; }

        /// <summary>
        /// Does the job if needed.
        /// </summary>
        /// <param name="wakeUpRow">The wake up event.</param>
        public abstract void DoJobIfNeeded(TRow wakeUpRow);

        /// <summary>
        /// Dispatches the specified event.
        /// </summary>
        /// <param name="incomingRow">The incoming row.</param>
        public void Process(WakeUpRow incomingRow)
        {
            var wakeUpEvent = incomingRow as TRow;
            if (wakeUpEvent != null) this.DoJobIfNeeded(wakeUpEvent);
        }
    }
}