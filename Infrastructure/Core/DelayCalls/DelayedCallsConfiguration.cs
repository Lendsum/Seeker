using System.Collections.Generic;
using System.Configuration;
using System.Linq;

/// <summary>
/// 
/// </summary>
namespace Lendsum.Infrastructure.Core.DelayCalls
{
    /// <summary>
    /// Configuration section of DelayedCalls
    /// </summary>
    public class DelayedCallsQueueConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelayedCallsQueueConfiguration"/> class.
        /// </summary>
        public DelayedCallsQueueConfiguration() { }

        /// <summary>
        /// Gets or sets the queueName.
        /// </summary>
        /// <value>
        /// The queueName.
        /// </value>
        public string QueueName
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the threads.
        /// </summary>
        /// <value>
        /// The threads.
        /// </value>
        public int Threads { get; set; }
    }

    /// <summary>
    /// Class to group configuration elements.
    /// </summary>
    /// <seealso cref="ConfigurationElementCollection" />
    public class DelayedCallsQueueConfigurations
    {
        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public IEnumerable<DelayedCallsQueueConfiguration> Values { get; set; }

        /// <summary>
        /// Gets the name of the threads per.
        /// </summary>
        /// <returns></returns>
        public int GetThreadsPerName(string name)
        {
            var value = this.Values.Where(x => x.QueueName == name).FirstOrDefault();
            if (value == null) return 1;
            else return value.Threads;
        }
    }
}