using Lendsum.Crosscutting.Common;
using Lendsum.Infrastructure.Core.DelayCalls;
using System.Collections.Generic;

namespace Lendsum.Infrastructure.Core.Dispatcher
{
    /// <summary>
    /// Dispatcher of events generated inside the system.
    /// </summary>
    public class DispatcherHub : IDispatcherHub
    {
        private IDelayer<EventDispatcherData> eventDelayer;
        private IThreadContext threadContext;
        private IEnumerable<IListener> syncListeners = new List<IListener>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherHub" /> class.
        /// </summary>
        /// <param name="threadContext">The thread context.</param>
        /// <param name="listeners">The listeners.</param>
        /// <param name="eventDelayer">The event delayer.</param>
        /// <param name="work">The work.</param>
        public DispatcherHub(
            IThreadContext threadContext,
            IEnumerable<IListener> listeners,
            IDelayer<EventDispatcherData> eventDelayer,
            IUnitOfWork work)
        {
            Check.NotNull(() => work);
            this.syncListeners = Check.NotNull(() => listeners);

            // breaking the circular reference here.
            work.DispatcherHub = this;
            this.eventDelayer = Check.NotNull(() => eventDelayer);
            this.threadContext = Check.NotNull(() => threadContext);
        }

        private Queue<AggregateEvent> enqueuedEvents
        {
            get
            {
                var value = this.threadContext.GetValue<Queue<AggregateEvent>>("dispatcherHub.eventsToProcessSync");

                if (value == null)
                {
                    value = new Queue<AggregateEvent>();
                    this.threadContext.Update(value, "dispatcherHub.eventsToProcessSync");
                }

                return value;
            }
        }

        private bool processingSync
        {
            get
            {
                return this.threadContext.GetValue<bool>("dispatcherHub.processingSync");
            }
            set
            {
                this.threadContext.Update(value, "dispatcherHub.processingSync");
            }
        }

        /// <summary>
        /// Dispatches the specified event across the listeners that can consume the event.
        /// </summary>
        /// <param name="incomingEvent">The event.</param>
        public void Dispatch(AggregateEvent incomingEvent)
        {
            Check.NotNull(() => incomingEvent);

            this.enqueuedEvents.Enqueue(incomingEvent);

            if (!this.processingSync)
            {
                this.processingSync = true;
                while (this.enqueuedEvents.Count > 0)
                {
                    var eventToProcess = this.enqueuedEvents.Dequeue();
                    if (eventToProcess.ControlEvent == false)
                    {
                        foreach (var listener in this.syncListeners)
                        {
                            listener.Consume(eventToProcess);
                        }
                    }

                    if (eventToProcess.ControlEvent == false)
                    {
                        var eventDispatcherData = new EventDispatcherData
                        {
                            AggregateUid = eventToProcess.AggregateUid
                        };
                        this.eventDelayer.Call(eventDispatcherData);
                    }
                }

                this.processingSync = false;
            }
        }
    }
}