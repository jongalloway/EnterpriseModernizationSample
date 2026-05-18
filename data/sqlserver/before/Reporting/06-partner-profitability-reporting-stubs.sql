USE [$(ReportingDatabase)];
GO

IF OBJECT_ID(N'dbo.usp_Reporting_PartnerRevenueAggregator', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Reporting_PartnerRevenueAggregator;
GO

CREATE PROCEDURE dbo.usp_Reporting_PartnerRevenueAggregator
    @SummaryDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @SummaryDate IS NULL
    BEGIN
        SET @SummaryDate = CONVERT(DATE, GETUTCDATE());
    END

    SELECT
        pa.PartnerCode,
        pa.PartnerName,
        pa.RelationshipTier,
        pa.PreferredStoreNumber,
        @SummaryDate AS SummaryDate,
        CAST(0.00 AS DECIMAL(18,2)) AS RevenueAmount,
        CAST(0 AS INT) AS DeliveredOrders,
        CAST(0.00 AS DECIMAL(9,2)) AS AverageTicket,
        CAST(0.00 AS DECIMAL(9,2)) AS GrowthPercentage
    FROM [$(CustomerHubDatabase)].dbo.PartnerAccount pa
    WHERE pa.StatusCode = N'Active';
END
GO

IF OBJECT_ID(N'dbo.usp_Reporting_CommissionCalculator', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Reporting_CommissionCalculator;
GO

CREATE PROCEDURE dbo.usp_Reporting_CommissionCalculator
    @SummaryDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @SummaryDate IS NULL
    BEGIN
        SET @SummaryDate = CONVERT(DATE, GETUTCDATE());
    END

    SELECT
        profitability.PartnerCode,
        profitability.SummaryDate,
        profitability.GrossSales,
        profitability.FeePercentage,
        CAST(0.00 AS DECIMAL(18,2)) AS GrossCommission,
        CAST(0.00 AS DECIMAL(18,2)) AS ReserveWithheld,
        CAST(0.00 AS DECIMAL(18,2)) AS NetCommissionPayout
    FROM dbo.PartnerProfitabilitySummary profitability
    WHERE profitability.SummaryDate = @SummaryDate;
END
GO

IF OBJECT_ID(N'dbo.usp_Reporting_ChargebackProcessor', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Reporting_ChargebackProcessor;
GO

CREATE PROCEDURE dbo.usp_Reporting_ChargebackProcessor
    @SummaryDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @SummaryDate IS NULL
    BEGIN
        SET @SummaryDate = CONVERT(DATE, GETUTCDATE());
    END

    SELECT
        profitability.PartnerCode,
        profitability.SummaryDate,
        CAST(0 AS INT) AS ChargebackCount,
        CAST(0 AS INT) AS DisputedCount,
        CAST(0.00 AS DECIMAL(9,2)) AS ChargebackRatePercentage,
        CAST(0.00 AS DECIMAL(9,2)) AS DisputeRatePercentage,
        CAST(0.00 AS DECIMAL(18,2)) AS GrossChargebackAmount,
        CAST(0.00 AS DECIMAL(18,2)) AS RecoveredAmount,
        CAST(0.00 AS DECIMAL(18,2)) AS OutstandingExposure
    FROM dbo.PartnerProfitabilitySummary profitability
    WHERE profitability.SummaryDate = @SummaryDate;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'Reporting\06-partner-profitability-reporting-stubs.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'Reporting\06-partner-profitability-reporting-stubs.sql');
END
GO
