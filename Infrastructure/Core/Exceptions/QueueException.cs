using System;
using System.Runtime.Serialization;

namespace Lendsum.Infrastructure.Core.Exceptions
{
    /// <summary>
    /// Class to represent an exception creating a new aggregate using an event.
    /// </summary>
    [Serializable]
    public class QueueException : EventSourcingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueException"/> class.
        /// </summary>
        public QueueException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public QueueException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public QueueException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected QueueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}