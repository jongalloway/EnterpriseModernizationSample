using System.IO;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Legacy.Tests.SmokeTests
{
    [TestFixture]
    public class ReportingBatchFixture
    {
        [Test]
        public void Reporting_batch_includes_ssrs_execution_config_and_shared_data_source()
        {
            var appConfig = File.ReadAllText(LegacyServiceSmokePaths.Combine("Fabrikam.EnterprisePizza.Reporting.Batch", "App.config"));
            Assert.That(appConfig, Does.Contain("ReportExecution2005.asmx"));
            Assert.That(appConfig, Does.Contain("FabrikamPizza_Reporting"));
            Assert.That(appConfig, Does.Contain("/Fabrikam Enterprise Pizza/Shared Data Sources/FabrikamPizza_Reporting"));

            var sharedDataSource = File.ReadAllText(LegacyServiceSmokePaths.Combine("Fabrikam.EnterprisePizza.Reporting.Batch", "Reports", "DataSources", "FabrikamPizza_Reporting.rds"));
            Assert.That(sharedDataSource, Does.Contain("Initial Catalog=FabrikamPizza_Reporting"));
            Assert.That(sharedDataSource, Does.Contain("<IntegratedSecurity>true</IntegratedSecurity>"));
        }

        [TestCase("DeliveryPerformance.rdl")]
        [TestCase("StoreOperationsSummary.rdl")]
        [TestCase("PartnerProfitability.rdl")]
        public void Report_definition_uses_shared_data_source_and_tablix_layout(string reportFileName)
        {
            var reportDefinition = File.ReadAllText(LegacyServiceSmokePaths.Combine("Fabrikam.EnterprisePizza.Reporting.Batch", "Reports", "Definitions", reportFileName));

            Assert.That(reportDefinition, Does.Contain("<DataSourceReference>/Fabrikam Enterprise Pizza/Shared Data Sources/FabrikamPizza_Reporting</DataSourceReference>"));
            Assert.That(reportDefinition, Does.Contain("<Tablix Name="));
            Assert.That(reportDefinition, Does.Contain("<CommandType>StoredProcedure</CommandType>"));
        }
    }
}
