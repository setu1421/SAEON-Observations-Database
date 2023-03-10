/*
Deployment script for ObservationsElwandle

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "ObservationsElwandle"
:setvar DefaultFilePrefix "ObservationsElwandle"
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
PRINT N'Dropping Index [dbo].[Observation].[IX_Observation_ImportBatchID]...';


GO
DROP INDEX [IX_Observation_ImportBatchID]
    ON [dbo].[Observation];


GO
PRINT N'Dropping Index [dbo].[Observation].[IX_Observation_SensorID]...';


GO
DROP INDEX [IX_Observation_SensorID]
    ON [dbo].[Observation];


GO
PRINT N'Dropping Index [dbo].[Observation].[IX_Observation_SensorID_ValueDate]...';


GO
DROP INDEX [IX_Observation_SensorID_ValueDate]
    ON [dbo].[Observation];


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
PRINT N'Creating Index [dbo].[Observation].[IX_Observation_ImportBatchID]...';


GO
CREATE NONCLUSTERED INDEX [IX_Observation_ImportBatchID]
    ON [dbo].[Observation]([ImportBatchID] ASC)
    ON [Observations];


GO
PRINT N'Creating Index [dbo].[Observation].[IX_Observation_SensorID]...';


GO
CREATE NONCLUSTERED INDEX [IX_Observation_SensorID]
    ON [dbo].[Observation]([SensorID] ASC)
    ON [Observations];


GO
PRINT N'Refreshing View [dbo].[vObservationExpansion]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vObservationExpansion]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vSensorThingsAPIObservations]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPIObservations]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vObservation]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vObservation]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vObservationJSON]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vObservationJSON]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vStationObservations]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vStationObservations]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering View [dbo].[vDatasetsExpansion]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
ALTER VIEW [dbo].[vDatasetsExpansion]
AS
Select Distinct
  Datasets.*,
  Organisation.ID OrganisationID, Organisation.Code OrganisationCode, Organisation.Name OrganisationName, Organisation.Description OrganisationDescription, Organisation.Url OrganisationUrl,
  Programme.ID ProgrammeID, Programme.Code ProgrammeCode, Programme.Name ProgrammeName, Programme.Description ProgrammeDescription, Programme.Url ProgrammeUrl,
  Project.ID ProjectID, Project.Code ProjectCode, Project.Name ProjectName, Project.Description ProjectDescription, Project.Url ProjectUrl,
  Site.ID SiteID, Site.Code SiteCode, Site.Name SiteName, Site.Description SiteDescription, Site.Url SiteUrl,
  Station.Code StationCode, Station.Name StationName, Station.Description StationDescription, Station.Url StationUrl,
  Phenomenon.ID PhenomenonID, Phenomenon.Code PhenomenonCode, Phenomenon.Name PhenomenonName, Phenomenon.Description PhenomenonDescription, Phenomenon.Url PhenomenonUrl,
  Offering.ID OfferingID, Offering.Code OfferingCode, Offering.Name OfferingName, Offering.Description OfferingDescription,
  UnitOfMeasure.ID UnitOfMeasureID, UnitOfMeasure.Code UnitOfMeasureCode, UnitOfMeasure.Unit UnitOfMeasureUnit, UnitOfMeasure.UnitSymbol UnitOfMeasureSymbol,
  Cast((case when (Organisation.Code in ('SAEON','SMCRI','EFTEON')) and (Programme.Code <> 'SACTN') and (VerifiedCount > 0) and (LatitudeNorth is not null) and (LatitudeSouth is not null) and (LongitudeWest is not null) and (LongitudeEast is not null) then 1 else 0 end) as bit) IsValid
from
  Datasets
  inner join Station
    on (Datasets.StationID  = Station.ID)
  inner join Site
    on (Station.SiteID = Site.ID)
  inner join PhenomenonOffering
    on (Datasets.PhenomenonOfferingID = PhenomenonOffering.ID)
  inner join Phenomenon
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
  inner join PhenomenonUOM
    on (Datasets.PhenomenonUOMID = PhenomenonUOM.ID)
  inner join UnitOfMeasure
    on (PhenomenonUOM.UnitOfMeasureID = UnitOfMeasure.ID)
  left join Project_Station
    on (Project_Station.StationID = Station.ID)
  left join Project
    on (Project_Station.ProjectID = Project.ID)
  left join Programme
    on (Project.ProgrammeID = Programme.ID)
  left join vStationOrganisation
    on (vStationOrganisation.StationID = Station.ID)
  left join Organisation
    on (vStationOrganisation.OrganisationID = Organisation.ID)
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering View [dbo].[vLocations]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
ALTER VIEW [dbo].[vLocations]
AS
Select distinct
  OrganisationID, OrganisationName, OrganisationUrl,
  ProgrammeID, ProgrammeName, ProgrammeUrl,
  ProjectID, ProjectName, ProjectUrl,
  SiteID, SiteName, SiteUrl,
  StationID, StationName, StationUrl,
  --[Count],  VerifiedCount, UnverifiedCount, 
  (LatitudeNorth + LatitudeSouth) / 2 Latitude,
  (LongitudeWest + LongitudeEast) / 2 Longitude,
  (ElevationMaximum + ElevationMinimum) / 2 Elevation
from
  vDatasetsExpansion
where
  (IsValid = 1)
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering View [dbo].[vVariables]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
ALTER VIEW [dbo].[vVariables]
AS 
Select distinct
  PhenomenonID, PhenomenonName, PhenomenonUrl,
  PhenomenonOfferingID, OfferingID, OfferingName,
  PhenomenonUOMID, UnitOfMeasureID, UnitOfMeasureUnit
from
  vDatasetsExpansion
where
  (IsValid = 1)
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering View [dbo].[vInventoryDatasets]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
ALTER VIEW [dbo].[vInventoryDatasets]
AS 
Select
  Row_Number() over (order by StationCode, PhenomenonCode, OfferingCode, UnitOfMeasureCode) ID, s.*,
  Cast((case when (OrganisationCode in ('SAEON','SMCRI','EFTEON')) and (ProgrammeCode <> 'SACTN') and (VerifiedCount > 0) and (LatitudeNorth is not null) and (LatitudeSouth is not null) and (LongitudeWest is not null) and (LongitudeEast is not null) then 1 else 0 end) as bit) IsValid
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
  Sum(ValueCount) ValueCount,
  Sum(NullCount) NullCount,
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
PRINT N'Altering View [dbo].[vInventorySensors]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
ALTER VIEW [dbo].[vInventorySensors]
AS
Select
  Row_Number() over (order by SiteName, StationName, InstrumentName, SensorName, PhenomenonName, OfferingName, UnitOfMeasureUnit) ID, s.*,
  Cast((case when (OrganisationCode in ('SAEON','SMCRI','EFTEON')) and (ProgrammeCode <> 'SACTN') and (VerifiedCount > 0) and (LatitudeNorth is not null) and (LatitudeSouth is not null) and (LongitudeWest is not null) and (LongitudeEast is not null) then 1 else 0 end) as bit) IsValid
from
(
Select
  OrganisationID, OrganisationCode, OrganisationName, OrganisationDescription, OrganisationUrl,
  ProgrammeID, ProgrammeCode, ProgrammeName, ProgrammeDescription, ProgrammeUrl,
  ProjectID, ProjectCode, ProjectName, ProjectDescription, ProjectUrl,
  SiteID, SiteCode, SiteName, SiteDescription, SiteUrl,
  StationID, StationCode, StationName, StationDescription, StationUrl,
  InstrumentID, InstrumentCode, InstrumentName, InstrumentDescription, InstrumentUrl,
  SensorID, SensorCode, SensorName, SensorDescription, SensorUrl,
  PhenomenonID, PhenomenonCode, PhenomenonName, PhenomenonDescription, PhenomenonUrl,
  PhenomenonOfferingID, OfferingCode, OfferingName, OfferingDescription,
  PhenomenonUOMID, UnitOfMeasureCode, UnitOfMeasureUnit, UnitOfMeasureSymbol,
  Sum(Count) Count, 
  Sum(ValueCount) ValueCount,
  Sum(NullCount) NullCount,
  Sum(VerifiedCount) VerifiedCount,
  Sum(UnverifiedCount) UnverifiedCount,
  Min(StartDate) StartDate, Max(EndDate) EndDate,
  Max(LatitudeNorth) LatitudeNorth, Min(LatitudeSouth) LatitudeSouth,
  Min(LongitudeWest) LongitudeWest, Max(LongitudeEast) LongitudeEast
from
  vImportBatchSummary
group by
  OrganisationID, OrganisationCode, OrganisationName, OrganisationDescription, OrganisationUrl,
  ProgrammeID, ProgrammeCode, ProgrammeName, ProgrammeDescription, ProgrammeUrl,
  ProjectID, ProjectCode, ProjectName, ProjectDescription, ProjectUrl,
  SiteID, SiteCode, SiteName, SiteDescription, SiteUrl,
  StationID, StationCode, StationName, StationDescription, StationUrl,
  InstrumentID, InstrumentCode, InstrumentName, InstrumentDescription, InstrumentUrl,
  SensorID, SensorCode, SensorName, SensorDescription, SensorUrl,
  PhenomenonID, PhenomenonCode, PhenomenonName, PhenomenonDescription, PhenomenonUrl,
  PhenomenonOfferingID, OfferingCode, OfferingName, OfferingDescription,
  PhenomenonUOMID, UnitOfMeasureCode, UnitOfMeasureUnit, UnitOfMeasureSymbol
) s
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vSensorThingsAPIDatastreams]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPIDatastreams]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vSensorThingsAPILocations]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPILocations]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vSensorThingsAPIObservedProperties]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPIObservedProperties]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vSensorThingsAPISensors]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPISensors]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vSensorThingsAPIThings]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPIThings]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vSensorThingsAPIFeaturesOfInterest]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPIFeaturesOfInterest]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing View [dbo].[vSensorThingsAPIHistoricalLocations]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPIHistoricalLocations]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering View [dbo].[vInventorySnapshots]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
ALTER VIEW [dbo].[vInventorySnapshots]
as
with VerifiedDatasets as
(
Select 
	* 
from 
	vDatasetsExpansion
where
  (IsValid = 1)
	--(VerifiedCount > 0)  and 
	--(LatitudeNorth is not null) and (LatitudeSouth is not null) and 
	--(LongitudeEast is not null) and (LongitudeWest is not null)
),
VerifiedImportBatchSummaries as
(
Select
	*
from
	vImportBatchSummary
where
    (OrganisationCode in ('SAEON','SMCRI','EFTEON')) and (ProgrammeCode <> 'SACTN') and 
	(VerifiedCount > 0)  and 
	(LatitudeNorth is not null) and (LatitudeSouth is not null) and 
	(LongitudeEast is not null) and (LongitudeWest is not null)
)
Select 
	(Select Count(distinct OrganisationCode) from VerifiedDatasets) Organisations,
	(Select Count(distinct ProgrammeCode) from VerifiedDatasets) Programmes,
	(Select Count(distinct ProjectCode) from VerifiedDatasets) Projects,
	(Select Count(distinct SiteCode) from VerifiedDatasets) Sites,
	(Select Count(distinct StationCode) from VerifiedDatasets) Stations,
	(Select Count(distinct InstrumentCode) from VerifiedImportBatchSummaries) Instruments,
	(Select Count(distinct SensorCode) from VerifiedImportBatchSummaries) Sensors,
	(Select Count(distinct PhenomenonCode) from VerifiedDatasets) Phenomena,
	(Select Count(distinct OfferingCode) from VerifiedDatasets) Offerings,
	(Select Count(distinct UnitOfMeasureCode) from VerifiedDatasets) UnitsOfMeasure,
	(Select Count(*) from vVariables) Variables,
	(Select Count(*) from VerifiedDatasets) Datasets,
	(Select Sum(VerifiedCount) from VerifiedImportBatchSummaries) Observations,
	(Select Count(*) from UserDownloads) Downloads
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing Procedure [dbo].[spCreateImportBatchSummaries]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[spCreateImportBatchSummaries]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing Procedure [dbo].[spCreateInventorySnapshot]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[spCreateInventorySnapshot]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Update complete.';


GO
