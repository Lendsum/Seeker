using Lendsum.Infrastructure.Core.Dispatcher;
using Lendsum.Infrastructure.Core.Persistence;

namespace Lendsum.Infrastructure.Core.Projections
{
    /// <summary>
    /// Class to implement a basic query. it also implements the Listener to receive the events and change the query according to that.
    /// </summary>
    public abstract class AsyncProjection<T> : ProjectionBase<T>, IAsyncListener where T : class, IPersistable
    {
        /// <summary>
        /// The rebuildable
        /// </summary>
        public bool Rebuildable => true;

        /// <summary>
        /// Gets a value indicating whether [consume only last event].
        /// </summary>
        /// <value>
        /// <c>true</c> if [consume only last event]; otherwise, <c>false</c>.
        /// </value>
        public bool ConsumeOnlyLastEvent => false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncProjection{T}"/> class.
        /// </summary>
        protected AsyncProjection(IPersistenceProvider provider, IUnitOfWork work) : base(provider, work)
        {
        }
    }
}