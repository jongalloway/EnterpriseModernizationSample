USE [$(StoreOpsDatabase)];
GO

IF OBJECT_ID(N'dbo.usp_PartnerSync_ApplyCustomerHubExtract', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_PartnerSync_ApplyCustomerHubExtract;
GO

CREATE PROCEDURE dbo.usp_PartnerSync_ApplyCustomerHubExtract
    @SyncBatchId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    IF @SyncBatchId IS NULL
    BEGIN
        RAISERROR('A sync batch identifier is required.', 16, 1);
        RETURN;
    END

    UPDATE target
    SET
        target.PartnerName = source.PartnerName,
        target.RelationshipTier = source.RelationshipTier,
        target.PreferredStoreNumber = source.PreferredStoreNumber,
        target.LastSyncUtc = source.ExtractedUtc
    FROM dbo.PartnerAccountCache target
    INNER JOIN [$(CustomerHubDatabase)].dbo.PartnerAccountExtract source
        ON source.PartnerCode = target.PartnerCode
    WHERE source.SyncBatchId = @SyncBatchId;

    INSERT INTO dbo.PartnerAccountCache
    (
        PartnerCode,
        PartnerName,
        RelationshipTier,
        PreferredStoreNumber,
        LastSyncUtc
    )
    SELECT
        source.PartnerCode,
        source.PartnerName,
        source.RelationshipTier,
        source.PreferredStoreNumber,
        source.ExtractedUtc
    FROM [$(CustomerHubDatabase)].dbo.PartnerAccountExtract source
    WHERE source.SyncBatchId = @SyncBatchId
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.PartnerAccountCache target
          WHERE target.PartnerCode = source.PartnerCode
      );

    DELETE target
    FROM dbo.PartnerAccountCache target
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [$(CustomerHubDatabase)].dbo.PartnerAccountExtract source
        WHERE source.SyncBatchId = @SyncBatchId
          AND source.PartnerCode = target.PartnerCode
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'StoreOps\05-migration-procedures.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'StoreOps\05-migration-procedures.sql');
END
GO
