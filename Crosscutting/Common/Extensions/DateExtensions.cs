using System;
using System.Collections.Generic;
using System.Linq;

namespace Lendsum.Crosscutting.Common.Extensions
{
    /// <summary>
    ///
    /// </summary>
    public static class DateExtensions
    {
        /// <summary>
        /// To the short date string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string ToShortDateString(this Date value)
        {
            Check.NotNull(() => value);

            return value.DateTime.ToShortDateString();
        }

        /// <summary>
        /// To the date.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Date ToDate(this DateTime value)
        {
            if (value.TimeOfDay != new TimeSpan(0)) return null;

            return new Date(value.Year, value.Month, value.Day);
        }

        /// <summary>
        /// Adds the days.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="days">The days.</param>
        /// <returns></returns>
        public static Date AddDays(this Date value, int days)
        {
            Check.NotNull(() => value);

            var properDate = value.DateTime;
            properDate = properDate.AddDays(days);

            return properDate.ToDate();
        }

        /// <summary>
        /// Adds the months.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="months">The months.</param>
        /// <returns></returns>
        public static Date AddMonths(this Date value, int months)
        {
            Check.NotNull(() => value);

            var properDate = value.DateTime;
            properDate = properDate.AddMonths(months);

            return properDate.ToDate();
        }

        /// <summary>
        /// Advances to last day of the month or day specified, what happen first.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="day">The day.</param>
        /// <returns></returns>
        public static Date AdvanceToDayOrLastDayOfMonth(this Date value, int day)
        {
            Check.NotNull(() => value);

            var properDate = value.DateTime;

            return properDate.AdvanceToDayOrLastDayOfMonth(day).ToDate();
        }

        /// <summary>
        /// Advances to last day of the month or day specified, what happen first.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Date AdvanceToLastDayOftheMonth(this Date value)
        {
            Check.NotNull(() => value);

            var properDate = value.DateTime;

            return properDate.AdvanceToLastDayOftheMonth().ToDate();
        }

        /// <summary>
        /// Determines whether [is included in or sunday] [the specified date time list].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dateList">The date time list.</param>
        /// <returns></returns>
        public static bool IsDateIncludedInOrWeekend(this Date value, IEnumerable<Date> dateList = null)
        {
            Check.NotNull(() => value);

            var properDate = value.DateTime;
            var dateTimeList = dateList.Select(x => x.DateTime);

            return properDate.IsDateIncludedInOrWeekend(dateTimeList);
        }

        /// <summary>
        /// Determines whether [has day plus interval been reached] [the specified interval in days].
        /// </summary>
        /// <param name="init">The initialize.</param>
        /// <param name="intervalInDays">The interval in days.</param>
        /// <param name="now">The now.</param>
        /// <returns></returns>
        public static bool HasDayPlusIntervalBeenReached(this Date init, int intervalInDays, Date now)
        {
            return init.AddDays(intervalInDays) <= now;
        }
    }
}