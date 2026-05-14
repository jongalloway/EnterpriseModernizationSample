namespace Fabrikam.EnterprisePizza.Data.Gateways
{
    public class LegacyDbGateway
    {
        public string GetConnectionName(string area)
        {
            return "FabrikamPizza_" + area;
        }
    }
}
