using System;
using Microsoft.Practices.Unity;

namespace Fabrikam.EnterprisePizza.Core.Composition
{
    public static class LegacyServiceLocator
    {
        private static readonly object SyncRoot = new object();
        private static IUnityContainer _container;

        public static void Initialize(Action<ILegacyServiceRegistry> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            lock (SyncRoot)
            {
                var container = new UnityContainer();
                configure(new UnityLegacyServiceRegistry(container));
                _container = container;
            }
        }

        public static T Resolve<T>()
        {
            var container = _container;
            if (container == null)
            {
                throw new InvalidOperationException("The legacy service locator has not been initialized for this host.");
            }

            return container.Resolve<T>();
        }

        private sealed class UnityLegacyServiceRegistry : ILegacyServiceRegistry
        {
            private readonly IUnityContainer _container;

            public UnityLegacyServiceRegistry(IUnityContainer container)
            {
                _container = container ?? throw new ArgumentNullException(nameof(container));
            }

            public void Register<TService>() where TService : class
            {
                _container.RegisterType<TService>();
            }

            public void Register<TFrom, TTo>()
                where TFrom : class
                where TTo : class, TFrom
            {
                _container.RegisterType<TFrom, TTo>();
            }
        }
    }
}
