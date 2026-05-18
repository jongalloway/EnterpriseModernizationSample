using System;

namespace Fabrikam.EnterprisePizza.Core.Composition
{
    public interface ILegacyServiceRegistry
    {
        void Register<TService>() where TService : class;

        void Register<TService>(LegacyLifetime lifetime) where TService : class;

        void Register<TFrom, TTo>()
            where TFrom : class
            where TTo : class, TFrom;

        void Register<TFrom, TTo>(LegacyLifetime lifetime)
            where TFrom : class
            where TTo : class, TFrom;

        void Register(Type serviceType, Type mapToType, string name, LegacyLifetime lifetime);
    }
}
