namespace Lendsum.Crosscutting.Common.Enums
{
    /// <summary>
    /// Enum to represent the week days. We don't use System.Runtime.Interop.DayOfWeek
    /// because it doesnt have the NotSet value
    /// </summary>
    public enum WeekDay
    {
        /// <summary>
        /// The not set
        /// </summary>
        NotSet = 0,

        /// <summary>
        /// The monday
        /// </summary>
        Monday = 1,

        /// <summary>
        /// The tuesday
        /// </summary>
        Tuesday = 2,

        /// <summary>
        /// The wednesday
        /// </summary>
        Wednesday = 3,

        /// <summary>
        /// The thursday
        /// </summary>
        Thursday = 4,

        /// <summary>
        /// The friday
        /// </summary>
        Friday = 5,

        /// <summary>
        /// The saturday
        /// </summary>
        Saturday = 6,

        /// <summary>
        /// The sunday
        /// </summary>
        Sunday = 7,
    }

    /// <summary>
    /// Extensions methods of Weekday type
    /// </summary>
    public static class WeekDayExtensions
    {
        /// <summary>
        /// Determines whether the specified week day has value.
        /// </summary>
        /// <param name="weekDay">The week day.</param>
        /// <returns>True if has a value, false in otherwise</returns>
        public static bool HasValue(this WeekDay weekDay)
        {
            return weekDay != WeekDay.NotSet;
        }
    }
}