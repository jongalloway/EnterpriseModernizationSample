namespace Fabrikam.EnterprisePizza.Core.Composition
{
    public interface ILegacyServiceRegistry
    {
        void Register<TService>() where TService : class;

        void Register<TFrom, TTo>()
            where TFrom : class
            where TTo : class, TFrom;
    }
}
