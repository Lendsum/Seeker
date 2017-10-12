using Lendsum.Crosscutting.Common.Extensions;
using System;
using System.Linq;

namespace Lendsum.Crosscutting.Common
{
    /// <summary>
    ///
    /// </summary>
    public class Date : IComparable<Date>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Date"/> class.
        /// </summary>
        public Date()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Date"/> class.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        public Date(int year, int month, int day)
        {
            this.dateTime = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Unspecified);

            this.Year = year;
            this.Month = month;
            this.Day = day;
        }

        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        /// <value>
        /// The year.
        /// </value>
        public int Year
        {
            get { return year; }
            set
            {
                if (value < 1 || value > 9999) throw new ArgumentOutOfRangeException("Date.Year", S.Invariant($"{value} is not a valid year"));

                if (this.month != 0 && this.day != 0)
                {
                    this.dateTime = new DateTime(value, this.month, this.day, 0, 0, 0, DateTimeKind.Unspecified);
                }
                this.year = value;
            }
        }

        private int year;

        /// <summary>
        /// Gets or sets the month.
        /// </summary>
        /// <value>
        /// The month.
        /// </value>
        public int Month
        {
            get { return month; }
            set
            {
                if (value < 1 || value > 12) throw new ArgumentOutOfRangeException("Date.Month", S.Invariant($"{value} is not a valid month"));

                if (this.year != 0 && this.day != 0)
                {
                    this.dateTime = new DateTime(this.year, value, this.day, 0, 0, 0, DateTimeKind.Unspecified);
                }
                this.month = value;
            }
        }

        private int month;

        /// <summary>
        /// Gets or sets the day.
        /// </summary>
        /// <value>
        /// The day.
        /// </value>
        public int Day
        {
            get { return day; }
            set
            {
                if (value < 1 || value > 31) throw new ArgumentOutOfRangeException("Date.Day", S.Invariant($"{value} is not a valid day"));

                if (this.year != 0 && this.month != 0)
                {
                    this.dateTime = new DateTime(this.year, this.month, value, 0, 0, 0, DateTimeKind.Unspecified);
                }
                this.day = value;
            }
        }

        private int day;

        /// <summary>
        /// Gets the date time.
        /// </summary>
        /// <value>
        /// The date time.
        /// </value>
        public DateTime DateTime => this.dateTime;

        private DateTime dateTime;

        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static bool TryParse(string input, out Date result)
        {
            Check.NotNull(() => input);

            result = null;

            if (string.IsNullOrEmpty(input)) return false;
            if (string.IsNullOrWhiteSpace(input)) return false;

            if (input.IndexOf("Day", 0, StringComparison.CurrentCultureIgnoreCase) == -1) return false;
            if (input.IndexOf("Month", 0, StringComparison.CurrentCultureIgnoreCase) == -1) return false;
            if (input.IndexOf("Year", 0, StringComparison.CurrentCultureIgnoreCase) == -1) return false;

            var inputSplit = input.Split(',');
            var dayEntryInSplit = inputSplit.FirstOrDefault(x => x.IndexOf("Day", 0, StringComparison.CurrentCultureIgnoreCase) > 0);
            var monthEntryInSplit = inputSplit.FirstOrDefault(x => x.IndexOf("Month", 0, StringComparison.CurrentCultureIgnoreCase) > 0);
            var yearEntryInSplit = inputSplit.FirstOrDefault(x => x.IndexOf("Year", 0, StringComparison.CurrentCultureIgnoreCase) > 0);

            var day = dayEntryInSplit.GetNumbers().ToInt();
            var month = monthEntryInSplit.GetNumbers().ToInt();
            var year = yearEntryInSplit.GetNumbers().ToInt();

            if (day == null) return false;
            if (month == null) return false;
            if (year == null) return false;

            try
            {
                result = new Date(year.Value, month.Value, day.Value);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Substraction operator for Amounts
        /// </summary>
        /// <param name="v1">Left side addend</param>
        /// <param name="v2">Right side addend</param>
        /// <returns></returns>
        public static TimeSpan operator -(Date v1, Date v2)
        {
            Check.NotNull(() => v1);
            Check.NotNull(() => v2);

            var date1 = v1.DateTime;
            var date2 = v2.DateTime;

            var span = date1 - date2;

            return span;
        }

        /// <summary>
        /// Substraction operator for Amounts
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns></returns>
        public static TimeSpan Subtract(Date v1, Date v2)
        {
            return v1 - v2;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Date v1, Date v2)
        {
            if (Object.ReferenceEquals(v1, null))
            {
                if (Object.ReferenceEquals(v2, null))
                {
                    return true;
                }

                return false;
            }

            return v1.Equals(v2);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Date v1, Date v2)
        {
            return !(v1 == v2);
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >(Date v1, Date v2)
        {
            Check.NotNull(() => v1);
            Check.NotNull(() => v2);

            return v1.DateTime > v2.DateTime;
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <(Date v1, Date v2)
        {
            Check.NotNull(() => v1);
            Check.NotNull(() => v2);

            return v1.DateTime < v2.DateTime;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >=(Date v1, Date v2)
        {
            return v1 > v2 || v1 == v2;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <=(Date v1, Date v2)
        {
            return v1 < v2 || v1 == v2;
        }

        /// <summary>
        /// Compares the specified v1.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns></returns>
        public static int Compare(Date v1, Date v2)
        {
            Check.NotNull(() => v1);
            Check.NotNull(() => v2);

            if (v1 > v2) return 1;
            else if (v1 < v2) return -1;
            else return 0;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Date);
        }

        /// <summary>
        /// Equalses the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public bool Equals(Date obj)
        {
            if (Object.ReferenceEquals(obj, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, obj))
            {
                return true;
            }

            if (this.GetType() != obj.GetType())
                return false;

            return this.year == obj.year && this.month == obj.month && this.day == obj.day;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + this.year.GetHashCode();
            hash = hash * 23 + this.month.GetHashCode();
            hash = hash * 23 + this.day.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="other" /> in the sort order.  Zero This instance occurs in the same position in the sort order as <paramref name="other" />. Greater than zero This instance follows <paramref name="other" /> in the sort order.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int CompareTo(Date other)
        {
            Check.NotNull(() => other);

            if (this.DateTime < other.DateTime) return -1;
            if (this.DateTime > other.DateTime) return 1;

            return 0;
        }
    }
}