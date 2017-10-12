namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Interface to be implemented by a command
    /// </summary>
    public interface ICommand<T>
    {
        /// <summary>
        /// Executes this command with the specific data.
        /// </summary>
        /// <param name="data">The data.</param>
        void Execute(T data);
    }
}