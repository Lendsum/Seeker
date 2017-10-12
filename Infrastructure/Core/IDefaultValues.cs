namespace Lendsum.Infrastructure.Core
{
    /// <summary>
    /// Interface to be implemented by clases that will create default values in the start-up
    /// </summary>
    public interface IDefaultValues
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Init();
    }
}