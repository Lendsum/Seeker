using Lendsum.Crosscutting.Common;
using Lendsum.Infrastructure.Core.Dispatcher;
using System.Collections.Generic;
using System.Linq;

namespace Lendsum.Infrastructure.Core.Scheduler
{
    /// <summary>
    /// launcher of IRecurrent jobs.
    /// </summary>
    public class SchedulerListener : IAsyncListener
    {
        private IEnumerable<IRecurrentAlarm> jobs;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerLauncher" /> class.
        /// </summary>
        /// <param name="jobs">The jobs.</param>
        public SchedulerListener(IEnumerable<IRecurrentAlarm> jobs)
        {
            this.jobs = Check.NotNull(() => jobs).GroupBy(x => x.GetType())
                .Select(group => group.First());
        }

        /// <summary>
        /// Dispatches the specified event.
        /// </summary>
        /// <param name="incomingEvent">The event.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Consume(AggregateEvent incomingEvent)
        {
            foreach (var job in this.jobs)
            {
                job.Consume(incomingEvent);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IListener" /> is rebuildable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if rebuildable; otherwise, <c>false</c>.
        /// </value>
        public bool Rebuildable => true;

        /// <summary>
        /// Gets a value indicating whether [consume only last event].
        /// </summary>
        /// <value>
        /// <c>true</c> if [consume only last event]; otherwise, <c>false</c>.
        /// </value>
        public bool ConsumeOnlyLastEvent => false;
    }
}