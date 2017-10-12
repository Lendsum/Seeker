using Lendsum.Crosscutting.Common;
using Lendsum.Infrastructure.Core.Locks;
using Lendsum.Infrastructure.Core.Persistence;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lendsum.Infrastructure.Core.Scheduler
{
    /// <summary>
    /// launcher of IRecurrent jobs.
    /// </summary>
    public class SchedulerLauncher : ISchedulerLauncher
    {
        private IEnumerable<IRecurrentAlarm> jobs;

        private IPersistenceProvider provider;
        private IUnitOfWork unitOfWork;
        private ILocker locker;

        /// <summary>
        /// Gets or sets the date time provider.
        /// </summary>
        /// <value>
        /// The date time provider.
        /// </value>
        public IDateTimeProvider DateTimeProvider { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger log { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerLauncher" /> class.
        /// </summary>
        /// <param name="jobs">The jobs.</param>
        /// <param name="dateTimeProvider">The date time provider.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="locker">The locker.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public SchedulerLauncher(IEnumerable<IRecurrentAlarm> jobs,
            IDateTimeProvider dateTimeProvider,
            IPersistenceProvider provider,
            ILocker locker,
            IUnitOfWork unitOfWork,
            ILogger logger)
        {
            this.DateTimeProvider = Check.NotNull(() => dateTimeProvider);
            this.log = Check.NotNull(() => logger);
            this.provider = Check.NotNull(() => provider);
            this.locker = Check.NotNull(() => locker);
            this.unitOfWork = Check.NotNull(() => unitOfWork);
            this.jobs = Check.NotNull(() => jobs).GroupBy(x => x.GetType())
                .Select(group => group.First());
        }

        /// <summary>
        /// Reads this instance.
        /// </summary>
        public void Go()
        {
            log.LogDebug("Scheduler reader");

            foreach (var job in this.jobs)
            {
                ProcessIfNeeded(job);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "its the front-end method, so we cannot raise any exception")]
        private void ProcessIfNeeded(IRecurrentAlarm job)
        {
            try
            {
                log.LogDebug(S.Invariant($"Processing job {job.JobName}"));

                var jobRow = new WakeUpRow
                {
                    SchedulerName = job.JobName,
                    DateRangeStart = this.DateTimeProvider.UtcNowTicks
                };

                var jobRows = this.provider.GetValuesByKeyPattern<WakeUpRow>(100, jobRow.StartKey, jobRow.DocumentType).Items.Where(x => !x.ReverseRow);

                foreach (var row in jobRows)
                {
                    if (row.AggregateUid != new Guid())
                    {
                        if (this.locker.TryLockKey(row.AggregateUid.ToString(), 300) == null)
                        {
                            this.log.LogDebug(S.Invariant($"Aggregate {row.AggregateUid} is locked so the scheduler cannot procces it right now"));
                            continue;
                        }
                    }
                    else
                    {
                        if (this.TryLockItem(row, 300) == null)
                        {
                            this.log.LogDebug(S.Invariant($"Row {row.DocumentKey} is blocked so the asynchub cannot procces it right now"));
                            continue;
                        }
                    }

                    try
                    {
                        using (var context = new ThreadContextContainer())
                        {
                            log.LogDebug(S.Invariant($"Processing row {row.DocumentKey} for job {job.JobName}"));

                            job.Process(row);

                            this.unitOfWork.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        this.log.LogError(                             
                               ex,
                               S.Invariant($"Error processing row {row.DocumentKey}"));
                    }

                    try
                    {
                        this.ReleaseItem(row);
                        this.locker.ReleaseKey(row.AggregateUid.ToString());
                    }
                    catch (Exception ex)
                    {
                        this.log.LogError(
                              ex,
                              S.Invariant($"Error releasing row {row.DocumentKey}"));
                    }
                }
            }
            catch (Exception ex)
            {
                this.log.LogError(
                        ex,
                        S.Invariant($"Uncontrolled Exception"));
            }
        }

        private WakeUpRow TryLockItem(WakeUpRow item, int maxExecutingInSeconds)
        {
            if (item.Processing == true && item.LockInitDate.AddSeconds(maxExecutingInSeconds) > DateTimeProvider.UtcNow) return null;

            item.LockInitDate = this.DateTimeProvider.UtcNow;
            item.Processing = true;

            var insertResult = this.provider.UpdateOrInsert(item);
            if (insertResult != PersistenceResultEnum.Success) return null;

            return item;
        }

        /// <summary>
        /// Releases the key.
        /// </summary>
        /// <param name="item">The item.</param>
        public void ReleaseItem(WakeUpRow item)
        {
            Check.NotNull(() => item);

            item = this.provider.GetValue<WakeUpRow>(item.DocumentKey);

            if (item == null) return;

            item.Processing = false;
            item.LockEndDate = this.DateTimeProvider.UtcNow;

            provider.UpdateOrInsert(item);
        }
    }
}