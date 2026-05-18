namespace Fabrikam.EnterprisePizza.Data.StoredProcedures
{
    public static class LegacyStoredProcedures
    {
        public static class StoreOps
        {
            public const string GetActiveDispatchTickets = "dbo.usp_DispatchBoard_GetActiveTickets";
            public const string GetOrder = "dbo.usp_OrderService_GetOrder";
            public const string SearchOrders = "dbo.usp_OrderService_SearchOrders";
            public const string PlaceOrder = "dbo.usp_OrderService_PlaceOrder";
            public const string UpdateOrderStatus = "dbo.usp_OrderService_UpdateOrderStatus";
            public const string GetOrderHistory = "dbo.usp_OrderService_GetOrderHistory";
            public const string GetLatestPosImportBatch = "dbo.usp_PosImport_GetLatestBatch";
        }

        public static class CustomerHub
        {
            public const string GetPreferredPartners = "dbo.usp_CorporateAccounts_GetPreferredPartners";
            public const string GetPartnerProfile = "dbo.usp_PartnerAccounts_GetPartnerProfile";
            public const string RegisterPartner = "dbo.usp_PartnerAccounts_RegisterPartner";
            public const string UpdatePartnerContract = "dbo.usp_PartnerContracts_UpdateCurrent";
            public const string GetPartnerReferrals = "dbo.usp_PartnerReferrals_GetByPartner";
            public const string SubmitPartnerReferral = "dbo.usp_PartnerReferrals_Submit";
            public const string ProcessPartnerCommission = "dbo.usp_PartnerSettlements_ProcessCommission";
        }

        public static class Reporting
        {
            public const string GetDimStoreCatalog = "dbo.usp_ReportingDimStore_GetCatalog";
            public const string GetDimDriverCatalog = "dbo.usp_ReportingDimDriver_GetCatalog";
            public const string GetDimPartnerCatalog = "dbo.usp_ReportingDimPartner_GetCatalog";
            public const string GetDimTimeRange = "dbo.usp_ReportingDimTime_GetRange";
            public const string GetFactDeliveryByDate = "dbo.usp_ReportingFactDelivery_GetByDate";
            public const string GetFactOrderByDate = "dbo.usp_ReportingFactOrder_GetByDate";
            public const string GetFactPartnerRevenueByMonth = "dbo.usp_ReportingFactPartnerRevenue_GetByMonth";
            public const string GetDeliveryStoreDriverSnapshot = "dbo.usp_ReportingStar_GetDeliveryStoreDriverSnapshot";
            public const string GetOrderChannelMixSnapshot = "dbo.usp_ReportingStar_GetOrderChannelMixSnapshot";
            public const string GetPartnerRevenueSettlementSnapshot = "dbo.usp_ReportingStar_GetPartnerRevenueSettlementSnapshot";
            public const string GetLaborCostSummary = "dbo.usp_WorkforceReports_GetLaborCostSummary";
            public const string GetOvertimeTrend = "dbo.usp_WorkforceReports_GetOvertimeTrend";
            public const string GetTurnoverSummary = "dbo.usp_WorkforceReports_GetTurnoverSummary";
            public const string GetStaffingSummary = "dbo.usp_WorkforceReports_GetStaffingSummary";
        }
    }
}
