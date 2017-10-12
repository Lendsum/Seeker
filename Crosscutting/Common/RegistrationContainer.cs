using Autofac;
using Lendsum.Crosscutting.Common.Hash;
using Lendsum.Crosscutting.Common.IoC;
using Lendsum.Crosscutting.Common.Providers;
using Lendsum.Crosscutting.Common.Serialization;

namespace Lendsum.Crosscutting.Common
{
    /// <summary>
    /// Registration container of Crosscutting.Common classes.
    /// </summary>
    public class RegistrationContainer : IRegistrationContainer
    {
        /// <summary>
        /// Registers this instance.
        /// </summary>
        public void Register(ContainerBuilder builder)
        {
            //builder.RegisterGeneric(typeof(Cache<>)).As(typeof(ICache<>)).SingleInstance();
            builder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>().SingleInstance();
            builder.RegisterType<HashService>().As<IHashService>().SingleInstance();
            builder.RegisterType<JsonTextSerializer>().As<ITextSerializer>().SingleInstance();
            builder.RegisterType<DownloaderService>().As<IDownloaderService>().SingleInstance();
            builder.RegisterType<ThreadContext>().As<IThreadContext>().SingleInstance();
            builder.RegisterType<RandomGenerator>().As<IRandomGenerator>().SingleInstance();
        }
    }
}