using Autofac;
using Lendsum.Crosscutting.Common.IoC;
using Lendsum.Infrastructure.Core.DelayCalls;
using Lendsum.Infrastructure.Core.Dispatcher;
using Lendsum.Infrastructure.Core.Locks;
using Lendsum.Infrastructure.Core.Persistence;
using Lendsum.Infrastructure.Core.Persistence.BlobFileStorage;
using Lendsum.Infrastructure.Core.Persistence.RabbitMQPersistence;
using Lendsum.Infrastructure.Core.Persistence.SqlServerPersistence;
using Lendsum.Infrastructure.Core.Queues;
using Lendsum.Infrastructure.Core.Scheduler;
using System.Configuration;
using System.Reflection;

namespace Lendsum.Infrastructure.Core
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
            RegisterPersistenceProviders(builder);
            RegisterInfrastructure(builder);
            RegisterContexts(builder);
            RegisterUtils(builder);
        }

        private static void RegisterUtils(ContainerBuilder builder)
        {
            // utils
            builder.RegisterGeneric(typeof(LazyLoader<>)).As(typeof(ILazyLoader<>)).SingleInstance();
            builder.RegisterGeneric(typeof(LazyInContextLoader<>)).As(typeof(ILazyInContextLoader<>)).SingleInstance();
        }

        private static void RegisterInfrastructure(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // infrastructure.
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<UnitOfWorkQueueSender>().As<IUnitOfWorkQueueSender>().SingleInstance();

            // loaders and savers.
            builder.RegisterType<AggregateLoader>().As<IAggregateLoader>().SingleInstance();
            builder.RegisterType<AggregateSnapShoter>().As<IAggregateSnapShoter>().SingleInstance();

            // dispachers
            builder.RegisterType<DispatcherHub>().As<IDispatcherHub>().SingleInstance();

            builder.RegisterAssemblyTypes(assembly).AssignableTo<IAsyncListener>().As<IAsyncListener>().SingleInstance();

            // locker
            builder.RegisterType<Locker>().As<ILocker>().SingleInstance();

            RegisterDelayedInfrastructure(builder, assembly);

            // scheduler
            builder.RegisterType<SchedulerLauncher>().As<ISchedulerLauncher>().SingleInstance();
        }

        private static void RegisterDelayedInfrastructure(ContainerBuilder builder, Assembly assembly)
        {
            // Delayer
            builder.RegisterGeneric(typeof(Delayer<>)).As(typeof(IDelayer<>)).SingleInstance();
            builder.RegisterAssemblyTypes(assembly).AssignableTo<IDelayed>().As<IDelayed>().SingleInstance();
            builder.RegisterType<DelayedCallsHub>().As<IDelayedCallsHub>().SingleInstance();
        }

        private static void RegisterPersistenceProviders(ContainerBuilder builder)
        {
            // persistence
            // check what provider should we use, if sql server is available, we will choose that, if not, couchbse.
            builder.RegisterType<SqlServerProvider>().As<IPersistenceProvider>().SingleInstance();
            
            builder.RegisterType<RabbitWrapper>().As<IQueueFactory>().SingleInstance();

            //var config = BlobFileConfiguration.ReadConfiguration();
            //if (config == null || string.IsNullOrWhiteSpace(config.AzureConnectionString))
            //{
            //    builder.RegisterType<BlobFileService>().As<IBlobService>().SingleInstance();
            //}
            //else
            //{
            //    builder.RegisterType<AzureBlobWrapper>().As<IBlobService>().SingleInstance();
            //}
        }

        private static void RegisterContexts(ContainerBuilder builder)
        {
            // context
            builder.RegisterType<AggregateEventContext>().As<IAggregateEventContext>().SingleInstance();
            builder.RegisterType<UnitOfWorkContext>().As<IUnitOfWorkContext>().SingleInstance();
        }
    }
}