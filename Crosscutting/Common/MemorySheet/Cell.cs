using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Lendsum.Crosscutting.Common.MemorySheet
{
    /// <summary>
    /// Cell contained in a table.
    /// </summary>
    public class Cell
    {
        private IEvaluable expression = null;
        private object value = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell" /> class.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="position">The position.</param>
        /// <exception cref="System.ArgumentNullException">table</exception>
        public Cell(Table table, Position position)
        {
            if (table == null) throw new ArgumentNullException("table");
            this.Table = table;
            this.Position = position;
        }

        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        /// <value>
        /// The table.
        /// </value>
        public Table Table { get; protected set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Position Position { get; protected set; }

        /// <summary>
        /// Sets the expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        public void SetExpression<T>(Expression<Func<T, object>> expression)
        {
            this.Reset();
            SheetExpression<T> sheetExpression = new SheetExpression<T>(expression);
            this.expression = sheetExpression;
        }

        /// <summary>
        /// Sets the expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        public void SetExpression<T>(Expression<Func<T, T, object>> expression)
        {
            this.Reset();
            SheetExpression<T> sheetExpression = new SheetExpression<T>(expression);
            this.expression = sheetExpression;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object Value
        {
            get
            {
                return this.Evaluate();
            }
            set
            {
                this.value = value;
            }
        }

        /// <summary>
        /// Get the dependencies of this cell.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Because must be calculed, better have a method")]
        public IEnumerable<Position> GetDependencies()
        {
            if (this.expression != null) return this.expression.Parameters.Select(x => ResolveRelativePosition(x));
            if (this.Table[this.Position.Column]?.Expression != null)
                return this.Table[this.Position.Column]?.Expression.Parameters.Select(x => ResolveRelativePosition(x));

            return null;
        }

        private void Reset()
        {
            this.value = null;
            this.expression = null;
        }

        private object Evaluate()
        {
            if (this.value != null) return this.value;
            if (this.expression != null) return EvaluateExpression(this.expression);
            if (this.Table[this.Position.Column]?.Expression != null) return EvaluateExpression(this.Table[this.Position.Column].Expression);

            return null;
        }

        private object EvaluateExpression(IEvaluable expression)
        {
            List<object> parameters = new List<object>();
            foreach (var par in expression.Parameters)
            {
                var pos = ResolveRelativePosition(par);
                parameters.Add(Table[pos.Column, pos.Row]);
            }

            return expression.Evaluate(parameters.ToArray());
        }

        private Position ResolveRelativePosition(Position pos)
        {
            int row = pos.Row;
            if (pos.RowRelative)
            {
                row = this.Position.Row + pos.Row;
            }

            return new Position(pos.Column, row);
        }
    }
}