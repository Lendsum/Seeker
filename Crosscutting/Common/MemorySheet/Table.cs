using System;
using System.Collections.Generic;
using System.Linq;

namespace Lendsum.Crosscutting.Common.MemorySheet
{
    /// <summary>
    /// Table to containt columns and row
    /// </summary>
    public class Table
    {
        private Dictionary<string, Column> columns = new Dictionary<string, Column>();
        private Dictionary<int, Dictionary<string, Cell>> cells = new Dictionary<int, Dictionary<string, Cell>>();
        private Dictionary<int, Dictionary<string, Depencency>> dependencies = new Dictionary<int, Dictionary<string, Depencency>>();

        /// <summary>
        /// Gets the <see cref="Column"/> with the specified name.
        /// </summary>
        /// <value>
        /// The <see cref="Column"/>.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Column this[string name]
        {
            get
            {
                return this.columns[name];
            }
        }

        /// <summary>
        /// Gets the cells at the specified index row
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public IDictionary<string, Cell> this[int index]
        {
            get
            {
                return this.cells[index];
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="object"/> with the specified column.
        /// </summary>
        /// <value>
        /// The <see cref="object"/>.
        /// </value>
        /// <param name="column">The column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1023:IndexersShouldNotBeMultidimensional", Justification = "By design, columns are strings and rows intenger")]
        public object this[string column, int index]
        {
            set
            {
                var cell = this.GetCell(column, index);
                if (cell == null) this.AddUpdateCell(new Cell(this, new Position(column, index)));
                this.cells[index][column].Value = value;

                DismissDependableValues(column, index);
            }
            get
            {
                // check if the value is already calculated and then return.
                var dependency = this.GetDependency(column, index);
                if (dependency.Available) return dependency.CurrentValue;

                // evaluate the cell
                var cell = this.GetCell(column, index);
                if (cell == null) this.AddUpdateCell(new Cell(this, new Position(column, index)));
                var newValue = this.cells[index][column].Value;

                // save the current value;
                dependency.CurrentValue = newValue;

                // mark which cell can affect this value.
                MarkCellDependencies(column, index);
                return newValue;
            }
        }

        /// <summary>
        /// Gets the <see cref="Cell"/> with the specified column and row index
        /// </summary>
        /// <value>
        /// The <see cref="Cell"/>.
        /// </value>
        /// <param name="column">The column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public Cell GetCell(string column, int index)
        {
            if (this.cells.ContainsKey(index))
            {
                if (this.cells[index].ContainsKey(column))
                {
                    return this.cells[index][column];
                }
            }

            Cell cell = new Cell(this, new Position(column, index));
            AddUpdateCell(cell);
            return cell;
        }

        /// <summary>
        /// Adds the update cell.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.ArgumentException">column doesnt exists</exception>
        public void AddUpdateCell(Cell value)
        {
            if (value == null) return;
            var column = value.Position.Column;
            var index = value.Position.Row;

            if (!this.columns.ContainsKey(column))
            {
                throw new ArgumentException("column doesnt exists");
            }
            if (!this.cells.ContainsKey(index))
            {
                this.cells.Add(index, new Dictionary<string, Cell>());
            }

            if (!this.cells[index].ContainsKey(column))
            {
                this.cells[index].Add(column, value);
            }
            else
            {
                this.cells[index][column] = value;
            }
        }

        /// <summary>
        /// Adds the column.
        /// </summary>
        /// <param name="column">The column.</param>
        public void AddColumn(Column column)
        {
            if (column == null) throw new ArgumentNullException("column");
            Position.CheckColumnName(column.Name);

            if (columns.ContainsKey(column.Name)) throw new ArgumentException("The table already has a column with that name");
            this.columns.Add(column.Name, column);
        }

        /// <summary>
        /// Adds the columns.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void AddColumns(IEnumerable<Column> columns)
        {
            if (columns == null) return;
            foreach (var column in columns)
            {
                this.AddColumn(column);
            }
        }

        private Depencency GetDependency(string column, int row)
        {
            if (!this.dependencies.ContainsKey(row)) this.dependencies.Add(row, new Dictionary<string, Depencency>());
            if (!this.dependencies[row].ContainsKey(column)) this.dependencies[row].Add(column, new Depencency());
            return this.dependencies[row][column];
        }

        private void MarkCellDependencies(string column, int row)
        {
            var cell = this.cells[row]?[column];
            var dependencies = cell?.GetDependencies();
            if (dependencies == null) return;
            foreach (var dep in dependencies)
            {
                this.GetDependency(dep.Column, dep.Row).MarkDependency(cell.Position);
            }
        }

        private void DismissDependableValues(string column, int row)
        {
            var dependency = GetDependency(column, row);
            dependency.Dismiss();
            var dependencies = dependency.CellsDependsOnThisValue;
            if (dependencies == null || dependencies.Count() == 0) return;

            foreach (var dep in dependencies)
            {
                this.GetDependency(dep.Column, dep.Row).Dismiss();
            }
        }
    }
}