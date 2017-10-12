using System;
using System.Collections.Generic;

namespace Lendsum.Infrastructure.Core.DelayCalls
{
    /// <summary>
    /// Interface to be implemented by classes with method that will be called delayed.
    /// </summary>
    public interface IDelayed
    {
        /// <summary>
        /// Gets the name of the lock or empty/null if no lock is needed
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        string GetLockName(object data);

        /// <summary>
        /// Completes the delayed call.
        /// </summary>
        /// <param name="data">The data.</param>
        void CompleteCall(object data);

        /// <summary>
        /// Gets the type suported of this call.
        /// </summary>
        /// <value>
        /// The type suported.
        /// </value>
        IEnumerable<Type> TypesSupported { get; }
    }
}