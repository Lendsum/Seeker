namespace Lendsum.Infrastructure.Core.Projections
{
    /// <summary>
    /// Enum to mark a ProjectionItem to be handled as a new item, or to update, or to delete.
    /// </summary>
    public enum ProjectionItemActionEnum
    {
        /// <summary>
        /// The not set
        /// </summary>
        NotSet = 0,

        /// <summary>
        /// The new
        /// </summary>
        New = 10,

        /// <summary>
        /// The update or new
        /// </summary>
        UpdateOrNew = 20,

        /// <summary>
        /// The delete
        /// </summary>
        Delete = 30,

        /// <summary>
        /// Do nothing with the value.
        /// </summary>
        Nothing = 40
    }
}