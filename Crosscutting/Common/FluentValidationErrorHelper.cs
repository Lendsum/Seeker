using FluentValidation.Results;

namespace Lendsum.Crosscutting.Common
{
    /// <summary>
    /// Helper to create fluent validation errors.
    /// </summary>
    public static class FluentValidationHelper
    {
        /// <summary>
        /// Creates the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="property">The property.</param>
        /// <param name="propertyMessage">The property message.</param>
        /// <returns></returns>
        public static FluentValidation.ValidationException CreateException(string message, string property, string propertyMessage = null)
        {
            return new FluentValidation.ValidationException(message
                , new[] { new ValidationFailure(property, propertyMessage ?? message) });
        }
    }
}