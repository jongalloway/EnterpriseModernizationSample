using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Fabrikam.EnterprisePizza.Core.Composition
{
    public class UnityServiceLocatorAdapter : IServiceLocator
    {
        private readonly IUnityContainer _container;

        public UnityServiceLocatorAdapter(IUnityContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public object GetInstance(Type serviceType)
        {
            return _container.Resolve(serviceType);
        }

        public object GetInstance(Type serviceType, string key)
        {
            return string.IsNullOrWhiteSpace(key)
                ? _container.Resolve(serviceType)
                : _container.Resolve(serviceType, key);
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.ResolveAll(serviceType).Cast<object>();
        }

        public TService GetInstance<TService>()
        {
            return _container.Resolve<TService>();
        }

        public TService GetInstance<TService>(string key)
        {
            return string.IsNullOrWhiteSpace(key)
                ? _container.Resolve<TService>()
                : _container.Resolve<TService>(key);
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return _container.ResolveAll<TService>();
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == null || !_container.IsRegistered(serviceType))
            {
                return null;
            }

            return _container.Resolve(serviceType);
        }
    }
}
