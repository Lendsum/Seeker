using Lendsum.Infrastructure.Core.Persistence;
using Lendsum.Infrastructure.Core.Projections;
using System;
using System.Collections.Generic;

namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Base class to implement a constraint.
    /// </summary>
    public class Constraint<T> : SyncProjection<T> where T : ConstraintItem, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Constraint{T}"/> class.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="work"></param>
        protected Constraint(IPersistenceProvider provider, IUnitOfWork work) : base(provider, work)
        {
        }

        /// <summary>
        /// check if the constraint is already in use and, if not, assignt it to the aggregate uid.
        /// </summary>
        /// <param name="constraintValue">The value.</param>
        /// <param name="aggregateUid">The aggregate uid.</param>
        /// <returns>
        /// True if constraint could be reserved, false if the constraint is already in use.
        /// </returns>
        public bool CheckAndReserve(string constraintValue, Guid aggregateUid)
        {
            T item = new T() { ConstraintValue = constraintValue };
            item = this.Get(item);
            if (item != null && item.AggregateUid != aggregateUid)
            {
                return false;
            }

            this.AddOrReplaceItem(new T() { AggregateUid = aggregateUid, ConstraintValue = constraintValue });
            return true;
        }

        /// <summary>
        /// check if the constraint is already in use
        /// </summary>
        /// <param name="constraintValue">The value.</param>
        /// <returns>
        /// True if constraint could be reserved, false if the constraint is already in use.
        /// </returns>
        public bool InUse(string constraintValue)
        {
            return this.GetAggregateUidByConstraint(constraintValue).HasValue;
        }

        /// <summary>
        /// Gets the aggregateUid reserved with this contrain.
        /// </summary>
        /// <param name="constraintValue">The value.</param>
        /// <returns>
        /// True if constraint could be reserved, false if the constraint is already in use.
        /// </returns>
        public Guid? GetAggregateUidByConstraint(string constraintValue)
        {
            T item = new T() { ConstraintValue = constraintValue };
            item = this.Get(item);
            if (item != null)
            {
                return item.AggregateUid;
            }

            return null;
        }

        /// <summary>
        /// Releases the specified constraint value to be used by another aggregate uid.
        /// </summary>
        /// <param name="constraintValue">The constraint value.</param>
        /// <param name="aggregateUid">The aggregate uid.</param>
        public void Release(string constraintValue, Guid aggregateUid)
        {
            T item = new T() { ConstraintValue = constraintValue, AggregateUid = aggregateUid };
            item = this.Get(item);
            if (item != null && item.AggregateUid != aggregateUid)
            {
                return;
            }

            this.RemoveItem(item);
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetAll { get { return this.GetByPrefix(new T()); } }
    }
}