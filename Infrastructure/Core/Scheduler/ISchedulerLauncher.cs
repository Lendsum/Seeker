namespace Lendsum.Infrastructure.Core.Scheduler
{
    /// <summary>
    /// Launcher interface
    /// </summary>
    public interface ISchedulerLauncher
    {
        /// <summary>
        /// Execute this scheduler. Method to called periodically.
        /// </summary>
        void Go();
    }
}