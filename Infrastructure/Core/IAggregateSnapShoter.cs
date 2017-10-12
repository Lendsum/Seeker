using System;

namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Interface of the snap shoter
    /// </summary>
    public interface IAggregateSnapShoter
    {
        /// <summary>
        /// Makes the snapshot if needed for the aggregate uid given.
        /// </summary>
        /// <param name="aggregateUid">The aggregate uid.</param>
        void SnapShotIfNeeded(Guid aggregateUid);
    }
}