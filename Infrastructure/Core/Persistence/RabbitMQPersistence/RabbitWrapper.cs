using Lendsum.Crosscutting.Common;
using Lendsum.Crosscutting.Common.Extensions;
using Lendsum.Infrastructure.Core.Exceptions;
using Lendsum.Infrastructure.Core.Queues;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace Lendsum.Infrastructure.Core.Persistence.RabbitMQPersistence
{
    /// <summary>
    /// Class to be a wrapper of rabbitmq
    /// </summary>
    public class RabbitWrapper : IQueueFactory, IDisposable
    {
        private ConnectionFactory connectionFactory;
        private IConnection connection;
        private List<RabbitQueue> instanciatedQeues = new List<RabbitQueue>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitWrapper"/> class.
        /// </summary>
        public RabbitWrapper(IOptions<RabbitMQConfiguration> configuration)
        {
            Check.NotNull(() => configuration);
            var config = configuration.Value;
            this.connectionFactory = new ConnectionFactory()
            {
                HostName = config.Hostname,
                Port = config.Port,
                UserName = config.Username,
                Password = config.Password,
                AutomaticRecoveryEnabled = true
            };

            try
            {
                this.connection = connectionFactory.CreateConnection();
            }
            catch (Exception ex)
            {
                throw new QueueException("The connection cannot be stablished to {0}:{1} - {2}:{3}".InvariantFormat(config.Hostname, config.Port, config.Username, config.Password), ex);
            }
        }

        /// <summary>
        /// Gets the queue.
        /// </summary>
        /// <param name="name">The name of the queue</param>
        /// <returns>The queue ready to be used.</returns>
        public IQueue GetQueue(string name)
        {
            RabbitQueue queue = new RabbitQueue(this.connection, name);
            instanciatedQeues.Add(queue);
            return queue;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.connection != null) this.connection.Dispose();
                foreach (var instance in this.instanciatedQeues)
                {
                    if (instance != null) instance.Dispose();
                }
            }
        }
    }
}