﻿
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
/*
The type for column AddedBy in table [dbo].[DigitalObjectIdentifiers] is currently  VARCHAR (128) NOT NULL but is being changed to  VARCHAR (36) NOT NULL. Data loss could occur and deployment may fail if the column contains data that is incompatible with type  VARCHAR (36) NOT NULL.

The type for column UpdatedBy in table [dbo].[DigitalObjectIdentifiers] is currently  VARCHAR (128) NOT NULL but is being changed to  VARCHAR (36) NOT NULL. Data loss could occur and deployment may fail if the column contains data that is incompatible with type  VARCHAR (36) NOT NULL.
*/

--IF EXISTS (select top 1 1 from [dbo].[DigitalObjectIdentifiers])
--    RAISERROR (N'Rows were detected. The schema update is terminating because data loss might occur.', 16, 127) WITH NOWAIT

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
PRINT N'Dropping Unique Constraint [dbo].[UX_ImportBatchSummary]...';


GO
ALTER TABLE [dbo].[ImportBatchSummary] DROP CONSTRAINT [UX_ImportBatchSummary];


GO
PRINT N'Altering Table [dbo].[DigitalObjectIdentifiers]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
ALTER TABLE [dbo].[DigitalObjectIdentifiers] ALTER COLUMN [AddedBy] VARCHAR (36) NOT NULL;

ALTER TABLE [dbo].[DigitalObjectIdentifiers] ALTER COLUMN [UpdatedBy] VARCHAR (36) NOT NULL;


GO
ALTER TABLE [dbo].[DigitalObjectIdentifiers]
    ADD [DatasetID] UNIQUEIDENTIFIER NULL;


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Creating Index [dbo].[DigitalObjectIdentifiers].[IX_DigitalObjectIdentifiers_DatasetID]...';


GO
CREATE NONCLUSTERED INDEX [IX_DigitalObjectIdentifiers_DatasetID]
    ON [dbo].[DigitalObjectIdentifiers]([DatasetID] ASC);


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
PRINT N'Altering Table [dbo].[UserDownloads]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
ALTER TABLE [dbo].[UserDownloads]
    ADD [IPAddress] VARCHAR (45) NULL,
        [FileSize]  BIGINT       NULL,
        [ZipSize]   BIGINT       NULL;


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Creating Table [dbo].[Datasets]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
CREATE TABLE [dbo].[Datasets] (
    [ID]                        UNIQUEIDENTIFIER NOT NULL,
    [Code]                      VARCHAR (200)    NOT NULL,
    [Name]                      VARCHAR (500)    NOT NULL,
    [Description]               VARCHAR (5000)   NULL,
    [Title]                     VARCHAR (5000)   NULL,
    [StationID]                 UNIQUEIDENTIFIER NOT NULL,
    [PhenomenonOfferingID]      UNIQUEIDENTIFIER NOT NULL,
    [PhenomenonUOMID]           UNIQUEIDENTIFIER NOT NULL,
    [DigitalObjectIdentifierID] INT              NULL,
    [Count]                     INT              NULL,
    [ValueCount]                INT              NULL,
    [NullCount]                 INT              NULL,
    [VerifiedCount]             INT              NULL,
    [UnverifiedCount]           INT              NULL,
    [StartDate]                 DATETIME         NULL,
    [EndDate]                   DATETIME         NULL,
    [LatitudeNorth]             FLOAT (53)       NULL,
    [LatitudeSouth]             FLOAT (53)       NULL,
    [LongitudeWest]             FLOAT (53)       NULL,
    [LongitudeEast]             FLOAT (53)       NULL,
    [ElevationMinimum]          FLOAT (53)       NULL,
    [ElevationMaximum]          FLOAT (53)       NULL,
    [HashCode]                  INT              NOT NULL,
    [NeedsUpdate]               BIT              NULL,
    [AddedAt]                   DATETIME         NULL,
    [AddedBy]                   VARCHAR (36)     NOT NULL,
    [UpdatedAt]                 DATETIME         NULL,
    [UpdatedBy]                 VARCHAR (36)     NOT NULL,
    [UserId]                    UNIQUEIDENTIFIER NOT NULL,
    [RowVersion]                ROWVERSION       NOT NULL,
    CONSTRAINT [PK_Datasets] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UX_Datasets_Code] UNIQUE NONCLUSTERED ([Code] ASC),
    CONSTRAINT [UX_Datasets_Name] UNIQUE NONCLUSTERED ([Name] ASC),
    CONSTRAINT [UX_Datasets_StationID_PhenomenonOfferingID_PhenomenonUOMID] UNIQUE NONCLUSTERED ([StationID] ASC, [PhenomenonOfferingID] ASC, [PhenomenonUOMID] ASC)
);


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Creating Index [dbo].[Datasets].[IX_Datasets_DigitalObjectIdentifierID]...';


GO
CREATE NONCLUSTERED INDEX [IX_Datasets_DigitalObjectIdentifierID]
    ON [dbo].[Datasets]([DigitalObjectIdentifierID] ASC);


GO
PRINT N'Creating Index [dbo].[Datasets].[IX_Datasets_StationID]...';


GO
CREATE NONCLUSTERED INDEX [IX_Datasets_StationID]
    ON [dbo].[Datasets]([StationID] ASC);


GO
PRINT N'Creating Index [dbo].[Datasets].[IX_Datasets_PhenomenonOfferingID]...';


GO
CREATE NONCLUSTERED INDEX [IX_Datasets_PhenomenonOfferingID]
    ON [dbo].[Datasets]([PhenomenonOfferingID] ASC);


GO
PRINT N'Creating Index [dbo].[Datasets].[IX_Datasets_PhenomenonUOMID]...';


GO
CREATE NONCLUSTERED INDEX [IX_Datasets_PhenomenonUOMID]
    ON [dbo].[Datasets]([PhenomenonUOMID] ASC);


GO
PRINT N'Creating Unique Constraint [dbo].[UX_ImportBatchSummary]...';


GO
ALTER TABLE [dbo].[ImportBatchSummary]
    ADD CONSTRAINT [UX_ImportBatchSummary] UNIQUE NONCLUSTERED ([ImportBatchID] ASC, [SensorID] ASC, [PhenomenonOfferingID] ASC, [PhenomenonUOMID] ASC);


GO
PRINT N'Creating Default Constraint [dbo].[DF_Datasets_ID]...';


GO
ALTER TABLE [dbo].[Datasets]
    ADD CONSTRAINT [DF_Datasets_ID] DEFAULT NewId() FOR [ID];


GO
PRINT N'Creating Default Constraint [dbo].[DF_Dataset_AddedAt]...';


GO
ALTER TABLE [dbo].[Datasets]
    ADD CONSTRAINT [DF_Dataset_AddedAt] DEFAULT (getdate()) FOR [AddedAt];


GO
PRINT N'Creating Default Constraint [dbo].[DF_Datasets_UpdatedAt]...';


GO
ALTER TABLE [dbo].[Datasets]
    ADD CONSTRAINT [DF_Datasets_UpdatedAt] DEFAULT (getdate()) FOR [UpdatedAt];


GO
PRINT N'Creating Foreign Key [dbo].[FK_Datasets_StationID]...';


GO
ALTER TABLE [dbo].[Datasets] WITH NOCHECK
    ADD CONSTRAINT [FK_Datasets_StationID] FOREIGN KEY ([StationID]) REFERENCES [dbo].[Station] ([ID]);


GO
PRINT N'Creating Foreign Key [dbo].[FK_Datasets_PhenomenonOfferingID]...';


GO
ALTER TABLE [dbo].[Datasets] WITH NOCHECK
    ADD CONSTRAINT [FK_Datasets_PhenomenonOfferingID] FOREIGN KEY ([PhenomenonOfferingID]) REFERENCES [dbo].[PhenomenonOffering] ([ID]);


GO
PRINT N'Creating Foreign Key [dbo].[FK_Datasets_PhenomenonUOMID]...';


GO
ALTER TABLE [dbo].[Datasets] WITH NOCHECK
    ADD CONSTRAINT [FK_Datasets_PhenomenonUOMID] FOREIGN KEY ([PhenomenonUOMID]) REFERENCES [dbo].[PhenomenonUOM] ([ID]);


GO
PRINT N'Creating Foreign Key [dbo].[FK_Datasets_DigitalObjectIdentifierID]...';


GO
ALTER TABLE [dbo].[Datasets] WITH NOCHECK
    ADD CONSTRAINT [FK_Datasets_DigitalObjectIdentifierID] FOREIGN KEY ([DigitalObjectIdentifierID]) REFERENCES [dbo].[DigitalObjectIdentifiers] ([ID]);


GO
PRINT N'Creating Foreign Key [dbo].[FK_DigitalObjectIdentifiers_DatasetID]...';


GO
ALTER TABLE [dbo].[DigitalObjectIdentifiers] WITH NOCHECK
    ADD CONSTRAINT [FK_DigitalObjectIdentifiers_DatasetID] FOREIGN KEY ([DatasetID]) REFERENCES [dbo].[Datasets] ([ID]);


GO
PRINT N'Creating Trigger [dbo].[TR_Datasets_Insert]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
CREATE TRIGGER [dbo].[TR_Datasets_Insert] ON [dbo].[Datasets]
FOR INSERT
AS
BEGIN
  SET NoCount ON
  Update
      src
  set
      AddedAt = GetDate(),
      UpdatedAt = NULL
  from
    Datasets src
    inner join inserted ins
      on (ins.ID = src.ID)
END
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Creating Trigger [dbo].[TR_Datasets_Update]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
CREATE TRIGGER [dbo].[TR_Datasets_Update] ON [dbo].[Datasets]
FOR UPDATE
AS
BEGIN
  SET NoCount ON
  Update
      src
  set
    AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate()),
    UpdatedAt = GetDate()
  from
    Datasets src
    inner join inserted ins
      on (ins.ID = src.ID)
    inner join deleted del
      on (del.ID = src.ID)
END
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


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
  vInventoryDatasets
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Creating View [dbo].[vDatasetsExpansion]...';


GO
SET ANSI_NULLS ON;

SET QUOTED_IDENTIFIER OFF;


GO
CREATE VIEW [dbo].[vDatasetsExpansion]
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
  UnitOfMeasure.ID UnitOfMeasureID, UnitOfMeasure.Code UnitOfMeasureCode, UnitOfMeasure.Unit UnitOfMeasureUnit, UnitOfMeasure.UnitSymbol UnitOfMeasureSymbol
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
where
  (VerifiedCount > 0) and (LatitudeNorth is not null) and (LatitudeSouth is not null) and (LongitudeWest is not null) and (LongitudeEast is not null)
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
	(VerifiedCount > 0)  and 
	(LatitudeNorth is not null) and (LatitudeSouth is not null) and 
	(LongitudeEast is not null) and (LongitudeWest is not null)
),
VerifiedImportBatchSummaries as
(
Select
	*
from
	vImportBatchSummary
where
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
PRINT N'Checking existing data against newly created constraints';


GO
USE [$(DatabaseName)];


GO
ALTER TABLE [dbo].[Datasets] WITH CHECK CHECK CONSTRAINT [FK_Datasets_StationID];

ALTER TABLE [dbo].[Datasets] WITH CHECK CHECK CONSTRAINT [FK_Datasets_PhenomenonOfferingID];

ALTER TABLE [dbo].[Datasets] WITH CHECK CHECK CONSTRAINT [FK_Datasets_PhenomenonUOMID];

ALTER TABLE [dbo].[Datasets] WITH CHECK CHECK CONSTRAINT [FK_Datasets_DigitalObjectIdentifierID];

ALTER TABLE [dbo].[DigitalObjectIdentifiers] WITH CHECK CHECK CONSTRAINT [FK_DigitalObjectIdentifiers_DatasetID];


GO
PRINT N'Update complete.';


GO
