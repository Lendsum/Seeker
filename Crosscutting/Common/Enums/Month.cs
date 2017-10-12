namespace Lendsum.Crosscutting.Common.Enums
{
    /// <summary>
    /// Enum to represent a month
    /// </summary>
    public enum Month
    {
        /// <summary>
        /// The not set
        /// </summary>
        NotSet = 0,

        /// <summary>
        /// The january
        /// </summary>
        January = 1,

        /// <summary>
        /// The february
        /// </summary>
        February = 2,

        /// <summary>
        /// The march
        /// </summary>
        March = 3,

        /// <summary>
        /// The april
        /// </summary>
        April = 4,

        /// <summary>
        /// The may
        /// </summary>
        May = 5,

        /// <summary>
        /// The june
        /// </summary>
        June = 6,

        /// <summary>
        /// The july
        /// </summary>
        July = 7,

        /// <summary>
        /// The august
        /// </summary>
        August = 8,

        /// <summary>
        /// The september
        /// </summary>
        September = 9,

        /// <summary>
        /// The october
        /// </summary>
        October = 10,

        /// <summary>
        /// The november
        /// </summary>
        November = 11,

        /// <summary>
        /// The december
        /// </summary>
        December = 12
    }

    /// <summary>
    /// Extensions methods of Month type
    /// </summary>
    public static class MonthExtensions
    {
        /// <summary>
        /// Determines whether the specified month has value.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <returns>True if has a value, false in otherwise</returns>
        public static bool HasValue(this Month month)
        {
            return month != Month.NotSet;
        }
    }
}