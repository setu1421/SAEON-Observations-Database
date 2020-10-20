﻿/*
Deployment script for Observations

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "Observations"
:setvar DefaultFilePrefix "Observations"
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
/*
The column [dbo].[DigitalObjectIdentifiers].[Code] on table [dbo].[DigitalObjectIdentifiers] must be added, but the column has no default value and does not allow NULL values. If the table contains data, the ALTER script will not work. To avoid this issue you must either: add a default value to the column, mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.

The column [dbo].[DigitalObjectIdentifiers].[DOIType] on table [dbo].[DigitalObjectIdentifiers] must be added, but the column has no default value and does not allow NULL values. If the table contains data, the ALTER script will not work. To avoid this issue you must either: add a default value to the column, mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.

The column [dbo].[DigitalObjectIdentifiers].[MetadataHtml] on table [dbo].[DigitalObjectIdentifiers] must be added, but the column has no default value and does not allow NULL values. If the table contains data, the ALTER script will not work. To avoid this issue you must either: add a default value to the column, mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.

The column [dbo].[DigitalObjectIdentifiers].[MetadataJson] on table [dbo].[DigitalObjectIdentifiers] must be added, but the column has no default value and does not allow NULL values. If the table contains data, the ALTER script will not work. To avoid this issue you must either: add a default value to the column, mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.

The column [dbo].[DigitalObjectIdentifiers].[MetadataJsonSha256] on table [dbo].[DigitalObjectIdentifiers] must be added, but the column has no default value and does not allow NULL values. If the table contains data, the ALTER script will not work. To avoid this issue you must either: add a default value to the column, mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.

The column [dbo].[DigitalObjectIdentifiers].[MetadataUrl] on table [dbo].[DigitalObjectIdentifiers] must be added, but the column has no default value and does not allow NULL values. If the table contains data, the ALTER script will not work. To avoid this issue you must either: add a default value to the column, mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.

The column Name on table [dbo].[DigitalObjectIdentifiers] must be changed from NULL to NOT NULL. If the table contains data, the ALTER script may not work. To avoid this issue, you must add values to this column for all rows or mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.

The type for column Name in table [dbo].[DigitalObjectIdentifiers] is currently  VARCHAR (1000) NULL but is being changed to  VARCHAR (500) NOT NULL. Data loss could occur.
*/

--IF EXISTS (select top 1 1 from [dbo].[DigitalObjectIdentifiers])
--    RAISERROR (N'Rows were detected. The schema update is terminating because data loss might occur.', 16, 127) WITH NOWAIT

GO
PRINT N'Dropping [dbo].[DigitalObjectIdentifiers].[IX_DigitalObjectIdentifiers_Name]...';


GO
DROP INDEX [IX_DigitalObjectIdentifiers_Name]
    ON [dbo].[DigitalObjectIdentifiers];


GO
PRINT N'Dropping [dbo].[Observation].[IX_Observation_ValueDecade]...';


GO
DROP INDEX [IX_Observation_ValueDecade]
    ON [dbo].[Observation];


GO
PRINT N'Dropping [dbo].[Observation].[IX_Observation_ValueYear]...';


GO
DROP INDEX [IX_Observation_ValueYear]
    ON [dbo].[Observation];


GO
PRINT N'Dropping [dbo].[Sensor].[IX_Sensor_CodeName]...';


GO
DROP INDEX [IX_Sensor_CodeName]
    ON [dbo].[Sensor];


GO
PRINT N'Dropping [dbo].[Observation].[IX_Observation_ValueDateDesc]...';


GO
DROP INDEX [IX_Observation_ValueDateDesc]
    ON [dbo].[Observation];


GO
PRINT N'Dropping [dbo].[UX_Sensor_Code]...';


GO
ALTER TABLE [dbo].[Sensor] DROP CONSTRAINT [UX_Sensor_Code];


GO
PRINT N'Altering [dbo].[DigitalObjectIdentifiers]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TABLE [dbo].[DigitalObjectIdentifiers] ALTER COLUMN [Name] VARCHAR (500) NOT NULL;


GO
ALTER TABLE [dbo].[DigitalObjectIdentifiers]
    ADD [AlternateID]            UNIQUEIDENTIFIER CONSTRAINT [DF_DigitalObjectIdentifiers_AlternateID] DEFAULT NewId() NULL,
        [ParentID]               INT              NULL,
        [DOIType]                TINYINT          NOT NULL,
        [Code]                   VARCHAR (200)    NOT NULL,
        [MetadataJson]           VARCHAR (MAX)    NOT NULL,
        [MetadataJsonSha256]     BINARY (32)      NOT NULL,
        [MetadataHtml]           VARCHAR (MAX)    NOT NULL,
        [MetadataUrl]            VARCHAR (250)    NOT NULL,
        [ObjectStoreUrl]         VARCHAR (250)    NULL,
        [QueryUrl]               VARCHAR (250)    NULL,
        [ODPMetadataID]          UNIQUEIDENTIFIER NULL,
        [ODPMetadataNeedsUpdate] BIT              NULL,
        [ODPMetadataIsValid]     BIT              NULL,
        [ODPMetadataErrors]      VARCHAR (MAX)    NULL;


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Creating [dbo].[UX_DigitalObjectIdentifiers_DOIType_Code]...';


GO
ALTER TABLE [dbo].[DigitalObjectIdentifiers]
    ADD CONSTRAINT [UX_DigitalObjectIdentifiers_DOIType_Code] UNIQUE NONCLUSTERED ([DOIType] ASC, [Code] ASC);


GO
PRINT N'Creating [dbo].[UX_DigitalObjectIdentifiers_DOIType_Name]...';


GO
ALTER TABLE [dbo].[DigitalObjectIdentifiers]
    ADD CONSTRAINT [UX_DigitalObjectIdentifiers_DOIType_Name] UNIQUE NONCLUSTERED ([DOIType] ASC, [Name] ASC);


GO
PRINT N'Creating [dbo].[DigitalObjectIdentifiers].[IX_DigitalObjectIdentifiers_DOIType]...';


GO
CREATE NONCLUSTERED INDEX [IX_DigitalObjectIdentifiers_DOIType]
    ON [dbo].[DigitalObjectIdentifiers]([DOIType] ASC);


GO
PRINT N'Creating [dbo].[DigitalObjectIdentifiers].[IX_DigitalObjectIdentifiers_ParentID]...';


GO
CREATE NONCLUSTERED INDEX [IX_DigitalObjectIdentifiers_ParentID]
    ON [dbo].[DigitalObjectIdentifiers]([ParentID] ASC);


GO
PRINT N'Altering [dbo].[ImportBatchSummary]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TABLE [dbo].[ImportBatchSummary]
    ADD [DigitalObjectIdentifierID] INT NULL;


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Creating [dbo].[ImportBatchSummary].[IX_ImportBatchSummary_DigitalObjectIdentifierID]...';


GO
CREATE NONCLUSTERED INDEX [IX_ImportBatchSummary_DigitalObjectIdentifierID]
    ON [dbo].[ImportBatchSummary]([DigitalObjectIdentifierID] ASC);


GO
PRINT N'Altering [dbo].[Observation]...';


GO
ALTER TABLE [dbo].[Observation] DROP COLUMN [ValueYear], COLUMN [ValueDecade];


GO
ALTER TABLE [dbo].[Observation]
    ADD [ValueYear]   AS (Year([ValueDate])),
        [ValueDecade] AS (Year([ValueDate]) / 10 * 10);


GO
PRINT N'Creating [dbo].[Observation].[IX_Observation_ValueDecade]...';


GO
CREATE NONCLUSTERED INDEX [IX_Observation_ValueDecade]
    ON [dbo].[Observation]([ValueDecade] ASC)
    ON [Observations];


GO
PRINT N'Creating [dbo].[Observation].[IX_Observation_ValueYear]...';


GO
CREATE NONCLUSTERED INDEX [IX_Observation_ValueYear]
    ON [dbo].[Observation]([ValueYear] ASC)
    ON [Observations];


GO
PRINT N'Creating [dbo].[Observation].[IX_Observation_SensorID_PhenomenonOfferingID_PhenomenonUOMID_ImportBatchID]...';


GO
CREATE NONCLUSTERED INDEX [IX_Observation_SensorID_PhenomenonOfferingID_PhenomenonUOMID_ImportBatchID]
    ON [dbo].[Observation]([SensorID] ASC, [PhenomenonOfferingID] ASC, [PhenomenonUOMID] ASC, [ImportBatchID] ASC)
    ON [Observations];


GO
PRINT N'Altering [dbo].[Organisation]...';


GO
ALTER TABLE [dbo].[Organisation]
    ADD [DigitalObjectIdentifierID] INT NULL;


GO
PRINT N'Creating [dbo].[Organisation].[IX_Organisation_DigitalObjectIdentifierID]...';


GO
CREATE NONCLUSTERED INDEX [IX_Organisation_DigitalObjectIdentifierID]
    ON [dbo].[Organisation]([DigitalObjectIdentifierID] ASC);


GO
PRINT N'Altering [dbo].[Programme]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TABLE [dbo].[Programme]
    ADD [DigitalObjectIdentifierID] INT NULL;


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Creating [dbo].[Programme].[IX_Programme_DigitalObjectIdentifierID]...';


GO
CREATE NONCLUSTERED INDEX [IX_Programme_DigitalObjectIdentifierID]
    ON [dbo].[Programme]([DigitalObjectIdentifierID] ASC);


GO
PRINT N'Altering [dbo].[Project]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TABLE [dbo].[Project]
    ADD [DigitalObjectIdentifierID] INT NULL;


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Creating [dbo].[Project].[IX_Project_DigitalObjectIdentifierID]...';


GO
CREATE NONCLUSTERED INDEX [IX_Project_DigitalObjectIdentifierID]
    ON [dbo].[Project]([DigitalObjectIdentifierID] ASC);


GO
PRINT N'Altering [dbo].[Sensor]...';


GO
ALTER TABLE [dbo].[Sensor] ALTER COLUMN [Code] VARCHAR (75) NOT NULL;


GO
PRINT N'Creating [dbo].[UX_Sensor_Code]...';


GO
ALTER TABLE [dbo].[Sensor]
    ADD CONSTRAINT [UX_Sensor_Code] UNIQUE NONCLUSTERED ([Code] ASC);


GO
PRINT N'Creating [dbo].[UX_Sensor_Name]...';


GO
ALTER TABLE [dbo].[Sensor]
    ADD CONSTRAINT [UX_Sensor_Name] UNIQUE NONCLUSTERED ([Name] ASC);


GO
PRINT N'Creating [dbo].[Sensor].[IX_Sensor_CodeName]...';


GO
CREATE NONCLUSTERED INDEX [IX_Sensor_CodeName]
    ON [dbo].[Sensor]([Code] ASC, [Name] ASC);


GO
PRINT N'Altering [dbo].[Site]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TABLE [dbo].[Site]
    ADD [DigitalObjectIdentifierID] INT NULL;


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Creating [dbo].[Site].[IX_Site_DigitalObjectIdentifierID]...';


GO
CREATE NONCLUSTERED INDEX [IX_Site_DigitalObjectIdentifierID]
    ON [dbo].[Site]([DigitalObjectIdentifierID] ASC);


GO
PRINT N'Altering [dbo].[Station]...';


GO
ALTER TABLE [dbo].[Station]
    ADD [DigitalObjectIdentifierID] INT NULL;


GO
PRINT N'Creating [dbo].[Station].[IX_Station_DigitalObjectIdentifierID]...';


GO
CREATE NONCLUSTERED INDEX [IX_Station_DigitalObjectIdentifierID]
    ON [dbo].[Station]([DigitalObjectIdentifierID] ASC);


GO
PRINT N'Creating [dbo].[FK_DigitalObjectIdentifiers_ParentID]...';


GO
ALTER TABLE [dbo].[DigitalObjectIdentifiers] WITH NOCHECK
    ADD CONSTRAINT [FK_DigitalObjectIdentifiers_ParentID] FOREIGN KEY ([ParentID]) REFERENCES [dbo].[DigitalObjectIdentifiers] ([ID]);


GO
PRINT N'Creating [dbo].[FK_ImportBatchSummary_DigitalObjectIdentifierID]...';


GO
ALTER TABLE [dbo].[ImportBatchSummary] WITH NOCHECK
    ADD CONSTRAINT [FK_ImportBatchSummary_DigitalObjectIdentifierID] FOREIGN KEY ([DigitalObjectIdentifierID]) REFERENCES [dbo].[DigitalObjectIdentifiers] ([ID]);


GO
PRINT N'Creating [dbo].[FK_Organisation_DigitalObjectIdentifierID]...';


GO
ALTER TABLE [dbo].[Organisation] WITH NOCHECK
    ADD CONSTRAINT [FK_Organisation_DigitalObjectIdentifierID] FOREIGN KEY ([DigitalObjectIdentifierID]) REFERENCES [dbo].[DigitalObjectIdentifiers] ([ID]);


GO
PRINT N'Creating [dbo].[FK_Programme_DigitalObjectIdentifierID]...';


GO
ALTER TABLE [dbo].[Programme] WITH NOCHECK
    ADD CONSTRAINT [FK_Programme_DigitalObjectIdentifierID] FOREIGN KEY ([DigitalObjectIdentifierID]) REFERENCES [dbo].[DigitalObjectIdentifiers] ([ID]);


GO
PRINT N'Creating [dbo].[FK_Project_DigitalObjectIdentifierID]...';


GO
ALTER TABLE [dbo].[Project] WITH NOCHECK
    ADD CONSTRAINT [FK_Project_DigitalObjectIdentifierID] FOREIGN KEY ([DigitalObjectIdentifierID]) REFERENCES [dbo].[DigitalObjectIdentifiers] ([ID]);


GO
PRINT N'Creating [dbo].[FK_Site_DigitalObjectIdentifierID]...';


GO
ALTER TABLE [dbo].[Site] WITH NOCHECK
    ADD CONSTRAINT [FK_Site_DigitalObjectIdentifierID] FOREIGN KEY ([DigitalObjectIdentifierID]) REFERENCES [dbo].[DigitalObjectIdentifiers] ([ID]);


GO
PRINT N'Creating [dbo].[FK_Station_DigitalObjectIdentifierID]...';


GO
ALTER TABLE [dbo].[Station] WITH NOCHECK
    ADD CONSTRAINT [FK_Station_DigitalObjectIdentifierID] FOREIGN KEY ([DigitalObjectIdentifierID]) REFERENCES [dbo].[DigitalObjectIdentifiers] ([ID]);


GO
PRINT N'Refreshing [dbo].[vUserDownloads]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vUserDownloads]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vObservationExpansion]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vObservationExpansion]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vSensorThingsAPIObservations]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPIObservations]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vObservation]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vObservation]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vObservationJSON]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vObservationJSON]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vStationObservations]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vStationObservations]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vInstrumentOrganisation]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vInstrumentOrganisation]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vOrganisationInstrument]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vOrganisationInstrument]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vOrganisationSite]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vOrganisationSite]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vOrganisationStation]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vOrganisationStation]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vSiteOrganisation]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSiteOrganisation]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vStationOrganisation]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vStationOrganisation]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vProject]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vProject]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vProjectStation]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vProjectStation]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vDataLog]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vDataLog]';


GO
PRINT N'Refreshing [dbo].[vInstrumentSensor]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vInstrumentSensor]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vSensor]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensor]';


GO
PRINT N'Refreshing [dbo].[vSensorDates]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorDates]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vSensorLocation]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorLocation]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vStation]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vStation]';


GO
PRINT N'Refreshing [dbo].[vSensorThingsAPIInstrumentDates]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPIInstrumentDates]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vSensorThingsAPIStationDates]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPIStationDates]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vStationInstrument]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vStationInstrument]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[vImportBatchSummary]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER VIEW [dbo].[vImportBatchSummary]
AS 
Select
  ImportBatchSummary.*, 
 -- case when Exists(
	--Select 
	--  * 
	--from 
	--  Observation
	--  inner join Status
	--    on (Observation.StatusID = Status.ID)
 --   where 
 --     ((Observation.ImportBatchID = ImportBatchSummary.ImportBatchID) and 
	--   (Observation.SensorID = ImportBatchSummary.SensorID) and
 --      (Observation.PhenomenonOfferingID = ImportBatchSummary.PhenomenonOfferingID) and
 --      (Observation.PhenomenonUOMID = ImportBatchSummary.PhenomenonUOMID) and
	--   (Status.Name = 'Verified'))
 -- ) then 1
 -- else 0
 -- end HasVerified, 
 -- (
 -- Select 
 --   Count(*) 
 -- from 
 --   Observation
	--inner join Status
	--  on (Observation.StatusID = Status.ID)
 -- where 
 --   ((Observation.ImportBatchID = ImportBatchSummary.ImportBatchID) and 
	-- (Observation.SensorID = ImportBatchSummary.SensorID) and
	-- (Observation.PhenomenonOfferingID = ImportBatchSummary.PhenomenonOfferingID) and
	-- (Observation.PhenomenonUOMID = ImportBatchSummary.PhenomenonUOMID) and
	-- (Status.Name = 'Verified'))
 -- ) Verifed,
  Phenomenon.ID PhenomenonID, Phenomenon.Code PhenomenonCode, Phenomenon.Name PhenomenonName, Phenomenon.Description PhenomenonDescription, Phenomenon.Url PhenomenonUrl,
  OfferingID OfferingID, Offering.Code OfferingCode, Offering.Name OfferingName, Offering.Description OfferingDescription, 
  UnitOfMeasureID, UnitOfMeasure.Code UnitOfMeasureCode, UnitOfMeasure.Unit UnitOfMeasureUnit, UnitOfMeasure.UnitSymbol UnitOfMeasureSymbol,
  Sensor.Code SensorCode, Sensor.Name SensorName, Sensor.Description SensorDescription,  Sensor.Url SensorUrl,
  Instrument.Code InstrumentCode, Instrument.Name InstrumentName, Instrument.Description InstrumentDescription, Instrument.Url InstrumentUrl,
  Station.Code StationCode, Station.Name StationName, Station.Description StationDescription, Station.Url StationUrl,
  Site.Code SiteCode, Site.Name SiteName, Site.Description SiteDescription, Site.Url SiteUrl,
  Project.ID ProjectID, Project.Code ProjectCode, Project.Name ProjectName, Project.Description ProjectDescription, Project.Url ProjectUrl,
  Programme.ID ProgrammeID, Programme.Code ProgrammeCode, Programme.Name ProgrammeName, Programme.Description ProgrammeDescription, Programme.Url ProgrammeUrl,
  Organisation.ID OrganisationID, Organisation.Code OrganisationCode, Organisation.Name OrganisationName, Organisation.Description OrganisationDescription, Organisation.Url OrganisationUrl
From
  ImportBatchSummary
  inner join Sensor
    on (ImportBatchSummary.SensorID = Sensor.ID)
  inner join Instrument
    on (ImportBatchSummary.InstrumentID = Instrument.ID)
  inner join Station
    on (ImportBatchSummary.StationID = Station.ID)
  inner join Site
    on (ImportBatchSummary.SiteID = Site.ID)
  inner join PhenomenonOffering
    on (ImportBatchSummary.PhenomenonOfferingID = PhenomenonOffering.ID)
  inner join Phenomenon
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
  inner join PhenomenonUOM
    on (ImportBatchSummary.PhenomenonUOMID = PhenomenonUOM.ID)
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
PRINT N'Altering [dbo].[vLocations]...';


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
  (LatitudeNorth + LatitudeSouth) / 2 Latitude,
  (LongitudeWest + LongitudeEast) / 2 Longitude,
  (ElevationMaximum + ElevationMinimum) / 2 Elevation
from
  vImportBatchSummary
where
  (Count > 0) and 
  (LatitudeNorth is not null) and (LatitudeSouth is not null) and
  (LongitudeWest is not null) and (LongitudeEast is not null)
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[vInventoryDatasets]...';


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
PRINT N'Altering [dbo].[vInventorySensors]...';


GO
ALTER VIEW [dbo].[vInventorySensors]
AS
Select
  Row_Number() over (order by SiteName, StationName, InstrumentName, SensorName, PhenomenonName, OfferingName, UnitOfMeasureUnit) ID, s.*
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
  Sum(Count) Count, Min(StartDate) StartDate, Max(EndDate) EndDate,
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
PRINT N'Refreshing [dbo].[vSensorThingsAPIDatastreams]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPIDatastreams]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vSensorThingsAPILocations]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPILocations]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vSensorThingsAPIObservedProperties]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPIObservedProperties]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vSensorThingsAPISensors]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPISensors]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vSensorThingsAPIThings]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPIThings]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vSensorThingsAPIFeaturesOfInterest]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPIFeaturesOfInterest]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vSensorThingsAPIHistoricalLocations]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsAPIHistoricalLocations]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Creating [dbo].[vStationDatasets]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
CREATE VIEW [dbo].[vStationDatasets]
AS
Select
  Row_Number() over (order by StationCode, PhenomenonCode, OfferingCode, UnitOfMeasureCode) ID, s.*
from
(
Select
  OrganisationID, OrganisationCode, OrganisationName, OrganisationDescription, OrganisationUrl,
  ProgrammeID, ProgrammeCode, ProgrammeName, ProgrammeDescription, ProgrammeUrl,
  ProjectID, ProjectCode, ProjectName, ProjectDescription, ProjectUrl,
  SiteID, SiteCode, SiteName, SiteDescription,
  StationID, StationCode, StationName, StationDescription,
  PhenomenonID, PhenomenonCode, PhenomenonName, PhenomenonDescription,
  PhenomenonOfferingID, OfferingID, OfferingCode, OfferingName, OfferingDescription,
  PhenomenonUOMID, UnitOfMeasureID, UnitOfMeasureCode, UnitOfMeasureUnit, UnitOfMeasureSymbol,
  Sum(Count) Count,
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
  SiteID, SiteCode, SiteName, SiteDescription,
  StationID, StationCode, StationName, StationDescription,
  PhenomenonID, PhenomenonCode, PhenomenonName, PhenomenonDescription,
  PhenomenonOfferingID, OfferingID, OfferingCode, OfferingName, OfferingDescription,
  PhenomenonUOMID, UnitOfMeasureID, UnitOfMeasureCode, UnitOfMeasureUnit, UnitOfMeasureSymbol
) s
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[vFeatures]...';


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
  vImportBatchSummary
where
  (Count > 0)
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Checking existing data against newly created constraints';


GO
USE [$(DatabaseName)];


GO
ALTER TABLE [dbo].[DigitalObjectIdentifiers] WITH CHECK CHECK CONSTRAINT [FK_DigitalObjectIdentifiers_ParentID];

ALTER TABLE [dbo].[ImportBatchSummary] WITH CHECK CHECK CONSTRAINT [FK_ImportBatchSummary_DigitalObjectIdentifierID];

ALTER TABLE [dbo].[Organisation] WITH CHECK CHECK CONSTRAINT [FK_Organisation_DigitalObjectIdentifierID];

ALTER TABLE [dbo].[Programme] WITH CHECK CHECK CONSTRAINT [FK_Programme_DigitalObjectIdentifierID];

ALTER TABLE [dbo].[Project] WITH CHECK CHECK CONSTRAINT [FK_Project_DigitalObjectIdentifierID];

ALTER TABLE [dbo].[Site] WITH CHECK CHECK CONSTRAINT [FK_Site_DigitalObjectIdentifierID];

ALTER TABLE [dbo].[Station] WITH CHECK CHECK CONSTRAINT [FK_Station_DigitalObjectIdentifierID];


GO
PRINT N'Update complete.';


GO
