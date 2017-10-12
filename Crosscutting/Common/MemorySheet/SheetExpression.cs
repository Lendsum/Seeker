using System;
using System.Linq;
using System.Linq.Expressions;

namespace Lendsum.Crosscutting.Common.MemorySheet
{
    /// <summary>
    /// Represent a expression, a way to include lambda expression in table.
    /// </summary>
    public sealed class SheetExpression<T> : IEvaluable
    {
        private Position[] parameters = null;
        private Delegate func = null;

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public Position[] Parameters
        {
            get
            {
                return this.parameters;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SheetExpression{T}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public SheetExpression(Expression<Func<T, object>> expression)
        {
            if (expression == null) return;
            this.parameters = expression.Parameters.Select(x => new Position(x.Name)).ToArray();
            this.func = expression.Compile();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SheetExpression{T}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public SheetExpression(Expression<Func<T, T, object>> expression)
        {
            if (expression == null) return;
            this.parameters = expression.Parameters.Select(x => new Position(x.Name)).ToArray();
            this.func = expression.Compile();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SheetExpression{T}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public SheetExpression(Expression<Func<T, T, T, object>> expression)
        {
            if (expression == null) return;
            this.parameters = expression.Parameters.Select(x => new Position(x.Name)).ToArray();
            this.func = expression.Compile();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SheetExpression{T}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public SheetExpression(Expression<Func<T, T, T, T, T, object>> expression)
        {
            if (expression == null) return;
            this.parameters = expression.Parameters.Select(x => new Position(x.Name)).ToArray();
            this.func = expression.Compile();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SheetExpression{T}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public SheetExpression(Expression<Func<T, T, T, T, object>> expression)
        {
            if (expression == null) return;
            this.parameters = expression.Parameters.Select(x => new Position(x.Name)).ToArray();
            this.func = expression.Compile();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SheetExpression{T}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public SheetExpression(Expression<Func<T, T, T, T, T, T, object>> expression)
        {
            if (expression == null) return;
            this.parameters = expression.Parameters.Select(x => new Position(x.Name)).ToArray();
            this.func = expression.Compile();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SheetExpression{T}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public SheetExpression(Expression<Func<T, T, T, T, T, T, T, object>> expression)
        {
            if (expression == null) return;
            this.parameters = expression.Parameters.Select(x => new Position(x.Name)).ToArray();
            this.func = expression.Compile();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SheetExpression{T}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public SheetExpression(Expression<Func<T, T, T, T, T, T, T, T, object>> expression)
        {
            if (expression == null) return;
            this.parameters = expression.Parameters.Select(x => new Position(x.Name)).ToArray();
            this.func = expression.Compile();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SheetExpression{T}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public SheetExpression(Expression<Func<T, T, T, T, T, T, T, T, T, object>> expression)
        {
            if (expression == null) return;
            this.parameters = expression.Parameters.Select(x => new Position(x.Name)).ToArray();
            this.func = expression.Compile();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SheetExpression{T}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public SheetExpression(Expression<Func<T, T, T, T, T, T, T, T, T, T, object>> expression)
        {
            if (expression == null) return;
            this.parameters = expression.Parameters.Select(x => new Position(x.Name)).ToArray();
            this.func = expression.Compile();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SheetExpression{T}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public SheetExpression(Expression<Func<T, T, T, T, T, T, T, T, T, T, T, object>> expression)
        {
            if (expression == null) return;
            this.parameters = expression.Parameters.Select(x => new Position(x.Name)).ToArray();
            this.func = expression.Compile();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SheetExpression{T}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public SheetExpression(Expression<Func<T, T, T, T, T, T, T, T, T, T, T, T, object>> expression)
        {
            if (expression == null) return;
            this.parameters = expression.Parameters.Select(x => new Position(x.Name)).ToArray();
            this.func = expression.Compile();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SheetExpression{T}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public SheetExpression(Expression<Func<T, T, T, T, T, T, T, T, T, T, T, T, T, object>> expression)
        {
            if (expression == null) return;
            this.parameters = expression.Parameters.Select(x => new Position(x.Name)).ToArray();
            this.func = expression.Compile();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SheetExpression{T}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public SheetExpression(Expression<Func<T, T, T, T, T, T, T, T, T, T, T, T, T, T, object>> expression)
        {
            if (expression == null) return;
            this.parameters = expression.Parameters.Select(x => new Position(x.Name)).ToArray();
            this.func = expression.Compile();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SheetExpression{T}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public SheetExpression(Expression<Func<T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, object>> expression)
        {
            if (expression == null) return;
            this.parameters = expression.Parameters.Select(x => new Position(x.Name)).ToArray();
            this.func = expression.Compile();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SheetExpression{T}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public SheetExpression(Expression<Func<T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, object>> expression)
        {
            if (expression == null) return;
            this.parameters = expression.Parameters.Select(x => new Position(x.Name)).ToArray();
            this.func = expression.Compile();
        }

        /// <summary>
        /// Evaluates the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public object Evaluate(params object[] args)
        {
            return this.func.DynamicInvoke(args);
        }
    }
}