using System;

namespace Lendsum.Crosscutting.Common
{
    /// <summary>
    /// Base class for all exceptions in Lendsum namespace
    /// </summary>
    [Serializable]
    public class LendsumException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LendsumException"/> class.
        /// </summary>
        public LendsumException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LendsumException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public LendsumException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LendsumException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public LendsumException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LendsumException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected LendsumException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}