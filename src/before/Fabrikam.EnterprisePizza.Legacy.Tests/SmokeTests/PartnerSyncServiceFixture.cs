using Fabrikam.EnterprisePizza.Services.PartnerSync;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Legacy.Tests.SmokeTests
{
    [TestFixture]
    public class PartnerSyncServiceFixture
    {
        [Test]
        public void GetPreferredPartners_returns_seeded_partner_names()
        {
            var service = new PartnerSyncService();

            var partners = service.GetPreferredPartners();

            Assert.That(partners, Has.Count.EqualTo(3));
            Assert.That(partners, Does.Contain("Contoso Office Parks"));
            Assert.That(partners, Does.Contain("Northwind Youth Sports League"));
            Assert.That(partners, Does.Contain("Adventure Works Bike Expo"));
        }
    }
}
