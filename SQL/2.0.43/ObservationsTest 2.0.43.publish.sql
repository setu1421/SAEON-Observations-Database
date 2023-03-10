/*
Deployment script for ObservationsTest

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "ObservationsTest"
:setvar DefaultFilePrefix "ObservationsTest"
:setvar DefaultDataPath "D:\Program Files\Microsoft SQL Server\MSSQL14.SAEON\MSSQL\DATA\"
:setvar DefaultLogPath "D:\Program Files\Microsoft SQL Server\MSSQL14.SAEON\MSSQL\DATA\"

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
IF (SELECT is_default
    FROM   [$(DatabaseName)].[sys].[filegroups]
    WHERE  [name] = N'Documents') = 0
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            MODIFY FILEGROUP [Documents] DEFAULT;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET TEMPORAL_HISTORY_RETENTION ON 
            WITH ROLLBACK IMMEDIATE;
    END

GO
PRINT N'Dropping [dbo].[vObservationsList]...';
GO
DROP VIEW vObservationsList

GO
PRINT N'Dropping [dbo].[vApiDataDownload]...';
GO
DROP VIEW vApiDataDownload

GO
PRINT N'Dropping [dbo].[vApiDataQuery]...';
GO
DROP VIEW vApiDataQuery

GO
PRINT N'Dropping [dbo].[vApiInventory]...';
GO
DROP VIEW vApiInventory

GO
PRINT N'Dropping [dbo].[vApiSpacialCoverage]...';
GO
DROP VIEW vApiSpacialCoverage

GO
PRINT N'Dropping [dbo].[vApiTemporalCoverage]...';
GO
DROP VIEW vApiTemporalCoverage

GO
PRINT N'Dropping [dbo].[vInventoryInstruments]...';
GO
DROP VIEW vInventoryInstruments

GO
PRINT N'Dropping [dbo].[vInventoryOrganisations]...';
GO
DROP VIEW vInventoryOrganisations

GO
PRINT N'Dropping [dbo].[vInventoryPhenomenaOfferings]...';
GO
DROP VIEW vInventoryPhenomenaOfferings

GO
PRINT N'Dropping [dbo].[vInventoryStations]...';
GO
DROP VIEW vInventoryStations

GO
PRINT N'Dropping [dbo].[vInventoryTotals]...';
GO
DROP VIEW vInventoryTotals

GO
PRINT N'Dropping [dbo].[vInventoryYears]...';
GO
DROP VIEW vInventoryYears

GO
PRINT N'Altering [dbo].[vInventory]...';


GO
ALTER VIEW [dbo].[vInventory]
AS
Select
  SiteCode, SiteName, StationCode, StationName, InstrumentCode, InstrumentName, SensorCode, SensorName, PhenomenonCode, PhenomenonName, 
  OfferingCode, OfferingName, UnitOfMeasureCode, UnitOfMeasureUnit, Sum(Count) Count, Min(StartDate) StartDate, Max(EndDate) EndDate
from
  vImportBatchSummary
group by
  SiteCode, SiteName, StationCode, StationName, InstrumentCode, InstrumentName, SensorCode, SensorName, PhenomenonCode, PhenomenonName, 
  OfferingCode, OfferingName, UnitOfMeasureCode, UnitOfMeasureUnit
GO
PRINT N'Update complete.';


GO
