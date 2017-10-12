using System.Configuration;

namespace Lendsum.Infrastructure.Core.Persistence.RabbitMQPersistence
{
    /// <summary>
    /// Class to wrap rabbitMQ configuration
    /// </summary>
    public class RabbitMQConfiguration 
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="RabbitMQConfiguration"/> class from being created.
        /// </summary>
        public RabbitMQConfiguration() {
            this.Hostname = "localhost";
            this.Port = 5672;
            this.Username = "guest";
            this.Password = "guest";

        }

        /// <summary>
        /// Gets or sets the hostname.
        /// </summary>
        /// <value>
        /// The hostname.
        /// </value>
        public string Hostname { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }
    }
}