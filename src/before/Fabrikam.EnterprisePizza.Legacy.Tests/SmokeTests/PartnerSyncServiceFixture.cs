using System.IO;
using System.Reflection;
using System.Web.Services;
using System.Web.Services.Protocols;
using Fabrikam.EnterprisePizza.Services.PartnerSync;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Legacy.Tests.SmokeTests
{
    [TestFixture]
    public class PartnerSyncServiceFixture
    {
        [Test]
        public void GetPreferredPartners_returns_the_expected_partner_catalog()
        {
            var service = new PartnerSyncService();

            var partners = service.GetPreferredPartners();

            Assert.That(partners, Is.EqualTo(new[]
            {
                "Contoso Office Parks",
                "Northwind Youth Sports League",
                "Adventure Works Bike Expo"
            }));
        }

        [Test]
        public void PartnerSync_exposes_asmx_host_directive_binding_and_http_protocols()
        {
            var serviceAttribute = typeof(PartnerSyncService).GetCustomAttribute<WebServiceAttribute>();
            Assert.That(serviceAttribute, Is.Not.Null);
            Assert.That(serviceAttribute.Namespace, Is.EqualTo("http://fabrikam.com/pizza/partners/"));

            var bindingAttribute = typeof(PartnerSyncService).GetCustomAttribute<WebServiceBindingAttribute>();
            Assert.That(bindingAttribute, Is.Not.Null);
            Assert.That(bindingAttribute.ConformsTo, Is.EqualTo(WsiProfiles.BasicProfile1_1));

            var soapDocumentAttribute = typeof(PartnerSyncService).GetCustomAttribute<SoapDocumentServiceAttribute>();
            Assert.That(soapDocumentAttribute, Is.Not.Null);
            Assert.That(soapDocumentAttribute.RoutingStyle, Is.EqualTo(SoapServiceRoutingStyle.RequestElement));

            var operation = typeof(PartnerSyncService).GetMethod("GetPreferredPartners");
            Assert.That(operation, Is.Not.Null);
            Assert.That(operation.GetCustomAttribute<WebMethodAttribute>(), Is.Not.Null);

            var hostDirective = File.ReadAllText(LegacyServiceSmokePaths.Combine("Fabrikam.EnterprisePizza.Services.PartnerSync", "PartnerSync.asmx"));
            Assert.That(hostDirective, Does.Contain("Class=\"Fabrikam.EnterprisePizza.Services.PartnerSync.PartnerSyncService\""));

            var webConfig = File.ReadAllText(LegacyServiceSmokePaths.Combine("Fabrikam.EnterprisePizza.Services.PartnerSync", "web.config"));
            Assert.That(webConfig, Does.Contain("<add key=\"PartnerSyncSourceSystem\" value=\"CustomerHub.PartnerSync\" />"));
            Assert.That(webConfig, Does.Contain("<add name=\"HttpGet\" />"));
            Assert.That(webConfig, Does.Contain("<add name=\"HttpPost\" />"));
        }
    }
}
