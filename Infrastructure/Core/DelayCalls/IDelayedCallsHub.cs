using System;

namespace Lendsum.Infrastructure.Core.DelayCalls
{
    /// <summary>
    ///
    /// </summary>
    public interface IDelayedCallsHub : IDisposable
    {
        /// <summary>
        /// Starts the processing of the delayed calls.
        /// </summary>
        void StartProcessing();
    }
}