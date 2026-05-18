using System;
using Fabrikam.EnterprisePizza.Core.Configuration;
using Microsoft.Practices.Unity;

namespace Fabrikam.EnterprisePizza.Core.Composition
{
    public static class LegacyObjectBuilder
    {
        public static void ApplyRegistrations(IUnityContainer container, EnterpriseLibraryConfigurationReader configurationReader, string containerName)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (configurationReader == null)
            {
                throw new ArgumentNullException(nameof(configurationReader));
            }

            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentException("A container name is required.", nameof(containerName));
            }

            foreach (var registration in configurationReader.ReadContainerRegistrations(containerName))
            {
                var serviceType = configurationReader.ResolveType(registration.ServiceTypeToken);
                var implementationType = configurationReader.ResolveType(registration.MapToTypeToken);
                var lifetimeManager = CreateLifetimeManager(registration.Lifetime);

                if (string.IsNullOrWhiteSpace(registration.Name))
                {
                    container.RegisterType(serviceType, implementationType, lifetimeManager);
                    continue;
                }

                container.RegisterType(serviceType, implementationType, registration.Name, lifetimeManager);
            }
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
