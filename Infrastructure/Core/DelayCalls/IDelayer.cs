namespace Lendsum.Infrastructure.Core.DelayCalls
{
    /// <summary>
    /// interface to be implemented by Idelayer caller
    /// It works with unit of work so must be commited.
    /// </summary>
    /// <typeparam name="T">parameters of the method to be delayed.</typeparam>
    public interface IDelayer<T> where T : class
    {
        /// <summary>
        /// Calls the method who can handle the data passed by parameters
        /// </summary>
        /// <param name="data">The data that will receive the method called delayed.</param>
        void Call(T data);
    }
}