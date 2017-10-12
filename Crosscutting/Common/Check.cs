using Lendsum.Crosscutting.Common.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Lendsum.Crosscutting.Common
{
    /// <summary>
    /// Class to perform parameter validation
    /// </summary>
    public static class Check
    {
        /// <summary>
        /// Checks that the string parameter is not null or empty
        /// </summary>
        /// <param name="parameterExpression">The parameter expression.</param>
        /// <returns>The value of the expression</returns>
        /// <exception cref="System.ArgumentNullException">
        /// parameterExpression
        /// or
        /// </exception>
        /// <exception cref="System.ArgumentException">Cannot be null or empty</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "It is a regular string expression")]
        public static string NotNullOrEmpty(Expression<Func<string>> parameterExpression)
        {
            if (parameterExpression == null)
            {
                throw new ArgumentNullException("parameterExpression");
            }

            var memberExpression = GetParameterExpression(parameterExpression);

            string value = parameterExpression.Compile()();

            if (value == null)
            {
                string name = GetParameterName(memberExpression);
                throw new ArgumentNullException(name);
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                string name = GetParameterName(memberExpression);
                throw new ArgumentException("Cannot be null or empty", name);
            }

            return value;
        }

        /// <summary>
        /// Checks that the parameter is not null
        /// </summary>
        /// <typeparam name="T">Type of the parameter</typeparam>
        /// <param name="parameterExpression">The parameter expression.</param>
        /// <returns>The value of the expression</returns>
        /// <exception cref="System.ArgumentNullException">
        /// parameterExpression
        /// or
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "It is a regular expression")]
        public static T NotNull<T>(Expression<Func<T>> parameterExpression)
        {
            if (parameterExpression == null)
            {
                throw new ArgumentNullException("parameterExpression");
            }

            var memberExpression = GetParameterExpression(parameterExpression);

            var value = parameterExpression.Compile()();
            if (value == null)
            {
                string name = GetParameterName(memberExpression);
                throw new ArgumentNullException(name);
            }

            return value;
        }

        /// <summary>
        /// Check if the specified parameter expression is between the range
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameterExpression">The parameter expression.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>The value of the expression</returns>
        /// <exception cref="System.ArgumentNullException">minValue
        /// or
        /// maxValue
        /// or</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The value is less than {0}.InvariantFormat(minValue)
        /// or
        /// The value is greater than {0}.InvariantFormat(maxValue)
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "It is a regular expression")]
        public static T Range<T>(Expression<Func<T>> parameterExpression, T minValue, T maxValue) where T : IComparable<T>
        {
            if (parameterExpression == null)
            {
                throw new ArgumentNullException("parameterExpression");
            }

            if (minValue == null)
            {
                throw new ArgumentNullException("minValue");
            }

            if (maxValue == null)
            {
                throw new ArgumentNullException("maxValue");
            }

            var memberExpression = GetParameterExpression(parameterExpression);

            var value = parameterExpression.Compile()();
            string name = GetParameterName(memberExpression);

            if (value == null)
            {
                throw new ArgumentNullException(name);
            }

            if (value.CompareTo(minValue) < 0)
            {
                throw new ArgumentOutOfRangeException(
                    name,
                    value,
                    "The value is less than {0}".InvariantFormat(minValue));
            }

            if (value.CompareTo(maxValue) > 0)
            {
                throw new ArgumentOutOfRangeException(
                    name,
                    value,
                    "The value is greater than {0}".InvariantFormat(maxValue));
            }

            return value;
        }

        /// <summary>
        /// Validates the enumeration value.
        /// </summary>
        /// <typeparam name="T">Type of the enumeration</typeparam>
        /// <param name="parameterExpression">The parameter expression.</param>
        /// <returns>The enumeration value if it is valid</returns>
        public static T ValidEnumValue<T>(Expression<Func<T>> parameterExpression) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            if (parameterExpression == null)
            {
                throw new ArgumentNullException("parameterExpression");
            }

            var memberExpression = GetMemberExpression(parameterExpression);

            var value = parameterExpression.Compile()();

            if (!Enum.IsDefined(typeof(T), value))
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Argument {0} has value {1} which is not valid in enumeration {2}",
                    memberExpression.Member.Name,
                    value,
                    typeof(T).FullName);
                throw new ArgumentException(message);
            }

            return value;
        }

        /// <summary>
        /// Gets the member expression, validating that the expression is .
        /// </summary>
        /// <typeparam name="T">Type of the expression</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>
        /// Member expression of the passed expression
        /// </returns>
        private static MemberExpression GetMemberExpression<T>(Expression<Func<T>> expression)
        {
            MemberExpression member = expression.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Expression '{0}' refers to a method, not a parameter.", expression));
            }

            return member;
        }

        /// <summary>
        /// Gets the member expression, validating that the expression is .
        /// </summary>
        /// <typeparam name="T">Type of the expression</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>
        /// Member expression of the passed expression
        /// </returns>
        private static MemberExpression GetParameterExpression<T>(Expression<Func<T>> expression)
        {
            MemberExpression member = GetMemberExpression(expression);

            FieldInfo propInfo = member.Member as FieldInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Expression '{0}' must be a parameter.", expression));
            }

            return member;
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Name of the parameter</returns>
        private static string GetParameterName(MemberExpression expression)
        {
            return expression.Member.Name;
        }
    }
}