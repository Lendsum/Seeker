using System;
using System.Collections.Generic;
using System.Text;

namespace Lendsum.Infrastructure.Core.Persistence.SqlServerPersistence
{
    /// <summary>
    /// Wrapp the connection strings.
    /// </summary>
    public class ConnectionString
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }
    }

    /// <summary>
    /// Collection of the connection strings.
    /// </summary>
    public class ConnectionStrings
    {
        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public IEnumerable<ConnectionString> Values { get; set; }
    }
}
