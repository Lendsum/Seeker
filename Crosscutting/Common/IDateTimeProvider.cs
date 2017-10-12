using System;

namespace Lendsum.Crosscutting.Common
{
    /// <summary>
    /// Interface to be implemented by DateTimeProvider
    /// It to wrapp DateTime structure and static methods like UtcNow. It can facilitate the mocking of those methods
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// Gets the current date in UTC.
        /// </summary>
        /// <returns>DateTime with the current date in UTC format</returns>
        DateTime UtcNow { get; }

        /// <summary>
        /// Gets the UTC now ticks.
        /// </summary>
        /// <value>
        /// The UTC now ticks.
        /// </value>
        long UtcNowTicks { get; }
    }
}