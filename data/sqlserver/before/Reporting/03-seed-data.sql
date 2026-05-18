USE [$(ReportingDatabase)];
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.ReportingBatchRun
    WHERE SummaryDate = CAST('2026-05-18' AS DATE)
      AND RunStatus = N'Seeded'
)
BEGIN
    INSERT INTO dbo.ReportingBatchRun (SyncBatchId, SummaryDate, StartedUtc, CompletedUtc, RunStatus, Notes)
    VALUES
    (
        '11111111-1111-1111-1111-111111111417',
        CAST('2026-05-18' AS DATE),
        CAST('2026-05-18T01:39:47.894' AS DATETIME),
        CAST('2026-05-18T01:39:47.894' AS DATETIME),
        N'Seeded',
        N'Star-schema analytics snapshot for delivery scorecards, settlements, and partner rollups.'
    );
END
GO

INSERT INTO dbo.DimStore
(
    SourceStoreId,
    StoreNumber,
    StoreName,
    RegionCode,
    DistrictName,
    MarketName,
    ActiveFlag,
    LastRefreshedUtc
)
SELECT
    s.StoreId,
    s.StoreNumber,
    s.StoreName,
    s.RegionCode,
    sos.DistrictName,
    CASE s.StoreNumber
        WHEN N'014' THEN N'Northwest Corporate Corridor'
        WHEN N'022' THEN N'Eastside Tech Belt'
        WHEN N'031' THEN N'Mall Service Belt'
        WHEN N'057' THEN N'Airport Service District'
        WHEN N'081' THEN N'Campus Commons'
        ELSE N'Regional Trade Area'
    END,
    s.IsActive,
    CAST('2026-05-18T01:39:47.894' AS DATETIME)
FROM [$(StoreOpsDatabase)].dbo.Store s
LEFT JOIN [$(StoreOpsDatabase)].dbo.StoreOperationsStatus sos
    ON sos.StoreId = s.StoreId
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.DimStore target
    WHERE target.StoreNumber = s.StoreNumber
);
GO

INSERT INTO dbo.DimDriver
(
    SourceDriverId,
    DriverCode,
    DriverName,
    EmploymentStatus,
    HomeZoneCode,
    HomeStoreNumber,
    EligibilityExpirationDate,
    ActiveFlag,
    LastRefreshedUtc
)
SELECT
    d.DriverId,
    d.DriverCode,
    d.DisplayName,
    d.EmploymentStatus,
    d.HomeZoneCode,
    s.StoreNumber,
    d.EligibilityExpirationDate,
    CASE WHEN d.EmploymentStatus = N'Active' THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END,
    CAST('2026-05-18T01:39:47.894' AS DATETIME)
FROM [$(StoreOpsDatabase)].dbo.Driver d
INNER JOIN [$(StoreOpsDatabase)].dbo.Store s
    ON s.StoreId = d.StoreId
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.DimDriver target
    WHERE target.DriverCode = d.DriverCode
);
GO

INSERT INTO dbo.DimPartner
(
    SourcePartnerAccountId,
    SourcePartnerContractId,
    PartnerCode,
    PartnerName,
    RelationshipTier,
    PreferredStoreNumber,
    ContractCode,
    PricingScheduleName,
    ReferralChannel,
    DiscountPercentage,
    StatusCode,
    CreditHoldFlag,
    LastRefreshedUtc
)
SELECT
    pa.PartnerAccountId,
    contract.PartnerContractId,
    pa.PartnerCode,
    pa.PartnerName,
    pa.RelationshipTier,
    pa.PreferredStoreNumber,
    contract.ContractCode,
    contract.PricingScheduleName,
    contract.ReferralChannel,
    ISNULL(contract.DiscountPercentage, 0.00),
    pa.StatusCode,
    pa.CreditHold,
    CAST('2026-05-18T01:39:47.894' AS DATETIME)
FROM [$(CustomerHubDatabase)].dbo.PartnerAccount pa
OUTER APPLY
(
    SELECT TOP (1)
        pc.PartnerContractId,
        pc.ContractCode,
        pc.PricingScheduleName,
        pc.ReferralChannel,
        pc.DiscountPercentage
    FROM [$(CustomerHubDatabase)].dbo.PartnerContract pc
    WHERE pc.PartnerAccountId = pa.PartnerAccountId
      AND pc.StatusCode = N'Active'
    ORDER BY pc.EffectiveDate DESC, pc.PartnerContractId DESC
) contract
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.DimPartner target
    WHERE target.PartnerCode = pa.PartnerCode
);
GO

INSERT INTO dbo.DimTime
(
    TimeKey,
    FullDate,
    DayName,
    DayOfWeekNumber,
    WeekEndingDate,
    MonthStartDate,
    MonthName,
    MonthNumber,
    QuarterNumber,
    CalendarYear,
    IsWeekend
)
SELECT
    source.TimeKey,
    source.FullDate,
    source.DayName,
    source.DayOfWeekNumber,
    source.WeekEndingDate,
    source.MonthStartDate,
    source.MonthName,
    source.MonthNumber,
    source.QuarterNumber,
    source.CalendarYear,
    source.IsWeekend
FROM
(
    VALUES
        (20260511, CAST('2026-05-11' AS DATE), N'Monday', 1, CAST('2026-05-17' AS DATE), CAST('2026-05-01' AS DATE), N'May', 5, 2, 2026, CAST(0 AS BIT)),
        (20260512, CAST('2026-05-12' AS DATE), N'Tuesday', 2, CAST('2026-05-17' AS DATE), CAST('2026-05-01' AS DATE), N'May', 5, 2, 2026, CAST(0 AS BIT)),
        (20260513, CAST('2026-05-13' AS DATE), N'Wednesday', 3, CAST('2026-05-17' AS DATE), CAST('2026-05-01' AS DATE), N'May', 5, 2, 2026, CAST(0 AS BIT)),
        (20260514, CAST('2026-05-14' AS DATE), N'Thursday', 4, CAST('2026-05-17' AS DATE), CAST('2026-05-01' AS DATE), N'May', 5, 2, 2026, CAST(0 AS BIT)),
        (20260515, CAST('2026-05-15' AS DATE), N'Friday', 5, CAST('2026-05-17' AS DATE), CAST('2026-05-01' AS DATE), N'May', 5, 2, 2026, CAST(0 AS BIT)),
        (20260516, CAST('2026-05-16' AS DATE), N'Saturday', 6, CAST('2026-05-17' AS DATE), CAST('2026-05-01' AS DATE), N'May', 5, 2, 2026, CAST(1 AS BIT)),
        (20260517, CAST('2026-05-17' AS DATE), N'Sunday', 7, CAST('2026-05-17' AS DATE), CAST('2026-05-01' AS DATE), N'May', 5, 2, 2026, CAST(1 AS BIT)),
        (20260518, CAST('2026-05-18' AS DATE), N'Monday', 1, CAST('2026-05-24' AS DATE), CAST('2026-05-01' AS DATE), N'May', 5, 2, 2026, CAST(0 AS BIT))
) source (TimeKey, FullDate, DayName, DayOfWeekNumber, WeekEndingDate, MonthStartDate, MonthName, MonthNumber, QuarterNumber, CalendarYear, IsWeekend)
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.DimTime target
    WHERE target.TimeKey = source.TimeKey
);
GO

INSERT INTO dbo.DimProduct
(
    ProductCode,
    ProductName,
    ProductCategory,
    SizeName,
    CrustStyle,
    BasePrice,
    ActiveFlag,
    LimitedTimeFlag,
    LastRefreshedUtc
)
SELECT
    source.ProductCode,
    source.ProductName,
    source.ProductCategory,
    source.SizeName,
    source.CrustStyle,
    source.BasePrice,
    source.ActiveFlag,
    source.LimitedTimeFlag,
    CAST('2026-05-18T01:39:47.894' AS DATETIME)
FROM
(
    VALUES
        (N'PEP-LG', N'Pepperoni Feast Pizza', N'Specialty Pizza', N'Large', N'Hand Tossed', CAST(18.99 AS MONEY), CAST(1 AS BIT), CAST(0 AS BIT)),
        (N'VEG-MD', N'Garden Harvest Pizza', N'Vegetarian Pizza', N'Medium', N'Thin Crust', CAST(15.50 AS MONEY), CAST(1 AS BIT), CAST(0 AS BIT)),
        (N'BBQ-XL', N'Smokehouse BBQ Chicken Pizza', N'Specialty Pizza', N'X-Large', N'Pan', CAST(21.75 AS MONEY), CAST(1 AS BIT), CAST(0 AS BIT)),
        (N'BREAD-KNOTS', N'Garlic Parmesan Bread Knots', N'Sides', N'Family', N'Oven Baked', CAST(7.49 AS MONEY), CAST(1 AS BIT), CAST(0 AS BIT)),
        (N'CAESAR-KIT', N'Caesar Salad Kit', N'Salads', N'Family', N'Chilled', CAST(6.99 AS MONEY), CAST(1 AS BIT), CAST(0 AS BIT)),
        (N'SUPREME-LG', N'Supreme Combo Pizza', N'Specialty Pizza', N'Large', N'Hand Tossed', CAST(20.49 AS MONEY), CAST(1 AS BIT), CAST(0 AS BIT))
) source (ProductCode, ProductName, ProductCategory, SizeName, CrustStyle, BasePrice, ActiveFlag, LimitedTimeFlag)
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.DimProduct target
    WHERE target.ProductCode = source.ProductCode
);
GO

;WITH source AS
(
    SELECT *
    FROM
    (
        VALUES
            (74105, 20260518, N'014', N'PEP-LG', N'CORP-1002', N'Web', N'Delivery', 1, 3, CAST(43.75 AS MONEY), CAST(3.72 AS MONEY), CAST(40.03 AS MONEY)),
            (74106, 20260518, N'014', N'VEG-MD', NULL, N'Call Center', N'Delivery', 1, 2, CAST(38.10 AS MONEY), CAST(0.00 AS MONEY), CAST(38.10 AS MONEY)),
            (71510, 20260518, N'014', N'BREAD-KNOTS', NULL, N'Phone', N'Carryout', 1, 1, CAST(18.75 AS MONEY), CAST(0.00 AS MONEY), CAST(18.75 AS MONEY)),
            (71518, 20260518, N'014', N'BBQ-XL', NULL, N'POS', N'Carryout', 1, 2, CAST(32.00 AS MONEY), CAST(0.00 AS MONEY), CAST(32.00 AS MONEY)),
            (72220, 20260518, N'022', N'PEP-LG', N'EVT-4405', N'Call Center', N'Delivery', 1, 2, CAST(24.60 AS MONEY), CAST(0.98 AS MONEY), CAST(23.62 AS MONEY)),
            (57011, 20260517, N'057', N'BBQ-XL', N'COMM-8821', N'Web', N'Delivery', 1, 4, CAST(58.00 AS MONEY), CAST(2.90 AS MONEY), CAST(55.10 AS MONEY)),
            (81044, 20260516, N'081', N'VEG-MD', NULL, N'App', N'Carryout', 1, 2, CAST(26.50 AS MONEY), CAST(0.00 AS MONEY), CAST(26.50 AS MONEY)),
            (81060, 20260516, N'081', N'SUPREME-LG', NULL, N'App', N'Delivery', 1, 2, CAST(29.40 AS MONEY), CAST(0.00 AS MONEY), CAST(29.40 AS MONEY)),
            (31088, 20260515, N'031', N'CAESAR-KIT', NULL, N'Phone', N'Carryout', 1, 3, CAST(21.00 AS MONEY), CAST(0.00 AS MONEY), CAST(21.00 AS MONEY))
    ) valueSet (OrderNumber, TimeKey, StoreNumber, ProductCode, PartnerCode, SalesChannel, ServiceMode, OrderCount, ItemQuantity, GrossSalesAmount, DiscountAmount, NetSalesAmount)
)
INSERT INTO dbo.FactOrder
(
    TimeKey,
    StoreKey,
    ProductKey,
    PartnerKey,
    OrderNumber,
    SalesChannel,
    ServiceMode,
    OrderCount,
    ItemQuantity,
    GrossSalesAmount,
    DiscountAmount,
    NetSalesAmount,
    LoadedUtc
)
SELECT
    timeDim.TimeKey,
    storeDim.StoreKey,
    productDim.ProductKey,
    partnerDim.PartnerKey,
    source.OrderNumber,
    source.SalesChannel,
    source.ServiceMode,
    source.OrderCount,
    source.ItemQuantity,
    source.GrossSalesAmount,
    source.DiscountAmount,
    source.NetSalesAmount,
    CAST('2026-05-18T01:39:47.894' AS DATETIME)
FROM source
INNER JOIN dbo.DimTime timeDim
    ON timeDim.TimeKey = source.TimeKey
INNER JOIN dbo.DimStore storeDim
    ON storeDim.StoreNumber = source.StoreNumber
INNER JOIN dbo.DimProduct productDim
    ON productDim.ProductCode = source.ProductCode
LEFT JOIN dbo.DimPartner partnerDim
    ON partnerDim.PartnerCode = source.PartnerCode
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.FactOrder target
    WHERE target.OrderNumber = source.OrderNumber
      AND target.ProductKey = productDim.ProductKey
);
GO

;WITH source AS
(
    SELECT *
    FROM
    (
        VALUES
            (4105, 74105, 20260518, N'014', N'DRV-17', N'CORP-1002', N'Completed', 1, 24, 25, 0, CAST(40.03 AS MONEY)),
            (4106, 74106, 20260518, N'014', N'DRV-03', NULL, N'CompletedLate', 1, 39, 30, 1, CAST(38.10 AS MONEY)),
            (4220, 72220, 20260518, N'022', N'DRV-22', N'EVT-4405', N'Completed', 1, 28, 30, 0, CAST(23.62 AS MONEY)),
            (5057, 57011, 20260517, N'057', N'DRV-57', N'COMM-8821', N'CompletedLate', 1, 34, 30, 1, CAST(55.10 AS MONEY)),
            (5081, 81060, 20260516, N'081', N'DRV-81', NULL, N'Completed', 1, 27, 28, 0, CAST(29.40 AS MONEY))
    ) valueSet (TicketNumber, OrderNumber, TimeKey, StoreNumber, DriverCode, PartnerCode, DeliveryStatusCode, DeliveryCount, DeliveryMinutes, PromiseWindowMinutes, LateDeliveryCount, NetSalesAmount)
)
INSERT INTO dbo.FactDelivery
(
    TimeKey,
    StoreKey,
    DriverKey,
    PartnerKey,
    OrderNumber,
    TicketNumber,
    DeliveryStatusCode,
    DeliveryCount,
    DeliveryMinutes,
    PromiseWindowMinutes,
    LateDeliveryCount,
    NetSalesAmount,
    LoadedUtc
)
SELECT
    timeDim.TimeKey,
    storeDim.StoreKey,
    driverDim.DriverKey,
    partnerDim.PartnerKey,
    source.OrderNumber,
    source.TicketNumber,
    source.DeliveryStatusCode,
    source.DeliveryCount,
    source.DeliveryMinutes,
    source.PromiseWindowMinutes,
    source.LateDeliveryCount,
    source.NetSalesAmount,
    CAST('2026-05-18T01:39:47.894' AS DATETIME)
FROM source
INNER JOIN dbo.DimTime timeDim
    ON timeDim.TimeKey = source.TimeKey
INNER JOIN dbo.DimStore storeDim
    ON storeDim.StoreNumber = source.StoreNumber
LEFT JOIN dbo.DimDriver driverDim
    ON driverDim.DriverCode = source.DriverCode
LEFT JOIN dbo.DimPartner partnerDim
    ON partnerDim.PartnerCode = source.PartnerCode
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.FactDelivery target
    WHERE target.TicketNumber = source.TicketNumber
);
GO

;WITH source AS
(
    SELECT *
    FROM
    (
        VALUES
            (20260518, N'014', N'CORP-1002', 1, CAST(43.75 AS MONEY), CAST(3.72 AS MONEY), CAST(40.03 AS MONEY), CAST(8.50 AS DECIMAL(5,2)), CAST(3.40 AS MONEY), CAST(36.63 AS MONEY)),
            (20260517, N'057', N'COMM-8821', 1, CAST(58.00 AS MONEY), CAST(2.90 AS MONEY), CAST(55.10 AS MONEY), CAST(5.00 AS DECIMAL(5,2)), CAST(2.76 AS MONEY), CAST(52.34 AS MONEY)),
            (20260518, N'022', N'EVT-4405', 1, CAST(24.60 AS MONEY), CAST(0.98 AS MONEY), CAST(23.62 AS MONEY), CAST(4.00 AS DECIMAL(5,2)), CAST(0.94 AS MONEY), CAST(22.68 AS MONEY))
    ) valueSet (TimeKey, StoreNumber, PartnerCode, DeliveredOrders, GrossSalesAmount, DiscountAmount, NetSalesAmount, PartnerFeePercentage, PartnerFeeAmount, SettlementAmount)
)
INSERT INTO dbo.FactPartnerRevenue
(
    TimeKey,
    StoreKey,
    PartnerKey,
    DeliveredOrders,
    GrossSalesAmount,
    DiscountAmount,
    NetSalesAmount,
    PartnerFeePercentage,
    PartnerFeeAmount,
    SettlementAmount,
    LoadedUtc
)
SELECT
    timeDim.TimeKey,
    storeDim.StoreKey,
    partnerDim.PartnerKey,
    source.DeliveredOrders,
    source.GrossSalesAmount,
    source.DiscountAmount,
    source.NetSalesAmount,
    source.PartnerFeePercentage,
    source.PartnerFeeAmount,
    source.SettlementAmount,
    CAST('2026-05-18T01:39:47.894' AS DATETIME)
FROM source
INNER JOIN dbo.DimTime timeDim
    ON timeDim.TimeKey = source.TimeKey
INNER JOIN dbo.DimStore storeDim
    ON storeDim.StoreNumber = source.StoreNumber
INNER JOIN dbo.DimPartner partnerDim
    ON partnerDim.PartnerCode = source.PartnerCode
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.FactPartnerRevenue target
    WHERE target.TimeKey = source.TimeKey
      AND target.StoreKey = storeDim.StoreKey
      AND target.PartnerKey = partnerDim.PartnerKey
);
GO

;WITH deliverySource AS
(
    SELECT
        storeDim.StoreNumber,
        timeDim.FullDate AS SummaryDate,
        SUM(fact.DeliveryCount) AS CompletedRuns,
        SUM(fact.LateDeliveryCount) AS LateRuns,
        SUM(fact.NetSalesAmount) AS SalesAmount
    FROM dbo.FactDelivery fact
    INNER JOIN dbo.DimStore storeDim
        ON storeDim.StoreKey = fact.StoreKey
    INNER JOIN dbo.DimTime timeDim
        ON timeDim.TimeKey = fact.TimeKey
    GROUP BY storeDim.StoreNumber, timeDim.FullDate
)
INSERT INTO dbo.DeliveryDailySummary
(
    StoreNumber,
    SummaryDate,
    CompletedRuns,
    LateRuns,
    SalesAmount,
    LastLoadedUtc
)
SELECT
    deliverySource.StoreNumber,
    deliverySource.SummaryDate,
    deliverySource.CompletedRuns,
    deliverySource.LateRuns,
    deliverySource.SalesAmount,
    CAST('2026-05-18T01:39:47.894' AS DATETIME)
FROM deliverySource
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.DeliveryDailySummary target
    WHERE target.StoreNumber = deliverySource.StoreNumber
      AND target.SummaryDate = deliverySource.SummaryDate
);
GO

INSERT INTO dbo.LaborDailySummary
(
    StoreNumber,
    SummaryDate,
    DriverCount,
    ExceptionCount,
    ScheduledHours,
    WorkedHours,
    OvertimeHours,
    RegularLaborCost,
    OvertimeLaborCost,
    AgencyLaborCost,
    NetSales,
    LastLoadedUtc
)
SELECT
    source.StoreNumber,
    source.SummaryDate,
    source.DriverCount,
    source.ExceptionCount,
    source.ScheduledHours,
    source.WorkedHours,
    source.OvertimeHours,
    source.RegularLaborCost,
    source.OvertimeLaborCost,
    source.AgencyLaborCost,
    source.NetSales,
    CAST('2026-05-18T01:39:47.894' AS DATETIME)
FROM
(
    VALUES
        (N'014', CAST('2026-05-18' AS DATE), 2, 1, CAST(164.00 AS DECIMAL(9,2)), CAST(171.50 AS DECIMAL(9,2)), CAST(7.50 AS DECIMAL(9,2)), CAST(2448.00 AS MONEY), CAST(213.75 AS MONEY), CAST(96.00 AS MONEY), CAST(8200.00 AS MONEY)),
        (N'022', CAST('2026-05-18' AS DATE), 1, 0, CAST(118.00 AS DECIMAL(9,2)), CAST(120.50 AS DECIMAL(9,2)), CAST(2.50 AS DECIMAL(9,2)), CAST(1711.00 AS MONEY), CAST(68.25 AS MONEY), CAST(0.00 AS MONEY), CAST(5940.00 AS MONEY))
) source (StoreNumber, SummaryDate, DriverCount, ExceptionCount, ScheduledHours, WorkedHours, OvertimeHours, RegularLaborCost, OvertimeLaborCost, AgencyLaborCost, NetSales)
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.LaborDailySummary target
    WHERE target.StoreNumber = source.StoreNumber
      AND target.SummaryDate = source.SummaryDate
);
GO

INSERT INTO dbo.LaborOvertimeWeeklySummary
(
    StoreNumber,
    WeekEndingDate,
    DriverOvertimeHours,
    KitchenOvertimeHours,
    ShiftLeadOvertimeHours,
    LastLoadedUtc
)
SELECT
    source.StoreNumber,
    source.WeekEndingDate,
    source.DriverOvertimeHours,
    source.KitchenOvertimeHours,
    source.ShiftLeadOvertimeHours,
    CAST('2026-05-18T01:39:47.894' AS DATETIME)
FROM
(
    VALUES
        (N'014', CAST('2026-04-26' AS DATE), CAST(5.00 AS DECIMAL(9,2)), CAST(2.00 AS DECIMAL(9,2)), CAST(1.00 AS DECIMAL(9,2))),
        (N'014', CAST('2026-05-03' AS DATE), CAST(4.50 AS DECIMAL(9,2)), CAST(2.50 AS DECIMAL(9,2)), CAST(1.25 AS DECIMAL(9,2))),
        (N'014', CAST('2026-05-10' AS DATE), CAST(5.75 AS DECIMAL(9,2)), CAST(2.75 AS DECIMAL(9,2)), CAST(1.50 AS DECIMAL(9,2))),
        (N'014', CAST('2026-05-17' AS DATE), CAST(6.00 AS DECIMAL(9,2)), CAST(3.00 AS DECIMAL(9,2)), CAST(1.50 AS DECIMAL(9,2))),
        (N'022', CAST('2026-05-10' AS DATE), CAST(2.00 AS DECIMAL(9,2)), CAST(1.50 AS DECIMAL(9,2)), CAST(0.50 AS DECIMAL(9,2))),
        (N'022', CAST('2026-05-17' AS DATE), CAST(2.25 AS DECIMAL(9,2)), CAST(1.25 AS DECIMAL(9,2)), CAST(0.50 AS DECIMAL(9,2)))
) source (StoreNumber, WeekEndingDate, DriverOvertimeHours, KitchenOvertimeHours, ShiftLeadOvertimeHours)
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.LaborOvertimeWeeklySummary target
    WHERE target.StoreNumber = source.StoreNumber
      AND target.WeekEndingDate = source.WeekEndingDate
);
GO

INSERT INTO dbo.WorkforceTurnoverMonthlySummary
(
    StoreNumber,
    SummaryMonth,
    BeginningHeadcount,
    HireCount,
    SeparationCount,
    EndingHeadcount,
    LastLoadedUtc
)
SELECT
    source.StoreNumber,
    source.SummaryMonth,
    source.BeginningHeadcount,
    source.HireCount,
    source.SeparationCount,
    source.EndingHeadcount,
    CAST('2026-05-18T01:39:47.894' AS DATETIME)
FROM
(
    VALUES
        (N'014', CAST('2026-05-01' AS DATE), 27, 3, 2, 28),
        (N'022', CAST('2026-05-01' AS DATE), 19, 1, 1, 19)
) source (StoreNumber, SummaryMonth, BeginningHeadcount, HireCount, SeparationCount, EndingHeadcount)
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.WorkforceTurnoverMonthlySummary target
    WHERE target.StoreNumber = source.StoreNumber
      AND target.SummaryMonth = source.SummaryMonth
);
GO

INSERT INTO dbo.StaffingDailySummary
(
    StoreNumber,
    SummaryDate,
    ScheduledDriverSlots,
    FilledDriverSlots,
    OpenDriverSlots,
    CrossTrainedTeamMembers,
    CalloutCount,
    LastLoadedUtc
)
SELECT
    source.StoreNumber,
    source.SummaryDate,
    source.ScheduledDriverSlots,
    source.FilledDriverSlots,
    source.OpenDriverSlots,
    source.CrossTrainedTeamMembers,
    source.CalloutCount,
    CAST('2026-05-18T01:39:47.894' AS DATETIME)
FROM
(
    VALUES
        (N'014', CAST('2026-05-18' AS DATE), 18, 15, 3, 2, 1),
        (N'022', CAST('2026-05-18' AS DATE), 12, 11, 1, 1, 0)
) source (StoreNumber, SummaryDate, ScheduledDriverSlots, FilledDriverSlots, OpenDriverSlots, CrossTrainedTeamMembers, CalloutCount)
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.StaffingDailySummary target
    WHERE target.StoreNumber = source.StoreNumber
      AND target.SummaryDate = source.SummaryDate
);
GO

;WITH partnerSource AS
(
    SELECT
        partnerDim.PartnerCode,
        timeDim.FullDate AS SummaryDate,
        SUM(fact.DeliveredOrders) AS DeliveredOrders,
        SUM(fact.GrossSalesAmount) AS GrossSales,
        MAX(fact.PartnerFeePercentage) AS FeePercentage
    FROM dbo.FactPartnerRevenue fact
    INNER JOIN dbo.DimPartner partnerDim
        ON partnerDim.PartnerKey = fact.PartnerKey
    INNER JOIN dbo.DimTime timeDim
        ON timeDim.TimeKey = fact.TimeKey
    GROUP BY partnerDim.PartnerCode, timeDim.FullDate
)
INSERT INTO dbo.PartnerProfitabilitySummary
(
    PartnerCode,
    SummaryDate,
    DeliveredOrders,
    GrossSales,
    FeePercentage,
    LastLoadedUtc
)
SELECT
    partnerSource.PartnerCode,
    partnerSource.SummaryDate,
    partnerSource.DeliveredOrders,
    partnerSource.GrossSales,
    partnerSource.FeePercentage,
    CAST('2026-05-18T01:39:47.894' AS DATETIME)
FROM partnerSource
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.PartnerProfitabilitySummary target
    WHERE target.PartnerCode = partnerSource.PartnerCode
      AND target.SummaryDate = partnerSource.SummaryDate
);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'Reporting\03-seed-data.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'Reporting\03-seed-data.sql');
END
GO

