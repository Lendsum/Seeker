using System;

namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Abstract class to use as a skeleton to build a constrain.
    /// </summary>
    public abstract class ConstraintItem : IPersistable
    {
        /// <summary>
        /// Gets or sets the cas. This value must be used only by persistance layer.
        /// </summary>
        /// <value>
        /// The cas.
        /// </value>
        public ulong Cas
        {
            get; set;
        }

        /// <summary>
        /// Gets the document key.
        /// </summary>
        /// <value>
        /// The document key.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public string DocumentKey
        {
            get
            {
                return this.DocumentType + ":" + this.ConstraintValue;
            }
        }

        /// <summary>
        /// Gets the type of the document.
        /// </summary>
        /// <value>
        /// The type of the document.
        /// </value>
        public abstract string DocumentType { get; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string ConstraintValue { get; set; }

        /// <summary>
        /// Gets or sets the aggregate uid.
        /// </summary>
        /// <value>
        /// The aggregate uid.
        /// </value>
        public Guid AggregateUid { get; set; }
    }
}