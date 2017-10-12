using Lendsum.Crosscutting.Common;
using Lendsum.Infrastructure.Core.Persistence;

namespace Lendsum.Infrastructure.Core.Locks
{
    /// <summary>
    /// Class to lock an aggregate and prevents that no more events been dispached asyncronly
    /// </summary>
    public class Locker : ILocker
    {
        private IPersistenceProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Locker" /> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public Locker(IPersistenceProvider provider)
        {
            this.provider = Check.NotNull(() => provider);
        }

        /// <summary>
        /// Tries to lock the key to prevent nobody can use it.
        /// </summary>
        /// <param name="key">The item.</param>
        /// <param name="maxExecutingInSeconds">The maximum executing in seconds. If the locker is taken more time than this parameter, it is considered released</param>
        /// <returns></returns>
        public LockerInfo TryLockKey(string key, int maxExecutingInSeconds)
        {
            LockerInfo info = provider.GetLock(key, maxExecutingInSeconds);

            return info;
        }

        /// <summary>
        /// Releases the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public void ReleaseKey(string key)
        {
            provider.ReleaseLock(key);
        }
    }
}