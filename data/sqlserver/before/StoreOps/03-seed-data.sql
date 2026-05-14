USE [$(StoreOpsDatabase)];
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Store WHERE StoreNumber = N'014')
BEGIN
    INSERT INTO dbo.Store (StoreNumber, StoreName, RegionCode, PhoneNumber, IsActive)
    VALUES
        (N'014', N'Bellevue Corporate Center', N'NW', N'555-0140', 1),
        (N'022', N'Redmond East', N'NW', N'555-0220', 1);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.RouteZone)
BEGIN
    INSERT INTO dbo.RouteZone (StoreId, ZoneCode, ZoneName, SortOrder)
    SELECT StoreId, N'NW-CORP', N'Northwest Corporate Corridor', 1 FROM dbo.Store WHERE StoreNumber = N'014'
    UNION ALL
    SELECT StoreId, N'MALL-ANNEX', N'Mall Annex', 2 FROM dbo.Store WHERE StoreNumber = N'014'
    UNION ALL
    SELECT StoreId, N'RED-EAST', N'Redmond East Campus', 1 FROM dbo.Store WHERE StoreNumber = N'022';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Driver)
BEGIN
    INSERT INTO dbo.Driver (StoreId, DriverCode, DisplayName, EmploymentStatus, HomeZoneCode, EligibilityExpirationDate)
    SELECT StoreId, N'DRV-17', N'Luis Mendoza', N'Active', N'NW-CORP', DATEADD(DAY, 365, GETUTCDATE()) FROM dbo.Store WHERE StoreNumber = N'014'
    UNION ALL
    SELECT StoreId, N'DRV-03', N'Rachel Kim', N'Active', N'MALL-ANNEX', DATEADD(DAY, 365, GETUTCDATE()) FROM dbo.Store WHERE StoreNumber = N'014'
    UNION ALL
    SELECT StoreId, N'DRV-22', N'Darren Scott', N'Active', N'RED-EAST', DATEADD(DAY, 365, GETUTCDATE()) FROM dbo.Store WHERE StoreNumber = N'022';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DispatchTicket WHERE TicketNumber = 4105)
BEGIN
    INSERT INTO dbo.DispatchTicket (TicketNumber, StoreId, DriverId, RouteZoneId, OrderOrigin, PromiseUtc, StatusCode, TotalAmount, LastUpdatedUtc)
    SELECT 4105, s.StoreId, d.DriverId, rz.RouteZoneId, N'CallCenter', DATEADD(MINUTE, 28, GETUTCDATE()), N'Dispatched', 43.75, GETUTCDATE()
    FROM dbo.Store s
    INNER JOIN dbo.Driver d ON d.DriverCode = N'DRV-17'
    INNER JOIN dbo.RouteZone rz ON rz.ZoneCode = N'NW-CORP' AND rz.StoreId = s.StoreId
    WHERE s.StoreNumber = N'014';

    INSERT INTO dbo.DispatchTicket (TicketNumber, StoreId, DriverId, RouteZoneId, OrderOrigin, PromiseUtc, StatusCode, TotalAmount, LastUpdatedUtc)
    SELECT 4106, s.StoreId, d.DriverId, rz.RouteZoneId, N'WebStorefront', DATEADD(MINUTE, 34, GETUTCDATE()), N'Dispatched', 38.10, GETUTCDATE()
    FROM dbo.Store s
    INNER JOIN dbo.Driver d ON d.DriverCode = N'DRV-03'
    INNER JOIN dbo.RouteZone rz ON rz.ZoneCode = N'MALL-ANNEX' AND rz.StoreId = s.StoreId
    WHERE s.StoreNumber = N'014';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PosOrderImportBatch)
BEGIN
    INSERT INTO dbo.PosOrderImportBatch (StoreId, SourceSystem, BatchDate, ImportedUtc, BatchStatus, ItemCount)
    SELECT StoreId, N'CampusPOS', CONVERT(DATE, GETUTCDATE()), GETUTCDATE(), N'Complete', 57
    FROM dbo.Store
    WHERE StoreNumber = N'014';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'StoreOps\03-seed-data.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'StoreOps\03-seed-data.sql');
END
GO
