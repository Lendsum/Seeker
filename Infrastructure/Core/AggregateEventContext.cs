using Lendsum.Crosscutting.Common;
using Lendsum.Crosscutting.Common.Extensions;
using System.Collections.Generic;

namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Context to be carried on about the event
    /// </summary>
    public class AggregateEventContext : IAggregateEventContext
    {
        private IThreadContext context;
        private const string contextKey = "AggregateEventContext";

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateEventContext"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public AggregateEventContext(IThreadContext context)
        {
            this.context = Check.NotNull(() => context);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            Check.NotNull(() => key);
            return this.Storage.GetOrAdd(key, () => string.Empty);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="key">The key.</param>
        public void SetValue(string key, string value)
        {
            this.Storage.AddOrReplace(key, value);
        }

        /// <summary>
        /// Loads from event.
        /// </summary>
        /// <param name="event">The event.</param>
        public void LoadFromEvent(AggregateEvent @event)
        {
            Check.NotNull(() => @event);

            if (@event.Context != null)
            {
                @event.Context.ForEach(x => this.Storage.AddOrReplace(x.Key, x.Value));
            }
        }

        /// <summary>
        /// Attaches to event.
        /// </summary>
        /// <param name="event">The event.</param>
        public void AttachToEvent(AggregateEvent @event)
        {
            Check.NotNull(() => @event);
            @event.Context = Clone(this.Storage);
        }

        private Dictionary<string, string> Storage
        {
            get
            {
                var storage = context.GetValue<Dictionary<string, string>>(contextKey);
                if (storage == null)
                {
                    storage = new Dictionary<string, string>();
                    context.Update(storage, contextKey);
                }

                return storage;
            }
        }

        private static Dictionary<string, string> Clone(Dictionary<string, string> original)
        {
            if (original == null) return null;

            var result = new Dictionary<string, string>();
            foreach (var item in original)
            {
                result.Add(item.Key, item.Value);
            }

            return result;
        }
    }
}