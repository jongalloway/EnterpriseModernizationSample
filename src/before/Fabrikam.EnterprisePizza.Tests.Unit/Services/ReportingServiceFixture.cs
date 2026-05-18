using System;
using System.Linq;
using Fabrikam.EnterprisePizza.Reporting.Batch;
using Fabrikam.EnterprisePizza.Reporting.Batch.Configuration;
using Fabrikam.EnterprisePizza.Reporting.Batch.Models;
using Fabrikam.EnterprisePizza.Reporting.Batch.Services;
using NUnit.Framework;

namespace Fabrikam.EnterprisePizza.Tests.Unit.Services
{
    [TestFixture]
    public class ReportingServiceFixture
    {
        private static readonly Uri ExecutionEndpoint = new Uri("http://reports.fabrikam.com/ReportServer/ReportExecution2005.asmx");

        [Test]
        public void CreateExecutionRequest_uses_catalog_path_render_format_and_parameters()
        {
            var service = CreateService();
            var parameters = new DeliveryPerformanceReportParameters
            {
                StoreNumber = "014",
                SummaryDate = new DateTime(2026, 5, 18),
                MinimumCompletedRuns = 2
            };

            var request = service.CreateExecutionRequest(ReportCatalog.DeliveryPerformance, parameters);

            Assert.That(request.ExecutionEndpoint, Is.EqualTo(ExecutionEndpoint));
            Assert.That(request.ReportPath, Is.EqualTo("/Fabrikam Enterprise Pizza/Operations/DeliveryPerformance"));
            Assert.That(request.RenderFormat, Is.EqualTo("PDF"));
            Assert.That(request.Parameters.Select(parameter => parameter.Name), Is.EqualTo(new[] { "StoreNumber", "SummaryDate", "MinimumCompletedRuns" }));
            Assert.That(request.Parameters.Select(parameter => parameter.Value), Is.EqualTo(new[] { "014", "2026-05-18", "2" }));
        }

        [Test]
        public void BuildSetExecutionParametersSoapEnvelope_includes_execution_header_and_parameters()
        {
            var service = CreateService();
            var envelope = service.BuildSetExecutionParametersSoapEnvelope(
                "execution-123",
                new PartnerProfitabilityReportParameters
                {
                    SummaryDate = new DateTime(2026, 5, 18),
                    PartnerCode = "CORP-1002",
                    MinimumGrossSales = 250m
                }.ToReportParameters());

            Assert.That(envelope, Does.Contain("execution-123"));
            Assert.That(envelope, Does.Contain("PartnerCode"));
            Assert.That(envelope, Does.Contain("CORP-1002"));
            Assert.That(envelope, Does.Contain("en-US"));
        }

        [Test]
        public void BuildRenderSoapEnvelope_includes_device_info_and_render_format()
        {
            var service = CreateService();
            var request = service.CreateExecutionRequest(ReportCatalog.StoreOperationsSummary, new StoreOperationsSummaryReportParameters());

            var envelope = service.BuildRenderSoapEnvelope(request, "execution-456");

            Assert.That(envelope, Does.Contain("execution-456"));
            Assert.That(envelope, Does.Contain("PDF"));
            Assert.That(envelope, Does.Contain("&lt;DeviceInfo&gt;&lt;Toolbar&gt;False&lt;/Toolbar&gt;&lt;/DeviceInfo&gt;"));
        }

        [Test]
        public void Parameter_models_reject_invalid_ranges()
        {
            Assert.Throws<InvalidOperationException>(() =>
                new StoreOperationsSummaryReportParameters
                {
                    StartDate = new DateTime(2026, 5, 18),
                    EndDate = new DateTime(2026, 5, 17)
                }.ToReportParameters().ToList());

            Assert.Throws<InvalidOperationException>(() =>
                new PartnerProfitabilityReportParameters
                {
                    MinimumGrossSales = -1m
                }.ToReportParameters().ToList());
        }

        [Test]
        public void Constructor_rejects_non_report_execution_endpoint()
        {
            Assert.Throws<ArgumentException>(() =>
                new ReportingServiceConfiguration(new Uri("http://reports.fabrikam.com/ReportServer/ReportService2005.asmx"), "/Fabrikam Enterprise Pizza/Operations", "PDF", 90, true));
        }

        private static ReportingService CreateService()
        {
            return new ReportingService(
                new ReportingServiceConfiguration(ExecutionEndpoint, "/Fabrikam Enterprise Pizza/Operations", "PDF", 90, true),
                new[]
                {
                    new ReportingDataSourceConfiguration(
                        "FabrikamPizza_Reporting",
                        "SQL",
                        "Data Source=SQLLEGACY01;Initial Catalog=FabrikamPizza_Reporting;Integrated Security=SSPI;",
                        "/Fabrikam Enterprise Pizza/Shared Data Sources/FabrikamPizza_Reporting",
                        true,
                        string.Empty)
                });
        }
    }
}
