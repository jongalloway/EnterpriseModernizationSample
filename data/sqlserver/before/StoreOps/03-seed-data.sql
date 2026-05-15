USE [$(StoreOpsDatabase)];
GO

INSERT INTO dbo.Store (StoreNumber, StoreName, RegionCode, PhoneNumber, IsActive)
SELECT source.StoreNumber, source.StoreName, source.RegionCode, source.PhoneNumber, source.IsActive
FROM
(
    VALUES
        (N'014', N'Bellevue Corporate Center', N'NW', N'555-0140', CAST(1 AS BIT)),
        (N'022', N'Redmond East', N'NW', N'555-0220', CAST(1 AS BIT)),
        (N'031', N'Mall Annex', N'NW', N'555-0310', CAST(1 AS BIT)),
        (N'057', N'Airport Service Road', N'SM', N'555-0570', CAST(1 AS BIT)),
        (N'081', N'College Commons', N'CM', N'555-0810', CAST(1 AS BIT))
) source (StoreNumber, StoreName, RegionCode, PhoneNumber, IsActive)
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Store target
    WHERE target.StoreNumber = source.StoreNumber
);
GO

INSERT INTO dbo.RouteZone (StoreId, ZoneCode, ZoneName, SortOrder)
SELECT s.StoreId, source.ZoneCode, source.ZoneName, source.SortOrder
FROM
(
    VALUES
        (N'014', N'NW-CORP', N'Northwest Corporate Corridor', CAST(1 AS TINYINT)),
        (N'014', N'MALL-ANNEX', N'Mall Annex', CAST(2 AS TINYINT)),
        (N'022', N'RED-EAST', N'Redmond East Campus', CAST(1 AS TINYINT)),
        (N'031', N'SECURITY-DESK', N'Mall Security Desk', CAST(1 AS TINYINT)),
        (N'057', N'AIRPORT-HOTEL', N'Airport Hotel Strip', CAST(1 AS TINYINT)),
        (N'081', N'CAMPUS-QUAD', N'Campus Quad', CAST(1 AS TINYINT))
) source (StoreNumber, ZoneCode, ZoneName, SortOrder)
INNER JOIN dbo.Store s
    ON s.StoreNumber = source.StoreNumber
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.RouteZone target
    WHERE target.StoreId = s.StoreId
      AND target.ZoneCode = source.ZoneCode
);
GO

INSERT INTO dbo.Driver (StoreId, DriverCode, DisplayName, EmploymentStatus, HomeZoneCode, EligibilityExpirationDate)
SELECT s.StoreId, source.DriverCode, source.DisplayName, source.EmploymentStatus, source.HomeZoneCode, DATEADD(DAY, 365, GETUTCDATE())
FROM
(
    VALUES
        (N'014', N'DRV-17', N'Luis Mendoza', N'Active', N'NW-CORP'),
        (N'014', N'DRV-03', N'Rachel Kim', N'Active', N'MALL-ANNEX'),
        (N'022', N'DRV-22', N'Darren Scott', N'Active', N'RED-EAST'),
        (N'031', N'DRV-31', N'Jasmine Patel', N'Active', N'SECURITY-DESK'),
        (N'057', N'DRV-57', N'Trevor Morris', N'Active', N'AIRPORT-HOTEL'),
        (N'081', N'DRV-81', N'Riley Nguyen', N'Active', N'CAMPUS-QUAD')
) source (StoreNumber, DriverCode, DisplayName, EmploymentStatus, HomeZoneCode)
INNER JOIN dbo.Store s
    ON s.StoreNumber = source.StoreNumber
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Driver target
    WHERE target.DriverCode = source.DriverCode
);
GO

INSERT INTO dbo.StoreOperationsStatus
(
    StoreId,
    DistrictName,
    DispatchTerminalId,
    ManagerOnDuty,
    BoardMode,
    LastStatusRefreshUtc,
    StoreStatus,
    EscalationNote
)
SELECT
    s.StoreId,
    source.DistrictName,
    source.DispatchTerminalId,
    source.ManagerOnDuty,
    source.BoardMode,
    DATEADD(MINUTE, source.MinutesSinceRefresh * -1, GETUTCDATE()),
    source.StoreStatus,
    source.EscalationNote
FROM
(
    VALUES
        (N'014', N'North Metro', N'TERM-02', N'M. Delgado', N'Balanced', 6, N'Normal', N'No escalations waiting.'),
        (N'022', N'North Metro', N'TERM-04', N'A. Rivera', N'Balanced', 8, N'Normal', N'Campus POS import landed on schedule.'),
        (N'031', N'North Metro', N'TERM-05', N'J. Patel', N'Rush Recovery', 8, N'Needs Follow-Up', N'Driver board printer is running behind on reprint slips.'),
        (N'057', N'South Metro', N'TERM-03', N'T. Morris', N'Delivery Heavy', 7, N'Normal', N'No escalations waiting.'),
        (N'081', N'Campus', N'TERM-07', N'R. Nguyen', N'Counter Hold', 11, N'Needs Follow-Up', N'Clock drift on the carryout counter is stretching promise windows.')
) source (StoreNumber, DistrictName, DispatchTerminalId, ManagerOnDuty, BoardMode, MinutesSinceRefresh, StoreStatus, EscalationNote)
INNER JOIN dbo.Store s
    ON s.StoreNumber = source.StoreNumber
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.StoreOperationsStatus target
    WHERE target.StoreId = s.StoreId
);
GO

INSERT INTO dbo.StoreOrder
(
    OrderNumber,
    StoreId,
    CustomerName,
    ChannelCode,
    ServiceMode,
    PromiseUtc,
    TicketTotal,
    KitchenStatus,
    DispatchStatus,
    PaymentStatus
)
SELECT
    source.OrderNumber,
    s.StoreId,
    source.CustomerName,
    source.ChannelCode,
    source.ServiceMode,
    DATEADD(MINUTE, source.PromiseOffsetMinutes, GETUTCDATE()),
    source.TicketTotal,
    source.KitchenStatus,
    source.DispatchStatus,
    source.PaymentStatus
FROM
(
    VALUES
        (74105, N'014', N'North Corridor Office', N'Web', N'Delivery', 28, CAST(27.50 AS MONEY), N'Make Line', N'Staged', N'Card Hold'),
        (74106, N'014', N'Corporate Lunch Desk', N'Call Center', N'Delivery', 34, CAST(33.75 AS MONEY), N'Ready', N'Ready', N'Settled'),
        (71510, N'014', N'Lobby Pickup - Harris', N'Phone', N'Carryout', 26, CAST(18.75 AS MONEY), N'Ready', N'Carryout Hold', N'Settled'),
        (71518, N'014', N'School Night Bundle', N'POS', N'Carryout', 34, CAST(32.00 AS MONEY), N'Exception', N'Carryout Hold', N'Cash Pending'),
        (72220, N'022', N'Redmond East Campus Order', N'Call Center', N'Delivery', 31, CAST(24.60 AS MONEY), N'Ready', N'Staged', N'Settled')
) source (OrderNumber, StoreNumber, CustomerName, ChannelCode, ServiceMode, PromiseOffsetMinutes, TicketTotal, KitchenStatus, DispatchStatus, PaymentStatus)
INNER JOIN dbo.Store s
    ON s.StoreNumber = source.StoreNumber
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.StoreOrder target
    WHERE target.OrderNumber = source.OrderNumber
);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DispatchTicket WHERE TicketNumber = 4105)
BEGIN
    INSERT INTO dbo.DispatchTicket (TicketNumber, StoreOrderId, StoreId, DriverId, RouteZoneId, OrderOrigin, PromiseUtc, StatusCode, TotalAmount, LastUpdatedUtc)
    SELECT 4105, so.StoreOrderId, s.StoreId, d.DriverId, rz.RouteZoneId, N'CallCenter', so.PromiseUtc, N'Dispatched', 43.75, GETUTCDATE()
    FROM dbo.Store s
    INNER JOIN dbo.StoreOrder so ON so.OrderNumber = 74105 AND so.StoreId = s.StoreId
    INNER JOIN dbo.Driver d ON d.DriverCode = N'DRV-17'
    INNER JOIN dbo.RouteZone rz ON rz.ZoneCode = N'NW-CORP' AND rz.StoreId = s.StoreId
    WHERE s.StoreNumber = N'014';

    INSERT INTO dbo.DispatchTicket (TicketNumber, StoreOrderId, StoreId, DriverId, RouteZoneId, OrderOrigin, PromiseUtc, StatusCode, TotalAmount, LastUpdatedUtc)
    SELECT 4106, so.StoreOrderId, s.StoreId, d.DriverId, rz.RouteZoneId, N'WebStorefront', so.PromiseUtc, N'Dispatched', 38.10, GETUTCDATE()
    FROM dbo.Store s
    INNER JOIN dbo.StoreOrder so ON so.OrderNumber = 74106 AND so.StoreId = s.StoreId
    INNER JOIN dbo.Driver d ON d.DriverCode = N'DRV-03'
    INNER JOIN dbo.RouteZone rz ON rz.ZoneCode = N'MALL-ANNEX' AND rz.StoreId = s.StoreId
    WHERE s.StoreNumber = N'014';
END
GO

UPDATE dt
SET dt.StoreOrderId = so.StoreOrderId
FROM dbo.DispatchTicket dt
INNER JOIN dbo.StoreOrder so
    ON so.OrderNumber = CASE dt.TicketNumber WHEN 4105 THEN 74105 WHEN 4106 THEN 74106 ELSE -1 END
WHERE dt.StoreOrderId IS NULL
  AND dt.TicketNumber IN (4105, 4106);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PosOrderImportBatch)
BEGIN
    INSERT INTO dbo.PosOrderImportBatch (StoreId, SourceSystem, BatchDate, ImportedUtc, BatchStatus, ItemCount)
    SELECT StoreId, N'CampusPOS', CONVERT(DATE, GETUTCDATE()), GETUTCDATE(), N'Complete', 57
    FROM dbo.Store
    WHERE StoreNumber = N'014';
END
GO

INSERT INTO dbo.PosOrderImportItem
(
    PosOrderImportBatchId,
    OrderNumber,
    ChannelCode,
    ServiceMode,
    ImportStatus,
    ImportedTicketTotal,
    ExceptionNote
)
SELECT
    pob.PosOrderImportBatchId,
    source.OrderNumber,
    source.ChannelCode,
    source.ServiceMode,
    source.ImportStatus,
    source.ImportedTicketTotal,
    source.ExceptionNote
FROM dbo.PosOrderImportBatch pob
INNER JOIN dbo.Store s
    ON s.StoreId = pob.StoreId
INNER JOIN
(
    VALUES
        (74105, N'Web', N'Delivery', N'Complete', CAST(27.50 AS MONEY), CAST(NULL AS NVARCHAR(200))),
        (74106, N'Call Center', N'Delivery', N'Complete', CAST(33.75 AS MONEY), CAST(NULL AS NVARCHAR(200))),
        (71518, N'POS', N'Carryout', N'Exception', CAST(32.00 AS MONEY), N'Cash drawer still waiting on close-out.')
) source (OrderNumber, ChannelCode, ServiceMode, ImportStatus, ImportedTicketTotal, ExceptionNote)
    ON s.StoreNumber = N'014'
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.PosOrderImportItem target
    WHERE target.PosOrderImportBatchId = pob.PosOrderImportBatchId
      AND target.OrderNumber = source.OrderNumber
);
GO

INSERT INTO dbo.WorkforceAlert
(
    StoreId,
    TeamName,
    ConcernText,
    ActionRequired,
    SeverityCode,
    EffectiveUtc,
    ResolvedUtc
)
SELECT
    s.StoreId,
    source.TeamName,
    source.ConcernText,
    source.ActionRequired,
    source.SeverityCode,
    DATEADD(MINUTE, source.MinutesAgo * -1, GETUTCDATE()),
    NULL
FROM
(
    VALUES
        (N'014', N'Drivers', N'2 drivers approaching overtime threshold', N'Shift meal-break coverage before 7:00 PM', N'Warning', 75),
        (N'014', N'Make line', N'Cross-trained cashier pulled to prep', N'Supervisor sign-off pending on labor transfer', N'Info', 60),
        (N'014', N'Front counter', N'One call-out logged after lunch', N'Hold flex labor unless queue exceeds 8 tickets', N'Watch', 45),
        (N'031', N'Drivers', N'Courier printer backlog is slowing reassignment slips', N'Escalate to field support if the next wave misses promise', N'Warning', 40),
        (N'081', N'Carryout', N'Counter clock drift is showing stale ready times', N'Confirm terminal time sync before dinner rush', N'Warning', 30)
) source (StoreNumber, TeamName, ConcernText, ActionRequired, SeverityCode, MinutesAgo)
INNER JOIN dbo.Store s
    ON s.StoreNumber = source.StoreNumber
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.WorkforceAlert target
    WHERE target.StoreId = s.StoreId
      AND target.TeamName = source.TeamName
      AND target.ConcernText = source.ConcernText
);
GO

INSERT INTO dbo.RouteZoneBulletin
(
    StoreId,
    RouteZoneId,
    BulletinText,
    EffectiveUtc,
    ExpiresUtc,
    RequiresAcknowledgement
)
SELECT
    s.StoreId,
    rz.RouteZoneId,
    source.BulletinText,
    DATEADD(MINUTE, source.MinutesAgo * -1, GETUTCDATE()),
    NULL,
    source.RequiresAcknowledgement
FROM
(
    VALUES
        (N'014', N'NW-CORP', N'Store 014 northwest corridor: watch apartment gate codes after 6:30 PM.', 120, CAST(0 AS BIT)),
        (N'014', N'MALL-ANNEX', N'Mall annex route: security desk still prefers paper receipt slips for after-hours drop-offs.', 90, CAST(1 AS BIT)),
        (N'031', N'SECURITY-DESK', N'Mall security desk is batching escort requests in fifteen-minute waves tonight.', 50, CAST(1 AS BIT)),
        (N'057', N'AIRPORT-HOTEL', N'Airport hotel strip: keep insulated bags on hand for stacked catering warm-holds.', 70, CAST(0 AS BIT)),
        (N'081', N'CAMPUS-QUAD', N'Campus quad route: dorm lobby access codes rotate after 8:00 PM.', 55, CAST(0 AS BIT))
) source (StoreNumber, ZoneCode, BulletinText, MinutesAgo, RequiresAcknowledgement)
INNER JOIN dbo.Store s
    ON s.StoreNumber = source.StoreNumber
LEFT JOIN dbo.RouteZone rz
    ON rz.StoreId = s.StoreId
   AND rz.ZoneCode = source.ZoneCode
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.RouteZoneBulletin target
    WHERE target.StoreId = s.StoreId
      AND ISNULL(target.RouteZoneId, 0) = ISNULL(rz.RouteZoneId, 0)
      AND target.BulletinText = source.BulletinText
);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PayrollDailyImport)
BEGIN
    INSERT INTO dbo.PayrollDailyImport
    (
        StoreId,
        WorkDate,
        ScheduledHours,
        WorkedHours,
        OvertimeHours,
        RegularLaborCost,
        OvertimeLaborCost,
        AgencyLaborCost,
        ScheduledDriverSlots,
        FilledDriverSlots,
        OpenDriverSlots,
        CrossTrainedTeamMembers,
        CalloutCount,
        NetSales
    )
    SELECT StoreId, CONVERT(DATE, GETUTCDATE()), 164.00, 171.50, 7.50, 2448.00, 213.75, 96.00, 18, 15, 3, 2, 1, 8200.00
    FROM dbo.Store
    WHERE StoreNumber = N'014'
    UNION ALL
    SELECT StoreId, CONVERT(DATE, GETUTCDATE()), 118.00, 120.50, 2.50, 1711.00, 68.25, 0.00, 12, 11, 1, 1, 0, 5940.00
    FROM dbo.Store
    WHERE StoreNumber = N'022';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PayrollOvertimeWeeklyImport)
BEGIN
    INSERT INTO dbo.PayrollOvertimeWeeklyImport
    (
        StoreId,
        WeekEndingDate,
        DriverOvertimeHours,
        KitchenOvertimeHours,
        ShiftLeadOvertimeHours
    )
    SELECT StoreId, DATEADD(DAY, -21, CONVERT(DATE, GETUTCDATE())), 5.00, 2.00, 1.00 FROM dbo.Store WHERE StoreNumber = N'014'
    UNION ALL
    SELECT StoreId, DATEADD(DAY, -14, CONVERT(DATE, GETUTCDATE())), 4.50, 2.50, 1.25 FROM dbo.Store WHERE StoreNumber = N'014'
    UNION ALL
    SELECT StoreId, DATEADD(DAY, -7, CONVERT(DATE, GETUTCDATE())), 5.75, 2.75, 1.50 FROM dbo.Store WHERE StoreNumber = N'014'
    UNION ALL
    SELECT StoreId, CONVERT(DATE, GETUTCDATE()), 6.00, 3.00, 1.50 FROM dbo.Store WHERE StoreNumber = N'014'
    UNION ALL
    SELECT StoreId, DATEADD(DAY, -7, CONVERT(DATE, GETUTCDATE())), 2.00, 1.50, 0.50 FROM dbo.Store WHERE StoreNumber = N'022'
    UNION ALL
    SELECT StoreId, CONVERT(DATE, GETUTCDATE()), 2.25, 1.25, 0.50 FROM dbo.Store WHERE StoreNumber = N'022';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PayrollTurnoverMonthlyImport)
BEGIN
    INSERT INTO dbo.PayrollTurnoverMonthlyImport
    (
        StoreId,
        SummaryMonth,
        BeginningHeadcount,
        HireCount,
        SeparationCount,
        EndingHeadcount
    )
    SELECT StoreId, DATEFROMPARTS(YEAR(GETUTCDATE()), MONTH(GETUTCDATE()), 1), 27, 3, 2, 28 FROM dbo.Store WHERE StoreNumber = N'014'
    UNION ALL
    SELECT StoreId, DATEFROMPARTS(YEAR(GETUTCDATE()), MONTH(GETUTCDATE()), 1), 19, 1, 1, 19 FROM dbo.Store WHERE StoreNumber = N'022';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'StoreOps\03-seed-data.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'StoreOps\03-seed-data.sql');
END
GO
