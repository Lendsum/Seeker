using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lendsum.Crosscutting.Common.MemorySheet
{
    /// <summary>
    /// Class to represent a position of a cell.
    /// </summary>
    public class Position
    {
        /// <summary>
        /// The position splitter
        /// </summary>
        public const char PositionSplitter = '_';

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> class.
        /// </summary>
        /// <param name="raw">The raw.</param>
        public Position(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) throw new ArgumentException("The position must not represent a cell or range of cells");
            var parts = raw.Split(PositionSplitter);
            if (parts.Count() > 0)
            {
                CheckColumnName(parts[0]);
                this.Column = parts[0];
                this.RowRelative = true;
            }

            if (parts.Count() > 1)
            {
                this.ParseRowNumber(parts[1]);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="index">The index.</param>
        public Position(string column, int index)
        {
            CheckColumnName(column);
            if (index < 0) throw new ArgumentException("The index must be equal or greater than zero");
            this.Column = column;
            this.Row = index;
        }

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        /// <value>
        /// The column.
        /// </value>
        public string Column { get; protected set; }

        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        /// <value>
        /// The row.
        /// </value>
        public int Row { get; protected set; }

        /// <summary>
        /// Gets a value indicatingif the row is relative or absolute
        /// </summary>
        public bool RowRelative { get; protected set; }

        private void ParseRowNumber(string name)
        {
            CheckRowName(name);
            int number = int.Parse(name.Replace("P", "").Replace("N", ""), CultureInfo.InvariantCulture);
            if (name.Contains('P'))
            {
                this.Row = number;
                this.RowRelative = true;
            }
            else if (name.Contains('N'))
            {
                this.Row = number * -1;
                this.RowRelative = true;
            }
            else
            {
                this.Row = number;
            }
        }

        /// <summary>
        /// Checks the name of the column.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentException">The name is not a column name, column must be composed only by letters</exception>
        public static void CheckColumnName(string name)
        {
            if (name.Any(x => !char.IsLetter(x))) throw new ArgumentException("The name is not a column name, column must be composed only by letters");
        }

        /// <summary>
        /// Checks the name of the row.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentException">The name is not a row name, row must be composed only by number followed by N or P if its a relative position</exception>
        public static void CheckRowName(string name)
        {
            if (!Regex.IsMatch(name, @"\d+(P|N)?")) throw new ArgumentException("The name is not a row name, row must be composed only by number followed by N or P if its a relative position");
        }
    }
}