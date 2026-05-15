using Fabrikam.EnterprisePizza.Business.CustomerHub.Services;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Services
{
    [TestFixture]
    public class PartnerAccountServiceFixture
    {
        [Test]
        public void GetPreferredPartners_returns_seeded_partner_names()
        {
            var service = new PartnerAccountService();

            var partners = service.GetPreferredPartners();

            Assert.That(partners, Has.Count.EqualTo(3));
            Assert.That(partners, Does.Contain("Contoso Office Parks"));
            Assert.That(partners, Does.Contain("Adventure Works Bike Expo"));
        }
    }
}
