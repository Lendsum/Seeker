//-----------------------------------------------------------------------
// <copyright file="ExpressionExtensions.cs" company="Mediatech Solutions">
//      Copyright (c) Mediatech Solutions. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Lendsum.Crosscutting.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Expression extensions.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// The mask
        /// </summary>
        private const string Mask = "*****";

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>
        /// The name of an expression.
        /// </returns>
        /// <exception cref="System.ArgumentException">Expression is not a member access;expression</exception>
        public static string GetMemberName(this LambdaExpression expression)
        {
            Check.NotNull(() => expression);

            MemberExpression body = (expression.Body.NodeType == ExpressionType.Convert) ?
                (MemberExpression)((UnaryExpression)expression.Body).Operand :
                (MemberExpression)expression.Body;

            return body.Member.Name;
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>
        /// The name of an expression.
        /// </returns>
        /// <exception cref="System.ArgumentException">Expression is not a member access;expression</exception>
        public static string GetFullMemberName(this LambdaExpression expression)
        {
            Check.NotNull(() => expression);

            MemberExpression body = (expression.Body.NodeType == ExpressionType.Convert) ?
                (MemberExpression)((UnaryExpression)expression.Body).Operand :
                (MemberExpression)expression.Body;

            string fullMemberName = body.ToString();

            int firstDot = fullMemberName.IndexOf('.');
            if (firstDot > 0)
            {
                return fullMemberName.Substring(firstDot + 1);
            }

            return fullMemberName;
        }

        /// <summary>
        /// Gets a dictionary where the key is the parameter name of the method call and the value is the value passed to the method.
        /// If the call contains sensible data you can filter this data by passing the name of the parameter in the filter argument.
        /// When a parameter is filter its value is replaced by a mask (*****) but it still appears in the dictionary.
        /// </summary>
        /// <param name="expression">The expression should be a lambda expression of a method call. Example: () => anObject.AMethodCall(parameter1, parameter2, 5, true). If is not a lambda expression null is returned.</param>
        /// <param name="filter">The names of the parameters which value are going to be masked in the resulting dictionary.</param>
        /// <returns>A dictionary where the key is the parameter name of the method call and the value is the value passed to the method.</returns>
        public static IDictionary<string, object> GetParametersWithValues(this LambdaExpression expression, params string[] filter)
        {
            Check.NotNull(() => expression);

            var call = expression.Body as MethodCallExpression;
            if (call == null)
            {
                return null;
            }

            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            var parameters = call.Method.GetParameters();

            for (int i = 0; i < call.Arguments.Count; i++)
            {
                var parameter = parameters[i];
                if (filter.Contains(parameter.Name))
                {
                    dictionary.Add(parameter.Name, Mask);
                }
                else
                {
                    Expression argument = call.Arguments[i];

                    LambdaExpression lambda = Expression.Lambda(argument, expression.Parameters);

                    Delegate d = lambda.Compile();
                    object value = d.DynamicInvoke(new object[0]);

                    dictionary.Add(parameter.Name, value);
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Gets the method name on a method call.
        /// </summary>
        /// <param name="expression">The expression should be a lambda expression of a method call. Example: () => anObject.AMethodCall(parameter1, parameter2, 5, true). If is not a lambda expression null is returned.</param>
        /// <returns>
        /// The method name. Example: () => anObject.AMethodCall(parameter1, parameter2, 5, true) returns "AMethodCall"
        /// </returns>
        public static string GetMethodName(this LambdaExpression expression)
        {
            Check.NotNull(() => expression);

            var call = expression.Body as MethodCallExpression;
            if (call == null)
            {
                return null;
            }

            return call.Method.Name;
        }
    }
}