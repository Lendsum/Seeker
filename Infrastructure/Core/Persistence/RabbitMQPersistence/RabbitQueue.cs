using Lendsum.Crosscutting.Common;
using Lendsum.Crosscutting.Common.Extensions;
using Lendsum.Infrastructure.Core.Exceptions;
using Lendsum.Infrastructure.Core.Queues;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Lendsum.Infrastructure.Core.Persistence.RabbitMQPersistence
{
    /// <summary>
    /// Abstraction of a rabbit queue.
    /// </summary>
    public class RabbitQueue : IQueue, IDisposable
    {
        private IModel channel;
        private IConnection connection;
        private string queueName;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitQueue" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="queueName">Name of the queue.</param>
        public RabbitQueue(IConnection connection, string queueName)
        {
            this.connection = Check.NotNull(() => connection);
            this.queueName = Check.NotNullOrEmpty(() => queueName);
            this.channel = this.connection.CreateModel();
            this.channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.BasicQos(prefetchSize: 0, prefetchCount: 10, global: false);
        }

        /// <summary>
        /// Pushes the specified message into the queue in unicode format
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="EventSourcingException"></exception>
        public void Push(string message)
        {
            Check.NotNull(() => message);

            try
            {
                var body = Encoding.Unicode.GetBytes(message);
                lock (this.channel)
                {
                    this.channel.BasicPublish(exchange: "", routingKey: this.queueName, basicProperties: null, body: body);
                }
            }
            catch (Exception ex)
            {
                throw new EventSourcingException(S.Invariant($"Error pushing into the queue {queueName}, message {message}"), ex);
            }
        }

        /// <summary>
        /// Associate an action to a queue to be executed when a message arrives.
        /// </summary>
        /// <returns>Null if cancelation token is activated, the value in other cases.</returns>
        public void Read(Action<string> consumingAction, ILogger log = null)
        {
            Action<byte[]> action = new Action<byte[]>(x => consumingAction(Encoding.Unicode.GetString(x)));
            this.Read(action, log);
        }

        /// <summary>
        /// Associate an action to a queue to be executed when a message arrives.
        /// </summary>
        /// <param name="consumingAction">The consuming action.</param>
        /// <param name="log">The log.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void Read(Action<byte[]> consumingAction, ILogger log)
        {
            var consumer = new EventingBasicConsumer(this.channel);
            consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body;
                    consumingAction.Invoke(body);
                    this.SendAck(ea);
                }
                catch (Exception ex)
                {
                    log?.LogError("Error reading the queue {0}".InvariantFormat(this.queueName), ex);
                    throw new EventSourcingException("Error reading the queue {0}".InvariantFormat(this.queueName), ex);
                }
            };

            this.channel.BasicConsume(queue: this.queueName, autoAck: false, consumer: consumer);
        }

        /// <summary>
        /// Associate an action to a queue to be executed when a message arrives but the ack will not be sent.
        /// Use SendAck to send the ack
        /// </summary>
        /// <param name="consumingAction">The consuming action.</param>
        /// <param name="log">The log.</param>
        public void ReadNoAck(Action<string, BasicDeliverEventArgs> consumingAction, ILogger log = null)
        {
            Action<byte[], BasicDeliverEventArgs> action = new Action<byte[], BasicDeliverEventArgs>((x, y) => consumingAction(Encoding.Unicode.GetString(x), y));
            this.ReadNoAck(action, log);
        }

        /// <summary>
        /// Associate an action to a queue to be executed when a message arrives but the ack will not be sent.
        /// Use SendAck to send the ack
        /// </summary>
        /// <returns>Null if cancelation token is activated, the value in other cases.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void ReadNoAck(Action<byte[], BasicDeliverEventArgs> consumingAction, ILogger log)
        {
            var consumer = new EventingBasicConsumer(this.channel);
            consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body;
                    consumingAction.Invoke(body, ea);
                }
                catch (Exception ex)
                {
                    log?.LogError("Error reading the queue {0}".InvariantFormat(this.queueName), ex);
                    throw new EventSourcingException("Error reading the queue {0}".InvariantFormat(this.queueName), ex);
                }
            };

            this.channel.BasicConsume(queue: this.queueName, autoAck: false, consumer: consumer);
        }

        /// <summary>
        /// Sends the ack.
        /// </summary>
        /// <param name="ea">The <see cref="BasicDeliverEventArgs"/> instance containing the event data.</param>
        public void SendAck(BasicDeliverEventArgs ea)
        {
            Check.NotNull(() => ea);
            this.channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }

        /// <summary>
        /// Sends the no ack.
        /// </summary>
        /// <param name="ea">The <see cref="BasicDeliverEventArgs"/> instance containing the event data.</param>
        public void SendNoAck(BasicDeliverEventArgs ea)
        {
            Check.NotNull(() => ea);
            this.channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
        }

        /// <summary>
        /// Purges this instance.
        /// </summary>
        public void Purge()
        {
            Action<string> nullAction = new Action<string>(x => { });
            this.Read(nullAction);
            Thread.Sleep(500);
            this.channel.Dispose();
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
                if (this.channel != null) this.channel.Close();
                if (this.channel != null) this.channel.Dispose();
            }
        }
    }
}