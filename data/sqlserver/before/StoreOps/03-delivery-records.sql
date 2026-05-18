USE [$(StoreOpsDatabase)];
GO

INSERT INTO dbo.DeliveryRecord
(
    DispatchTicketId,
    StoreOrderId,
    StoreId,
    DriverId,
    RouteZoneId,
    RoutedUtc,
    DepartedUtc,
    DeliveredUtc,
    ReturnedUtc,
    RouteMiles,
    DeliveryMinutes,
    TipAmount,
    OutcomeCode,
    RouteSummary
)
SELECT dt.DispatchTicketId, dt.StoreOrderId, dt.StoreId, dt.DriverId, dt.RouteZoneId, source.RoutedUtc, source.DepartedUtc, source.DeliveredUtc, source.ReturnedUtc, source.RouteMiles, source.DeliveryMinutes, source.TipAmount, source.OutcomeCode, source.RouteSummary
FROM
(
    VALUES
        (51005, '2026-05-17T23:49:47.894', '2026-05-18T00:53:47.894', '2026-05-18T01:15:47.894', '2026-05-18T01:30:47.894', CAST(3.28 AS DECIMAL(5,2)), 22, CAST(4.50 AS MONEY), N'Completed', N'Northwest Corporate Corridor late-night route'),
        (51006, '2026-05-17T23:23:47.894', '2026-05-18T00:38:47.894', '2026-05-18T01:09:47.894', '2026-05-18T01:25:47.894', CAST(3.40 AS DECIMAL(5,2)), 31, CAST(5.60 AS MONEY), N'CompletedLate', N'Bellevue Downtown Towers late-night route'),
        (51015, '2026-05-17T23:47:47.894', '2026-05-18T00:53:47.894', '2026-05-18T01:16:47.894', '2026-05-18T01:31:47.894', CAST(3.63 AS DECIMAL(5,2)), 23, CAST(4.90 AS MONEY), N'Completed', N'Redmond East Campus Loop late-night route'),
        (51016, '2026-05-17T23:21:47.894', '2026-05-18T00:38:47.894', '2026-05-18T01:10:47.894', '2026-05-18T01:26:47.894', CAST(3.75 AS DECIMAL(5,2)), 32, CAST(6.00 AS MONEY), N'CompletedLate', N'Sammamish Tech Ridge late-night route'),
        (51025, '2026-05-17T23:45:47.894', '2026-05-18T00:53:47.894', '2026-05-18T01:17:47.894', '2026-05-18T01:32:47.894', CAST(3.98 AS DECIMAL(5,2)), 24, CAST(5.30 AS MONEY), N'Completed', N'Southcenter Mall Ring late-night route'),
        (51026, '2026-05-17T23:19:47.894', '2026-05-18T00:38:47.894', '2026-05-18T01:11:47.894', '2026-05-18T01:27:47.894', CAST(4.10 AS DECIMAL(5,2)), 33, CAST(6.40 AS MONEY), N'CompletedLate', N'Tukwila Industry Park late-night route'),
        (51035, '2026-05-17T23:43:47.894', '2026-05-18T00:50:47.894', '2026-05-18T01:15:47.894', '2026-05-18T01:30:47.894', CAST(4.33 AS DECIMAL(5,2)), 25, CAST(5.70 AS MONEY), N'Completed', N'Kirkland Downtown Core late-night route'),
        (51036, '2026-05-17T23:17:47.894', '2026-05-18T00:35:47.894', '2026-05-18T01:09:47.894', '2026-05-18T01:25:47.894', CAST(4.45 AS DECIMAL(5,2)), 34, CAST(6.80 AS MONEY), N'CompletedLate', N'Harbor Point Waterfront late-night route'),
        (51045, '2026-05-17T23:41:47.894', '2026-05-18T00:54:47.894', '2026-05-18T01:16:47.894', '2026-05-18T01:31:47.894', CAST(4.68 AS DECIMAL(5,2)), 22, CAST(6.10 AS MONEY), N'Completed', N'Airport Hotel Strip late-night route'),
        (51046, '2026-05-17T23:15:47.894', '2026-05-18T00:39:47.894', '2026-05-18T01:10:47.894', '2026-05-18T01:26:47.894', CAST(4.80 AS DECIMAL(5,2)), 31, CAST(7.20 AS MONEY), N'CompletedLate', N'Des Moines Shuttle Row late-night route'),
        (51055, '2026-05-17T23:39:47.894', '2026-05-18T00:54:47.894', '2026-05-18T01:17:47.894', '2026-05-18T01:32:47.894', CAST(5.03 AS DECIMAL(5,2)), 23, CAST(6.50 AS MONEY), N'Completed', N'Issaquah Highlands Loop late-night route'),
        (51056, '2026-05-17T23:13:47.894', '2026-05-18T00:39:47.894', '2026-05-18T01:11:47.894', '2026-05-18T01:27:47.894', CAST(5.15 AS DECIMAL(5,2)), 32, CAST(7.60 AS MONEY), N'CompletedLate', N'Squak Valley Estates late-night route'),
        (51065, '2026-05-17T23:37:47.894', '2026-05-18T00:51:47.894', '2026-05-18T01:15:47.894', '2026-05-18T01:30:47.894', CAST(5.38 AS DECIMAL(5,2)), 24, CAST(6.90 AS MONEY), N'Completed', N'Campus Quad late-night route'),
        (51066, '2026-05-17T23:11:47.894', '2026-05-18T00:36:47.894', '2026-05-18T01:09:47.894', '2026-05-18T01:25:47.894', CAST(5.50 AS DECIMAL(5,2)), 33, CAST(8.00 AS MONEY), N'CompletedLate', N'Green Lake Late-Night Ring late-night route'),
        (51075, '2026-05-17T23:35:47.894', '2026-05-18T00:51:47.894', '2026-05-18T01:16:47.894', '2026-05-18T01:31:47.894', CAST(5.73 AS DECIMAL(5,2)), 25, CAST(7.30 AS MONEY), N'Completed', N'Ballard North Hill late-night route'),
        (51076, '2026-05-17T23:09:47.894', '2026-05-18T00:36:47.894', '2026-05-18T01:10:47.894', '2026-05-18T01:26:47.894', CAST(5.85 AS DECIMAL(5,2)), 34, CAST(8.40 AS MONEY), N'CompletedLate', N'Fremont Bridge Corridor late-night route')
) source (TicketNumber, RoutedUtc, DepartedUtc, DeliveredUtc, ReturnedUtc, RouteMiles, DeliveryMinutes, TipAmount, OutcomeCode, RouteSummary)
INNER JOIN dbo.DispatchTicket dt
    ON dt.TicketNumber = source.TicketNumber
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.DeliveryRecord target
    WHERE target.DispatchTicketId = dt.DispatchTicketId
);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'StoreOps\03-delivery-records.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'StoreOps\03-delivery-records.sql');
END
GO
