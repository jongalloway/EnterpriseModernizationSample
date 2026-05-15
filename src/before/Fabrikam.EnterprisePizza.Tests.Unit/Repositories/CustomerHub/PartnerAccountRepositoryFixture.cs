using Fabrikam.EnterprisePizza.Data.Repositories.CustomerHub;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Repositories.CustomerHub
{
    [TestFixture]
    public class PartnerAccountRepositoryFixture
    {
        [Test]
        public void GetPreferredPartners_returns_partner_names_from_preferred_partners_result_set()
        {
            var repository = new PartnerAccountRepository();

            var partners = repository.GetPreferredPartners();

            Assert.That(partners, Is.EqualTo(new[]
            {
                "Contoso Office Parks",
                "Northwind Youth Sports League",
                "Adventure Works Bike Expo"
            }));
        }
    }
}
