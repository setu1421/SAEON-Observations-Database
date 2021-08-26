﻿/*
Deployment script for ObservationsSACTN

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "ObservationsSACTN"
:setvar DefaultFilePrefix "ObservationsSACTN"
:setvar DefaultDataPath "D:\Program Files\Microsoft SQL Server\MSSQL15.SAEON2019\MSSQL\DATA\"
:setvar DefaultLogPath "D:\Program Files\Microsoft SQL Server\MSSQL15.SAEON2019\MSSQL\DATA\"

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


--GO
--IF EXISTS (SELECT 1
--           FROM   [master].[dbo].[sysdatabases]
--           WHERE  [name] = N'$(DatabaseName)')
--    BEGIN
--        ALTER DATABASE [$(DatabaseName)]
--            SET TEMPORAL_HISTORY_RETENTION ON 
--            WITH ROLLBACK IMMEDIATE;
--    END

go
drop view if exists vInventoryDataStreams

GO
PRINT N'Dropping Index [dbo].[Observation].[IX_Observation_ValueDecade]...';


GO
DROP INDEX [IX_Observation_ValueDecade]
    ON [dbo].[Observation];


GO
PRINT N'Dropping Index [dbo].[Observation].[IX_Observation_ValueYear]...';


GO
DROP INDEX [IX_Observation_ValueYear]
    ON [dbo].[Observation];


GO
PRINT N'Altering Table [dbo].[ImportBatchSummary]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TABLE [dbo].[ImportBatchSummary]
    ADD [UnverifiedCount] INT NULL;


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Creating Index [dbo].[ImportBatchSummary].[IX_ImportBatchSummary_UnverifiedCount]...';


GO
CREATE NONCLUSTERED INDEX [IX_ImportBatchSummary_UnverifiedCount]
    ON [dbo].[ImportBatchSummary]([UnverifiedCount] ASC);


GO
PRINT N'Creating Index [dbo].[ImportBatchSummary].[IX_ImportBatchSummary_VerifiedCount]...';


GO
CREATE NONCLUSTERED INDEX [IX_ImportBatchSummary_VerifiedCount]
    ON [dbo].[ImportBatchSummary]([VerifiedCount] ASC);


GO
PRINT N'Altering Table [dbo].[Observation]...';


GO
ALTER TABLE [dbo].[Observation] DROP COLUMN [ValueYear], COLUMN [ValueDecade];


GO
ALTER TABLE [dbo].[Observation]
    ADD [ValueYear]   AS (Year([ValueDate])),
        [ValueDecade] AS (Year([ValueDate]) / 10 * 10);


GO
PRINT N'Creating Index [dbo].[Observation].[IX_Observation_ValueDecade]...';


GO
CREATE NONCLUSTERED INDEX [IX_Observation_ValueDecade]
    ON [dbo].[Observation]([ValueDecade] ASC)
    ON [Observations];


GO
PRINT N'Creating Index [dbo].[Observation].[IX_Observation_ValueYear]...';


GO
CREATE NONCLUSTERED INDEX [IX_Observation_ValueYear]
    ON [dbo].[Observation]([ValueYear] ASC)
    ON [Observations];


GO
PRINT N'Refreshing View [dbo].[vImportBatchSummary]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vImportBatchSummary]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering View [dbo].[vInventoryDatasets]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER VIEW [dbo].[vInventoryDatasets]
AS 
Select
  Row_Number() over (order by StationCode, PhenomenonCode, OfferingCode, UnitOfMeasureCode) ID, s.*
from
(
Select
  OrganisationID, OrganisationCode, OrganisationName, OrganisationDescription, OrganisationUrl,
  ProgrammeID, ProgrammeCode, ProgrammeName, ProgrammeDescription, ProgrammeUrl,
  ProjectID, ProjectCode, ProjectName, ProjectDescription, ProjectUrl,
  SiteID, SiteCode, SiteName, SiteDescription, SiteUrl,
  StationID, StationCode, StationName, StationDescription, StationUrl,
  PhenomenonID, PhenomenonCode, PhenomenonName, PhenomenonDescription, PhenomenonUrl,
  PhenomenonOfferingID, OfferingID, OfferingCode, OfferingName, OfferingDescription,
  PhenomenonUOMID, UnitOfMeasureID, UnitOfMeasureCode, UnitOfMeasureUnit, UnitOfMeasureSymbol,
  Sum(Count) Count,
  Sum(VerifiedCount) VerifiedCount,
  Sum(UnverifiedCount) UnverifiedCount,
  Min(StartDate) StartDate,
  Max(EndDate) EndDate,
  Max(LatitudeNorth) LatitudeNorth,
  Min(LatitudeSouth) LatitudeSouth,
  Min(LongitudeWest) LongitudeWest,
  Max(LongitudeEast) LongitudeEast,
  Min(ElevationMinimum) ElevationMinimum,
  Max(ElevationMaximum) ElevationMaximum
from
  vImportBatchSummary
group by
  OrganisationID, OrganisationCode, OrganisationName, OrganisationDescription, OrganisationUrl,
  ProgrammeID, ProgrammeCode, ProgrammeName, ProgrammeDescription, ProgrammeUrl,
  ProjectID, ProjectCode, ProjectName, ProjectDescription, ProjectUrl,
  SiteID, SiteCode, SiteName, SiteDescription, SiteUrl,
  StationID, StationCode, StationName, StationDescription, StationUrl,
  PhenomenonID, PhenomenonCode, PhenomenonName, PhenomenonDescription, PhenomenonUrl,
  PhenomenonOfferingID, OfferingID, OfferingCode, OfferingName, OfferingDescription,
  PhenomenonUOMID, UnitOfMeasureID, UnitOfMeasureCode, UnitOfMeasureUnit, UnitOfMeasureSymbol
) s
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vInventorySensors]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vInventorySensors]';


GO
PRINT N'Altering View [dbo].[vLocations]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER VIEW [dbo].[vLocations]
AS
Select distinct
  OrganisationID, OrganisationName, OrganisationUrl,
  ProgrammeID, ProgrammeName, ProgrammeUrl,
  ProjectID, ProjectName, ProjectUrl,
  SiteID, SiteName, SiteUrl,
  StationID, StationName, StationUrl,
  [Count],  VerifiedCount, UnverifiedCount, 
  (LatitudeNorth + LatitudeSouth) / 2 Latitude,
  (LongitudeWest + LongitudeEast) / 2 Longitude,
  (ElevationMaximum + ElevationMinimum) / 2 Elevation
from
  vInventoryDatasets
where
  (VerifiedCount > 0) and
  (LatitudeNorth is not null) and (LatitudeSouth is not null) and
  (LongitudeWest is not null) and (LongitudeEast is not null)
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vStationDatasets]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vStationDatasets]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vSensorThingsAPIDatastreams]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPIDatastreams]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vSensorThingsAPILocations]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPILocations]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vSensorThingsAPIObservedProperties]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPIObservedProperties]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vSensorThingsAPISensors]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPISensors]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vSensorThingsAPIThings]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPIThings]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vSensorThingsAPIFeaturesOfInterest]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPIFeaturesOfInterest]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vSensorThingsAPIHistoricalLocations]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPIHistoricalLocations]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vObservationExpansion]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vObservationExpansion]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vSensorThingsAPIObservations]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPIObservations]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vObservation]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vObservation]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vObservationJSON]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vObservationJSON]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vStationObservations]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vStationObservations]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering View [dbo].[vFeatures]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER VIEW [dbo].[vFeatures]
AS 
Select distinct
  PhenomenonID, PhenomenonName, PhenomenonUrl,
  PhenomenonOfferingID, OfferingID, OfferingName,
  PhenomenonUOMID, UnitOfMeasureID, UnitOfMeasureUnit
from
  vInventoryDatasets
where
  (VerifiedCount > 0) and 
  (LatitudeNorth is not null) and (LatitudeSouth is not null) and 
  (LongitudeEast is not null) and (LongitudeWest is not null)
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Update complete.';


GO
