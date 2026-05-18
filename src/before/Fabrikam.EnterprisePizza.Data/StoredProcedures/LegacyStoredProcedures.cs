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
            public const string GetLaborCostSummary = "dbo.usp_WorkforceReports_GetLaborCostSummary";
            public const string GetOvertimeTrend = "dbo.usp_WorkforceReports_GetOvertimeTrend";
            public const string GetTurnoverSummary = "dbo.usp_WorkforceReports_GetTurnoverSummary";
            public const string GetStaffingSummary = "dbo.usp_WorkforceReports_GetStaffingSummary";
        }
    }
}
