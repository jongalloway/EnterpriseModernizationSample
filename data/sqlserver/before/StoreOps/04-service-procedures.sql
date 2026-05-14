USE [$(StoreOpsDatabase)];
GO

IF OBJECT_ID(N'dbo.usp_DispatchBoard_GetActiveTickets', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_DispatchBoard_GetActiveTickets;
GO

CREATE PROCEDURE dbo.usp_DispatchBoard_GetActiveTickets
    @StoreNumber NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        dt.TicketNumber AS TicketId,
        s.StoreNumber,
        d.DriverCode,
        rz.ZoneName AS RouteZone,
        dt.StatusCode,
        dt.TotalAmount,
        dt.PromiseUtc
    FROM dbo.DispatchTicket dt
    INNER JOIN dbo.Store s
        ON s.StoreId = dt.StoreId
    LEFT JOIN dbo.Driver d
        ON d.DriverId = dt.DriverId
    INNER JOIN dbo.RouteZone rz
        ON rz.RouteZoneId = dt.RouteZoneId
    WHERE s.StoreNumber = @StoreNumber
      AND dt.StatusCode IN (N'Queued', N'Dispatched', N'CompletedLate')
    ORDER BY dt.PromiseUtc, dt.TicketNumber;
END
GO

IF OBJECT_ID(N'dbo.usp_PosImport_GetLatestBatch', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_PosImport_GetLatestBatch;
GO

CREATE PROCEDURE dbo.usp_PosImport_GetLatestBatch
    @StoreNumber NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        pob.PosOrderImportBatchId,
        s.StoreNumber,
        pob.SourceSystem,
        pob.BatchDate,
        pob.ImportedUtc,
        pob.BatchStatus,
        pob.ItemCount
    FROM dbo.PosOrderImportBatch pob
    INNER JOIN dbo.Store s
        ON s.StoreId = pob.StoreId
    WHERE s.StoreNumber = @StoreNumber
    ORDER BY pob.ImportedUtc DESC;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'StoreOps\04-service-procedures.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'StoreOps\04-service-procedures.sql');
END
GO
