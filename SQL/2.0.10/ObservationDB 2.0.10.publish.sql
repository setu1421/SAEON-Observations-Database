/*
Deployment script for ObservationDB

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "ObservationDB"
:setvar DefaultFilePrefix "ObservationDB"
:setvar DefaultDataPath "C:\Program Files\Microsoft SQL Server\MSSQL13.SAEON\MSSQL\DATA\"
:setvar DefaultLogPath "C:\Program Files\Microsoft SQL Server\MSSQL13.SAEON\MSSQL\DATA\"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO
PRINT N'Altering [dbo].[Observation]...';


GO
ALTER TABLE [dbo].[Observation] ALTER COLUMN [Comment] ADD SPARSE;


GO
PRINT N'Creating [dbo].[Observation].[IX_Observation_Comment]...';


GO
CREATE NONCLUSTERED INDEX [IX_Observation_Comment]
    ON [dbo].[Observation]([Comment] ASC) WHERE Comment is not null;


GO
PRINT N'Creating [dbo].[Observation].[IX_Observation_Comment_Null]...';


GO
CREATE NONCLUSTERED INDEX [IX_Observation_Comment_Null]
    ON [dbo].[Observation]([Comment] ASC) WHERE Comment is null;


GO
PRINT N'Refreshing [dbo].[vInventory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vInventory]';


GO
PRINT N'Refreshing [dbo].[vObservation]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vObservation]';


GO
PRINT N'Refreshing [dbo].[vObservationRoles]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vObservationRoles]';


GO
PRINT N'Refreshing [dbo].[progress_Status_Raw]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[progress_Status_Raw]';


GO
PRINT N'Update complete.';


GO
