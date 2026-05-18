namespace Fabrikam.EnterprisePizza.Data.StoredProcedures
{
    public static class LegacyStoredProcedures
    {
        public static class StoreOps
        {
            public const string GetActiveDispatchTickets = "dbo.usp_DispatchBoard_GetActiveTickets";
            public const string GetLatestPosImportBatch = "dbo.usp_PosImport_GetLatestBatch";
        }

        public static class CustomerHub
        {
            public const string GetPreferredPartners = "dbo.usp_CorporateAccounts_GetPreferredPartners";
            public const string GetPartners = "dbo.usp_CustomerHubPartners_GetActive";
            public const string GetPartnerByCode = "dbo.usp_CustomerHubPartners_GetByCode";
            public const string SavePartner = "dbo.usp_CustomerHubPartners_Save";
            public const string DeletePartner = "dbo.usp_CustomerHubPartners_Delete";
            public const string GetContractByCode = "dbo.usp_CustomerHubContracts_GetByCode";
            public const string GetContractsByStatus = "dbo.usp_CustomerHubContracts_GetByStatus";
            public const string SaveContract = "dbo.usp_CustomerHubContracts_Save";
            public const string UpdateContractStatus = "dbo.usp_CustomerHubContracts_UpdateStatus";
            public const string GetAccountByCode = "dbo.usp_CustomerHubAccounts_GetByCode";
            public const string GetAccountsByTier = "dbo.usp_CustomerHubAccounts_GetByTier";
            public const string SaveAccount = "dbo.usp_CustomerHubAccounts_Save";
            public const string DeactivateAccount = "dbo.usp_CustomerHubAccounts_Deactivate";
            public const string GetReferralsByPartnerCode = "dbo.usp_CustomerHubReferrals_GetByPartnerCode";
            public const string SaveReferral = "dbo.usp_CustomerHubReferrals_Save";
            public const string GetReferralCommissionHistory = "dbo.usp_CustomerHubReferrals_GetCommissionHistory";
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
