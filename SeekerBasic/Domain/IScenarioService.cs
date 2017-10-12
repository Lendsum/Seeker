using SeekerBasic.Domain.Dtos;

namespace SeekerBasic.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public interface IScenarioService
    {
        /// <summary>
        /// Gets the sceneario.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        ScenarioAggregate GetSceneario(int id);

        /// <summary>
        /// Moves the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="direction">The direction.</param>
        /// <returns></returns>
        ScenarioAggregate Move(int id, DirectionEnum direction);

        /// <summary>
        /// Resets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        ScenarioAggregate Reset(int id);
    }
}