using System;
using System.Runtime.Serialization;

namespace Lendsum.Infrastructure.Core.Exceptions
{
    /// <summary>
    /// Class to represent an exception creating a new aggregate using an event.
    /// </summary>
    [Serializable]
    public class NewAggregateEventException : EventSourcingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewAggregateEventException"/> class.
        /// </summary>
        public NewAggregateEventException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewAggregateEventException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public NewAggregateEventException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewAggregateEventException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public NewAggregateEventException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewAggregateEventException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected NewAggregateEventException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}