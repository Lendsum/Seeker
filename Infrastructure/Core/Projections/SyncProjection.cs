using Lendsum.Infrastructure.Core.Dispatcher;
using Lendsum.Infrastructure.Core.Persistence;

namespace Lendsum.Infrastructure.Core.Projections
{
    /// <summary>
    /// Class to implement a basic query. it also implements the Listener to receive the events and change the query according to that.
    /// </summary>
    public abstract class SyncProjection<T> : ProjectionBase<T>, IListener where T : class, IPersistable
    {
        /// <summary>
        /// Projections are rebuildable by definition
        /// </summary>
        public bool Rebuildable => true;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncProjection{T}"/> class.
        /// </summary>
        protected SyncProjection(IPersistenceProvider provider, IUnitOfWork work) : base(provider, work)
        {
        }
    }
}