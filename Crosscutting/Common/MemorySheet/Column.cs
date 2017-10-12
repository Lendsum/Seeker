namespace Lendsum.Crosscutting.Common.MemorySheet
{
    /// <summary>
    ///
    /// </summary>
    public class Column
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>
        /// The expression.
        /// </value>
        public IEvaluable Expression { get; protected set; }

        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        /// <value>
        /// The table.
        /// </value>
        public Table Table { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Column"/> class.
        /// </summary>
        public Column(string name)
        {
            Position.CheckColumnName(name);
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Column"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="expression">The expression.</param>
        public Column(string name, IEvaluable expression) : this(name)
        {
            this.Expression = expression;
        }
    }
}