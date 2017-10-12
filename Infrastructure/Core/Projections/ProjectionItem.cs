using Lendsum.Crosscutting.Common;

namespace Lendsum.Infrastructure.Core.Projections
{
    /// <summary>
    /// Class to wrapp the items that a query produces
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ProjectionItem<T> where T : IPersistable
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public T Value { get; private set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public ProjectionItemActionEnum ProjectionItemAction { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionItem{T}" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        public ProjectionItem(T value, ProjectionItemActionEnum type)
        {
            this.Value = Check.NotNull(() => value);
            this.ProjectionItemAction = type;
        }

        /// <summary>
        /// Gets this projection item in a persistable form
        /// </summary>
        /// <value>
        /// The persistable.
        /// </value>
        public ProjectionItem<IPersistable> Persistable
        {
            get
            {
                return new ProjectionItem<IPersistable>(this.Value, this.ProjectionItemAction);
            }
        }
    }
}