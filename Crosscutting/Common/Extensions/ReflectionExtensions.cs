using System;

namespace Lendsum.Crosscutting.Common.Extensions
{
    /// <summary>
    /// Extensions involving type introspection
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Gets the property value as an instance of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="origin">The origin.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Property '{0}' was not found on given object.InvariantFormat(propertyName)</exception>
        public static T GetPropertyValue<T>(this object origin, string propertyName)
        {
            Check.NotNull(() => origin);
            Check.NotNull(() => propertyName);
            var property = origin.GetType().GetProperty(propertyName);
            if (property == null) throw new InvalidOperationException("Property '{0}' was not found on given object".InvariantFormat(propertyName));
            return (T)property.GetValue(origin);
        }

        /// <summary>
        /// Gets the field value as an instance of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="origin">The origin.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Field '{0}' was not found on given object.InvariantFormat(fieldName)</exception>
        public static T GetFieldValue<T>(this object origin, string fieldName)
        {
            Check.NotNull(() => origin);
            Check.NotNull(() => fieldName);
            var field = origin.GetType().GetField(fieldName);
            if (field == null) throw new InvalidOperationException("Field '{0}' was not found on given object".InvariantFormat(fieldName));
            return (T)field.GetValue(origin);
        }

        /// <summary>
        /// Determines whether the specified origin has a property whith the given name.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static bool HasProperty(this object origin, string propertyName)
        {
            Check.NotNull(() => origin);
            Check.NotNull(() => propertyName);
            return origin.GetType().GetProperty(propertyName) != null;
        }

        /// <summary>
        /// Determines whether the specified origin has a field whith the given name.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public static bool HasField(this object origin, string fieldName)
        {
            Check.NotNull(() => origin);
            Check.NotNull(() => fieldName);
            return origin.GetType().GetField(fieldName) != null;
        }
    }
}