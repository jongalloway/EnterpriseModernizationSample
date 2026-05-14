USE [master];
GO

IF DB_ID(N'$(ReportingDatabase)') IS NULL
BEGIN
    PRINT 'Creating $(ReportingDatabase).';
    CREATE DATABASE [$(ReportingDatabase)];
END
GO

ALTER DATABASE [$(ReportingDatabase)] SET RECOVERY SIMPLE WITH NO_WAIT;
GO

USE [$(ReportingDatabase)];
GO

IF OBJECT_ID(N'dbo.DatabaseDeploymentHistory', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DatabaseDeploymentHistory
    (
        DatabaseDeploymentHistoryId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ScriptName NVARCHAR(200) NOT NULL,
        AppliedBy NVARCHAR(128) NOT NULL CONSTRAINT DF_ReportingDeploymentHistory_AppliedBy DEFAULT (SUSER_SNAME()),
        AppliedUtc DATETIME NOT NULL CONSTRAINT DF_ReportingDeploymentHistory_AppliedUtc DEFAULT (GETUTCDATE())
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DatabaseDeploymentHistory WHERE ScriptName = N'Reporting\01-create-database.sql')
BEGIN
    INSERT INTO dbo.DatabaseDeploymentHistory (ScriptName)
    VALUES (N'Reporting\01-create-database.sql');
END
GO
