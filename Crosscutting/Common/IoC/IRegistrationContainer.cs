using Autofac;

namespace Lendsum.Crosscutting.Common.IoC
{
    /// <summary>
    /// Interface to be implemented by classes in charge of register dependencies in the ioc container.
    /// </summary>
    public interface IRegistrationContainer
    {
        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <param name="builder">The builder.</param>
        void Register(ContainerBuilder builder);
    }
}