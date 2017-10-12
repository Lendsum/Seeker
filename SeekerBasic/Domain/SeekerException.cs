using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeekerBasic.Domain
{

    /// <summary>
    /// Exception from Seeker Domain
    /// </summary>
    [Serializable]
    public class SeekerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeekerException"/> class.
        /// </summary>
        public SeekerException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SeekerException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SeekerException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SeekerException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public SeekerException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SeekerException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected SeekerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
