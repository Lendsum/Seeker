using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using System;

namespace Lendsum.Infrastructure.Core.Queues
{
    /// <summary>
    /// interface to represent a queue
    /// </summary>
    public interface IQueue
    {
        /// <summary>
        /// Pushes the specified message into the queue, in unicode format.
        /// </summary>
        /// <param name="message">The message.</param>
        void Push(string message);

        /// <summary>
        /// Associate an action to a queue to be executed when a message arrives. in unicode format.
        /// </summary>
        /// <returns>Null if cancelation token is activated, the value in other cases.</returns>
        void Read(Action<string> consumingAction, ILogger log = null);

        /// <summary>
        /// Associate an action to a queue to be executed when a message arrives.
        /// </summary>
        /// <returns>Null if cancelation token is activated, the value in other cases.</returns>
        void Read(Action<byte[]> consumingAction, ILogger log);

        /// <summary>
        /// Associate an action to a queue to be executed when a message arrives but the ack will not be sent.
        /// Use SendAck to send the ack
        /// </summary>
        /// <param name="consumingAction">The consuming action.</param>
        /// <param name="log">The log.</param>
        void ReadNoAck(Action<string, BasicDeliverEventArgs> consumingAction, ILogger log = null);

        /// <summary>
        /// Associate an action to a queue to be executed when a message arrives but the ack will not be sent.
        /// Use SendAck to send the ack
        /// </summary>
        /// <returns>Null if cancelation token is activated, the value in other cases.</returns>
        void ReadNoAck(Action<byte[], BasicDeliverEventArgs> consumingAction, ILogger log);

        /// <summary>
        /// Sends the ack.
        /// </summary>
        /// <param name="ea">The <see cref="BasicDeliverEventArgs"/> instance containing the event data.</param>
        void SendAck(BasicDeliverEventArgs ea);

        /// <summary>
        /// Sends the no ack.
        /// </summary>
        /// <param name="ea">The <see cref="BasicDeliverEventArgs"/> instance containing the event data.</param>
        void SendNoAck(BasicDeliverEventArgs ea);

        /// <summary>
        /// Purges this instance.
        /// </summary>
        void Purge();
    }
}