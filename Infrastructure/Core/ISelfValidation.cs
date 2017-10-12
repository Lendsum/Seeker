namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Interface to be implemented by dtos that can be autovalidated.
    /// </summary>
    public interface ISelfValidation
    {
        /// <summary>
        /// Validates this instance and throw a FluentValidation.ValidationException if something is wrong.
        /// </summary>
        void SelfValidate();
    }
}