USE [$(CustomerHubDatabase)];
GO

IF OBJECT_ID(N'dbo.usp_PartnerSync_BuildStoreOpsExtract', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_PartnerSync_BuildStoreOpsExtract;
GO

CREATE PROCEDURE dbo.usp_PartnerSync_BuildStoreOpsExtract
    @SyncBatchId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @SyncBatchId IS NULL
    BEGIN
        SET @SyncBatchId = NEWID();
    END

    DELETE FROM dbo.PartnerAccountExtract
    WHERE SyncBatchId = @SyncBatchId;

    INSERT INTO dbo.PartnerAccountExtract
    (
        SyncBatchId,
        PartnerCode,
        PartnerName,
        RelationshipTier,
        PreferredStoreNumber
    )
    SELECT
        @SyncBatchId,
        pa.PartnerCode,
        pa.PartnerName,
        pa.RelationshipTier,
        pa.PreferredStoreNumber
    FROM dbo.PartnerAccount pa
    WHERE pa.StatusCode = N'Active';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'CustomerHub\05-migration-procedures.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'CustomerHub\05-migration-procedures.sql');
END
GO
