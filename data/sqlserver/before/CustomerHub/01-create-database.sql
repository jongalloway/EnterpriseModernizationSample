USE [master];
GO

IF DB_ID(N'$(CustomerHubDatabase)') IS NULL
BEGIN
    PRINT 'Creating $(CustomerHubDatabase).';
    CREATE DATABASE [$(CustomerHubDatabase)];
END
GO

ALTER DATABASE [$(CustomerHubDatabase)] SET RECOVERY SIMPLE WITH NO_WAIT;
GO

USE [$(CustomerHubDatabase)];
GO

IF OBJECT_ID(N'dbo.DatabaseDeploymentHistory', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DatabaseDeploymentHistory
    (
        DatabaseDeploymentHistoryId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ScriptName NVARCHAR(200) NOT NULL,
        AppliedBy NVARCHAR(128) NOT NULL CONSTRAINT DF_DatabaseDeploymentHistory_AppliedBy DEFAULT (SUSER_SNAME()),
        AppliedUtc DATETIME NOT NULL CONSTRAINT DF_DatabaseDeploymentHistory_AppliedUtc DEFAULT (GETUTCDATE())
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'CustomerHub\01-create-database.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'CustomerHub\01-create-database.sql');
END
GO
