namespace Lendsum.Crosscutting.Common.MemorySheet
{
    /// <summary>
    /// Interface to be implemented by evaluable objects.
    /// </summary>
    public interface IEvaluable
    {
        /// <summary>
        /// Evaluates this instance.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        object Evaluate(params object[] args);

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "We are locking for performance, better an array")]
        Position[] Parameters { get; }
    }
}