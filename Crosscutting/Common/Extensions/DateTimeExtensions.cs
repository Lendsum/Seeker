using Lendsum.Crosscutting.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lendsum.Crosscutting.Common.Extensions
{
    /// <summary>
    /// Extensions methods for date time
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gets the week day.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static WeekDay GetWeekDay(this DateTime value)
        {
            var result = (int)value.DayOfWeek;

            // sunday in DayOfWeek zero but in WeekDay is 7
            if (result == 0) { result = 7; }

            return (WeekDay)result;
        }

        /// <summary>
        /// Advances to year.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        public static DateTime AdvanceToYear(this DateTime value, int year)
        {
            if (year < value.Year)
            {
                throw new ArgumentException("The year must be equal or greater than the value year", "year");
            }

            var result = value;
            int offset = year - value.Year;
            if (offset > 0)
            {
                result = result.AddYears(offset);
            }

            return result;
        }

        /// <summary>
        /// Move in time this value to the month
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="month">The next month</param>
        /// <returns>
        /// The DateTime
        /// </returns>
        /// <exception cref="System.ArgumentException">The month has to be set;month</exception>
        public static DateTime AdvanceToMonth(this DateTime value, Month month)
        {
            if (!month.HasValue())
            {
                throw new ArgumentException("The month has to be set", "month");
            }

            var result = value;
            int target = (int)month;
            if (target < value.Month)
            {
                result = result.AdvanceToYear(result.Year + 1);
            }

            int offSet = target - result.Month;
            if (offSet != 0)
            {
                result = result.AddMonths(offSet);
            }

            return result;
        }

        /// <summary>
        /// Advances to week day.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="weekDay">The week day.</param>
        /// <returns></returns>
        public static DateTime AdvanceToWeekDay(this DateTime value, WeekDay weekDay)
        {
            if (!weekDay.HasValue())
            {
                throw new ArgumentException("The weekDay has to be set", "weekDay");
            }

            int target = (int)weekDay;
            int from = (int)value.GetWeekDay();
            int offSet = target - from;

            // check if we have to jump the week
            if (offSet < 0) { offSet = offSet + 7; }
            var result = value.AddDays(offSet);
            return result;
        }

        /// <summary>
        /// Advances to day.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="day">The day.</param>
        /// <returns></returns>
        public static DateTime AdvanceToDay(this DateTime value, int day)
        {
            Check.Range(() => day, 1, 31);

            var result = value;
            if (day < value.Day)
            {
                result = result.AddMonths(1);
            }

            var offset = day - result.Day;
            if (offset != 0)
            {
                result = result.AddDays(offset);
            }

            return result;
        }

        /// <summary>
        /// Advances to hour.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="hour">The hour.</param>
        /// <returns></returns>
        public static DateTime AdvanceToHour(this DateTime value, int hour)
        {
            Check.Range(() => hour, 0, 23);

            var result = value;
            if (hour < value.Hour)
            {
                result = result.AddDays(1);
            }

            var offset = hour - value.Hour;
            if (offset != 0)
            {
                result = result.AddHours(offset);
            }

            return result;
        }

        /// <summary>
        /// Advances to hour.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="minute">The minute.</param>
        /// <returns></returns>
        public static DateTime AdvanceToMinute(this DateTime value, int minute)
        {
            Check.Range(() => minute, 0, 59);

            var result = value;
            if (minute < value.Minute)
            {
                result = result.AddHours(1);
            }

            var offset = minute - value.Minute;
            if (offset != 0)
            {
                result = result.AddMinutes(offset);
            }

            return result;
        }

        /// <summary>
        /// Advances to hour.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="second">The second.</param>
        /// <returns></returns>
        public static DateTime AdvanceToSecond(this DateTime value, int second)
        {
            Check.Range(() => second, 0, 59);

            var result = value;
            if (second < value.Second)
            {
                result = result.AddMinutes(1);
            }

            var offset = second - value.Second;
            if (offset != 0)
            {
                result = result.AddSeconds(offset);
            }

            return result;
        }

        /// <summary>
        /// Advances to last day ofthe month.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static DateTime AdvanceToLastDayOftheMonth(this DateTime value)
        {
            DateTime nextMonth;
            if (value.Day == 1)
            {
                nextMonth = value.AddMonths(1);
            }
            else
            {
                nextMonth = value.AdvanceToDay(1);
            }

            return nextMonth.AddDays(-1);
        }

        /// <summary>
        /// Advances to last day of the month or day specified, what happen first.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="day">The day.</param>
        /// <returns></returns>
        public static DateTime AdvanceToDayOrLastDayOfMonth(this DateTime value, int day)
        {
            if (value.Day == day) return value;
            else if (value.Day < day) return value.AdvanceToDay(day);
            else return value.AdvanceToLastDayOftheMonth();
        }

        /// <summary>
        /// Determines whether [is included in or sunday] [the specified date time list].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dateTimeList">The date time list.</param>
        /// <returns></returns>
        public static bool IsDateIncludedInOrWeekend(this DateTime value, IEnumerable<DateTime> dateTimeList = null)
        {
            if (value.DayOfWeek == DayOfWeek.Sunday) return true;
            if (value.DayOfWeek == DayOfWeek.Saturday) return true;
            if (dateTimeList == null) return false;
            return dateTimeList.Select(x => x.Date).Contains(value.Date);
        }

        /// <summary>
        /// Determines whether [has day plus interval been reached] [the specified interval in days].
        /// </summary>
        /// <param name="init">The initialize.</param>
        /// <param name="intervalInDays">The interval in days.</param>
        /// <param name="now">The now.</param>
        /// <returns></returns>
        public static bool HasDayPlusIntervalBeenReached(this DateTime init, int intervalInDays, DateTime now)
        {
            return init.Date.AddDays(intervalInDays) <= now.Date;
        }
    }
}