USE [$(StoreOpsDatabase)];
GO

INSERT INTO dbo.StoreConfiguration
(
    StoreId,
    DeliveryRadiusMiles,
    MaxConcurrentDeliveries,
    MaxStackedStops,
    DispatchLeadMinutes,
    CarryoutHoldMinutes,
    AcceptsThirdPartyDispatch,
    CateringWarmHoldMinutes,
    LastMenuRefreshUtc
)
SELECT s.StoreId, source.DeliveryRadiusMiles, source.MaxConcurrentDeliveries, source.MaxStackedStops, source.DispatchLeadMinutes, source.CarryoutHoldMinutes, source.AcceptsThirdPartyDispatch, source.CateringWarmHoldMinutes, source.LastMenuRefreshUtc
FROM
(
    VALUES
        (N'014', CAST(6.50 AS DECIMAL(5,2)), 12, 3, 14, 20, CAST(0 AS BIT), 45, '2026-05-17T01:27:47.894'),
        (N'022', CAST(5.75 AS DECIMAL(5,2)), 10, 3, 14, 18, CAST(0 AS BIT), 45, '2026-05-17T01:21:47.894'),
        (N'031', CAST(4.50 AS DECIMAL(5,2)), 8, 2, 12, 18, CAST(1 AS BIT), 40, '2026-05-16T01:35:47.894'),
        (N'044', CAST(5.00 AS DECIMAL(5,2)), 9, 3, 13, 18, CAST(0 AS BIT), 40, '2026-05-17T01:17:47.894'),
        (N'057', CAST(7.25 AS DECIMAL(5,2)), 14, 4, 15, 22, CAST(1 AS BIT), 50, '2026-05-17T01:30:47.894'),
        (N'068', CAST(6.00 AS DECIMAL(5,2)), 9, 3, 13, 18, CAST(0 AS BIT), 40, '2026-05-16T01:23:47.894'),
        (N'081', CAST(3.75 AS DECIMAL(5,2)), 7, 2, 11, 15, CAST(0 AS BIT), 35, '2026-05-17T01:06:47.894'),
        (N'093', CAST(4.75 AS DECIMAL(5,2)), 9, 3, 12, 18, CAST(1 AS BIT), 40, '2026-05-17T01:12:47.894')
) source (StoreNumber, DeliveryRadiusMiles, MaxConcurrentDeliveries, MaxStackedStops, DispatchLeadMinutes, CarryoutHoldMinutes, AcceptsThirdPartyDispatch, CateringWarmHoldMinutes, LastMenuRefreshUtc)
INNER JOIN dbo.Store s
    ON s.StoreNumber = source.StoreNumber
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.StoreConfiguration target
    WHERE target.StoreId = s.StoreId
);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'StoreOps\03-store-configuration.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'StoreOps\03-store-configuration.sql');
END
GO
