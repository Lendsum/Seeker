namespace Lendsum.Crosscutting.Common
{
    /// <summary>
    /// Interface to be implemented by DateTimeProvider
    /// It to wrapp DateTime structure and static methods like UtcNow. It can facilitate the mocking of those methods
    /// </summary>
    public interface IRandomGenerator
    {
        /// <summary>
        /// Nexts the specified maximum value.
        /// </summary>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns></returns>
        int Next(int maxValue);
    }
}