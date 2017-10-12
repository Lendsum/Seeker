using System;
using System.Linq.Expressions;

namespace Lendsum.Crosscutting.Common
{
    /// <summary>
    /// Class with methods about reflection.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Gets the name of the property given the expression.
        /// </summary>
        /// <returns></returns>
        public static string GetPropertyName<T>(Expression<Func<T>> exp)
        {
            Check.NotNull(() => exp);

            MemberExpression body = exp.Body as MemberExpression;

            if (body == null)
            {
                UnaryExpression ubody = exp.Body as UnaryExpression;
                if (ubody == null)
                {
                    throw new LendsumException("The expression is not a property");
                }

                body = ubody.Operand as MemberExpression;
                if (body == null)
                {
                    throw new LendsumException("The expression is not a property");
                }
            }

            return body.Member.Name;
        }
    }
}