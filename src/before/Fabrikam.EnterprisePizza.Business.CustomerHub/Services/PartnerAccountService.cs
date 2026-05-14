using System.Collections.Generic;

namespace Fabrikam.EnterprisePizza.Business.CustomerHub.Services
{
    public class PartnerAccountService
    {
        public IList<string> GetPreferredPartners()
        {
            return new List<string>
            {
                "Contoso Office Parks",
                "Northwind Youth Sports League",
                "Adventure Works Bike Expo"
            };
        }
    }
}
