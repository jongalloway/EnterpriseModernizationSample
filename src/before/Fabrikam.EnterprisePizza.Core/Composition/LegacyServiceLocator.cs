using System;
using Fabrikam.EnterprisePizza.Core.Configuration;
using Fabrikam.EnterprisePizza.Core.ExceptionHandling;
using Fabrikam.EnterprisePizza.Core.Logging;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Fabrikam.EnterprisePizza.Core.Composition
{
    public static class LegacyServiceLocator
    {
        private static readonly object SyncRoot = new object();
        private static IUnityContainer container;

        public static bool IsInitialized
        {
            get { return container != null; }
        }

        public static void Initialize(Action<ILegacyServiceRegistry> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            lock (SyncRoot)
            {
                var configuredContainer = CoreContainer.CreateConfiguredContainer();
                configure(new UnityLegacyServiceRegistry(configuredContainer));
                FinalizeInitialization(configuredContainer, null);
            }
        }

        public static void InitializeFromConfiguration(string containerName)
        {
            Initialize(new EnterpriseLibraryConfigurationReader(), containerName);
        }

        public static void Initialize(EnterpriseLibraryConfigurationReader configurationReader, string containerName)
        {
            if (configurationReader == null)
            {
                throw new ArgumentNullException(nameof(configurationReader));
            }

            var resolvedContainerName = string.IsNullOrWhiteSpace(containerName)
                ? configurationReader.GetDefaultContainerName("LegacyServices")
                : containerName.Trim();

            lock (SyncRoot)
            {
                var configuredContainer = CoreContainer.CreateConfiguredContainer();
                LegacyObjectBuilder.ApplyRegistrations(configuredContainer, configurationReader, resolvedContainerName);
                FinalizeInitialization(configuredContainer, configurationReader);
            }
        }

        public static T Resolve<T>()
        {
            var currentContainer = container;
            if (currentContainer == null)
            {
                throw new InvalidOperationException("The legacy service locator has not been initialized for this host.");
            }

            return currentContainer.Resolve<T>();
        }

        public static object Resolve(Type serviceType)
        {
            var currentContainer = container;
            if (currentContainer == null)
            {
                throw new InvalidOperationException("The legacy service locator has not been initialized for this host.");
            }

            return currentContainer.Resolve(serviceType);
        }

        private static void FinalizeInitialization(IUnityContainer configuredContainer, EnterpriseLibraryConfigurationReader configurationReader)
        {
            container = configuredContainer;
            var locator = new UnityServiceLocatorAdapter(configuredContainer);
            ServiceLocator.SetLocatorProvider(() => locator);

            if (configurationReader == null)
            {
                LogWriter.SetCurrent(new LegacyLogWriter("Operations", "legacy-enterprise"));
                ExceptionPolicy.SetPolicies(null, "ServiceBoundaryPolicy");
                return;
            }

            LogWriter.Configure(configurationReader);
            ExceptionPolicy.Configure(configurationReader);
        }

        private sealed class UnityLegacyServiceRegistry : ILegacyServiceRegistry
        {
            private readonly IUnityContainer container;

            public UnityLegacyServiceRegistry(IUnityContainer container)
            {
                this.container = container ?? throw new ArgumentNullException(nameof(container));
            }

            public void Register<TService>() where TService : class
            {
                Register<TService>(LegacyLifetime.Transient);
            }

            public void Register<TService>(LegacyLifetime lifetime) where TService : class
            {
                container.RegisterType<TService>(CreateLifetimeManager(lifetime));
            }

            public void Register<TFrom, TTo>()
                where TFrom : class
                where TTo : class, TFrom
            {
                Register<TFrom, TTo>(LegacyLifetime.Transient);
            }

            public void Register<TFrom, TTo>(LegacyLifetime lifetime)
                where TFrom : class
                where TTo : class, TFrom
            {
                container.RegisterType<TFrom, TTo>(CreateLifetimeManager(lifetime));
            }

            public void Register(Type serviceType, Type mapToType, string name, LegacyLifetime lifetime)
            {
                if (serviceType == null)
                {
                    throw new ArgumentNullException(nameof(serviceType));
                }

                if (mapToType == null)
                {
                    throw new ArgumentNullException(nameof(mapToType));
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    container.RegisterType(serviceType, mapToType, CreateLifetimeManager(lifetime));
                    return;
                }

                container.RegisterType(serviceType, mapToType, name, CreateLifetimeManager(lifetime));
            }

            private static LifetimeManager CreateLifetimeManager(LegacyLifetime lifetime)
            {
                switch (lifetime)
                {
                    case LegacyLifetime.Singleton:
                        return new ContainerControlledLifetimeManager();
                    default:
                        return new TransientLifetimeManager();
                }
            }
        }
    }
}
