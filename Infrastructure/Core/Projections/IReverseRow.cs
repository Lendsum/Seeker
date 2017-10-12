namespace Lendsum.Infrastructure.Core.Projections
{
    /// <summary>
    /// Interface to be implmeented by reversable projection rows.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReverseRow<T> where T : IPersistable
    {
        /// <summary>
        /// Gets the reverse version of the item T
        /// </summary>
        /// <value>
        /// The reverse.
        /// </value>
        T Reverse();
    }
}