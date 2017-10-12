using System;

namespace Lendsum.Crosscutting.Common.Providers
{
    /// <summary>
    /// Class to wrapp DateTime methods to obtain the current datetime.
    /// </summary>
    public class DateTimeProvider : IDateTimeProvider
    {
        /// <inheritdoc/>
        public DateTime UtcNow
        {
            get
            {
                return DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Gets the UTC now ticks.
        /// </summary>
        /// <value>
        /// The UTC now ticks.
        /// </value>
        public long UtcNowTicks
        {
            get
            {
                return DateTime.UtcNow.Ticks;
            }
        }
    }
}