namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Interface to be implemented by classses to be the base of any document that can be persistable.
    /// </summary>
    public interface IPersistable
    {
        /// <summary>
        /// Gets the type of the document.
        /// </summary>
        /// <value>
        /// The type of the document.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Mandatory for serialization purpouses")]
        string DocumentType { get; }

        /// <summary>
        /// Gets the document key.
        /// </summary>
        /// <value>
        /// The document key.
        /// </value>
        string DocumentKey { get; }

        /// <summary>
        /// Gets or sets the cas. This value must be used only by persistance layer.
        /// </summary>
        /// <value>
        /// The cas.
        /// </value>
        ulong Cas { get; set; }
    }
}