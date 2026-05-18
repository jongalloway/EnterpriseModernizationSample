using System.Collections.ObjectModel;
using Fabrikam.EnterprisePizza.Reporting.Batch.Configuration;
using Fabrikam.EnterprisePizza.Reporting.Batch.Models;

namespace Fabrikam.EnterprisePizza.Reporting.Batch
{
    public static class ReportCatalog
    {
        public static readonly ReportDefinition DeliveryPerformance = new ReportDefinition(
            "delivery-performance",
            "Delivery Performance",
            "/Fabrikam Enterprise Pizza/Operations/DeliveryPerformance",
            "Reports\\Definitions\\DeliveryPerformance.rdl",
            "/Fabrikam Enterprise Pizza/Shared Data Sources/FabrikamPizza_Reporting");

        public static readonly ReportDefinition StoreOperationsSummary = new ReportDefinition(
            "store-operations-summary",
            "Store Operations Summary",
            "/Fabrikam Enterprise Pizza/Operations/StoreOperationsSummary",
            "Reports\\Definitions\\StoreOperationsSummary.rdl",
            "/Fabrikam Enterprise Pizza/Shared Data Sources/FabrikamPizza_Reporting");

        public static readonly ReportDefinition PartnerProfitability = new ReportDefinition(
            "partner-profitability",
            "Partner Profitability",
            "/Fabrikam Enterprise Pizza/Operations/PartnerProfitability",
            "Reports\\Definitions\\PartnerProfitability.rdl",
            "/Fabrikam Enterprise Pizza/Shared Data Sources/FabrikamPizza_Reporting");

        public static readonly ReadOnlyCollection<ReportDefinition> All = new ReadOnlyCollection<ReportDefinition>(
            new[]
            {
                DeliveryPerformance,
                StoreOperationsSummary,
                PartnerProfitability
            });

        public static string ResolveServerPath(ReportingServiceConfiguration configuration, string reportName)
        {
            return ReportingServiceConfiguration.NormalizeServerPath(configuration.ReportFolder + "/" + reportName);
        }
    }
}
