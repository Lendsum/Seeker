namespace Lendsum.Infrastructure.Core.Scheduler
{
    /// <summary>
    /// interface to be implemented by recurrent job.
    /// </summary>
    public interface IRecurrentAlarm
    {
        /// <summary>
        /// Gets the name of the job.
        /// </summary>
        /// <value>
        /// The name of the job.
        /// </value>
        string JobName { get; }

        /// <summary>
        /// Process the row if needed.
        /// </summary>
        /// <param name="incomingRow">The incoming row.</param>
        void Process(WakeUpRow incomingRow);

        /// <summary>
        /// Consumes the specified incomingevent.
        /// </summary>
        /// <param name="incomingevent">The incomingevent.</param>
        void Consume(AggregateEvent incomingevent);
    }
}