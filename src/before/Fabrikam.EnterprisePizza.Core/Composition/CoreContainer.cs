using System;
using Fabrikam.EnterprisePizza.Core.Services;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Fabrikam.EnterprisePizza.Core.Composition
{
    public static class CoreContainer
    {
        public static IUnityContainer CreateConfiguredContainer()
        {
            var container = new UnityContainer();
            Register(container);
            return container;
        }

        public static void Register(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.RegisterType<IBusinessClock, SystemBusinessClock>(new ContainerControlledLifetimeManager());
            container.RegisterType<IOrderFulfillmentPolicy, LegacyOrderFulfillmentPolicy>(new TransientLifetimeManager());
        }

        public static IServiceLocator InitializeServiceLocator(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (!container.IsRegistered<IBusinessClock>() || !container.IsRegistered<IOrderFulfillmentPolicy>())
            {
                throw new InvalidOperationException("Core services must be registered before initializing the service locator. Call CreateConfiguredContainer() or Register(container) first.");
            }

            var locator = new UnityServiceLocatorAdapter(container);
            ServiceLocator.SetLocatorProvider(() => locator);
            return locator;
        }
    }
}
