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
PRINT N'Dropping [dbo].[DF_UserDownloads_UpdatedAt]...';


GO
ALTER TABLE [dbo].[UserDownloads] DROP CONSTRAINT [DF_UserDownloads_UpdatedAt];


GO
PRINT N'Dropping [dbo].[DF_UserDownloads_AddedAt]...';


GO
ALTER TABLE [dbo].[UserDownloads] DROP CONSTRAINT [DF_UserDownloads_AddedAt];


GO
PRINT N'Dropping [dbo].[DF_UserDownloads_ID]...';


GO
ALTER TABLE [dbo].[UserDownloads] DROP CONSTRAINT [DF_UserDownloads_ID];


GO
PRINT N'Altering [dbo].[ImportBatchSummary]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TABLE [dbo].[ImportBatchSummary] DROP COLUMN [BottomLatitude], COLUMN [LeftLongitude], COLUMN [RightLongitude], COLUMN [TopLatitude];


GO
ALTER TABLE [dbo].[ImportBatchSummary]
    ADD [LatitudeNorth]    FLOAT (53) NULL,
        [LatitudeSouth]    FLOAT (53) NULL,
        [LongitudeWest]    FLOAT (53) NULL,
        [LongitudeEast]    FLOAT (53) NULL,
        [ElevationMinimum] FLOAT (53) NULL,
        [ElevationMaximum] FLOAT (53) NULL;


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Starting rebuilding table [dbo].[UserDownloads]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_UserDownloads] (
    [ID]               UNIQUEIDENTIFIER CONSTRAINT [DF_UserDownloads_ID] DEFAULT (newid()) NOT NULL,
    [UserId]           NVARCHAR (128)   NOT NULL,
    [Name]             VARCHAR (150)    NOT NULL,
    [Description]      VARCHAR (5000)   NOT NULL,
    [Title]            VARCHAR (5000)   NOT NULL,
    [Input]            VARCHAR (5000)   NOT NULL,
    [ReQueryURL]       VARCHAR (5000)   NOT NULL,
    [DOIId]            INT              IDENTITY (1, 1) NOT NULL,
    [DOI]              AS               '10.15493/obsdb.' + Stuff(CONVERT (VARCHAR (20), CONVERT (VARBINARY (4), DOIId), 2), 5, 0, '.'),
    [DOIUrl]           AS               'https://doi.org/10.15493/obsdb.' + Stuff(CONVERT (VARCHAR (20), CONVERT (VARBINARY (4), DOIId), 2), 5, 0, '.'),
    [MetadataURL]      VARCHAR (2000)   NOT NULL,
    [DownloadURL]      VARCHAR (2000)   NOT NULL,
    [ZipFullName]      VARCHAR (2000)   NOT NULL,
    [ZipCheckSum]      VARCHAR (64)     NOT NULL,
    [Citation]         VARCHAR (5000)   NOT NULL,
    [Places]           VARCHAR (5000)   NULL,
    [LatitudeNorth]    FLOAT (53)       NULL,
    [LatitudeSouth]    FLOAT (53)       NULL,
    [LongitudeWest]    FLOAT (53)       NULL,
    [LongitudeEast]    FLOAT (53)       NULL,
    [ElevationMinimum] FLOAT (53)       NULL,
    [ElevationMaximum] FLOAT (53)       NULL,
    [StartDate]        DATETIME         NULL,
    [EndDate]          DATETIME         NULL,
    [AddedAt]          DATETIME         CONSTRAINT [DF_UserDownloads_AddedAt] DEFAULT (getdate()) NULL,
    [AddedBy]          NVARCHAR (128)   NOT NULL,
    [UpdatedAt]        DATETIME         CONSTRAINT [DF_UserDownloads_UpdatedAt] DEFAULT (getdate()) NULL,
    [UpdatedBy]        NVARCHAR (128)   NOT NULL,
    [RowVersion]       ROWVERSION       NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_UserDownloads1] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [tmp_ms_xx_constraint_UX_UserDownloads_UserId_Name1] UNIQUE NONCLUSTERED ([UserId] ASC, [Name] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[UserDownloads])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_UserDownloads] ([ID], [UserId], [Name], [Description], [AddedAt], [AddedBy], [UpdatedAt], [UpdatedBy], [MetadataURL], [DownloadURL], [Citation])
        SELECT   [ID],
                 [UserId],
                 [Name],
                 [Description],
                 [AddedAt],
                 [AddedBy],
                 [UpdatedAt],
                 [UpdatedBy],
                 [MetadataURL],
                 [DownloadURL],
                 [Citation]
        FROM     [dbo].[UserDownloads]
        ORDER BY [ID] ASC;
    END

DROP TABLE [dbo].[UserDownloads];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_UserDownloads]', N'UserDownloads';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_UserDownloads1]', N'PK_UserDownloads', N'OBJECT';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_UX_UserDownloads_UserId_Name1]', N'UX_UserDownloads_UserId_Name', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Creating [dbo].[TR_UserDownloads_Insert]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
CREATE TRIGGER [dbo].[TR_UserDownloads_Insert] ON [dbo].[UserDownloads]
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
    UserDownloads src
    inner join inserted ins
      on (ins.ID = src.ID)
END
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Creating [dbo].[TR_UserDownloads_Update]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
CREATE TRIGGER [dbo].[TR_UserDownloads_Update] ON [dbo].[UserDownloads]
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
    UserDownloads src
    inner join inserted ins
      on (ins.ID = src.ID)
    inner join deleted del
      on (del.ID = src.ID)
END
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vFeatures]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vFeatures]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vImportBatchSummary]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vImportBatchSummary]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vLocations]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vLocations]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[vInventory]...';


GO
ALTER VIEW [dbo].[vInventory]
AS
Select
  Row_Number() over (order by SiteName, StationName, InstrumentName, SensorName, PhenomenonName, OfferingName, UnitOfMeasureUnit) ID, s.*
from
(
Select
  SiteID, SiteCode, SiteName, 
  StationID, StationCode, StationName, 
  InstrumentID, InstrumentCode, InstrumentName, 
  SensorID, SensorCode, SensorName, 
  PhenomenonCode, PhenomenonName, 
  PhenomenonOfferingID, OfferingCode, OfferingName, 
  PhenomenonUOMID, UnitOfMeasureCode, UnitOfMeasureUnit, 
  Sum(Count) Count, Min(StartDate) StartDate, Max(EndDate) EndDate,
  Max(LatitudeNorth) LatitudeNorth, Min(LatitudeSouth) LatitudeSouth,
  Min(LongitudeWest) LongitudeWest, Max(LongitudeEast) LongitudeEast
from
  vImportBatchSummary
group by
  SiteID, SiteCode, SiteName, StationID, StationCode, StationName, InstrumentID, InstrumentCode, InstrumentName, 
  SensorID, SensorCode, SensorName, PhenomenonCode, PhenomenonName, 
  PhenomenonOfferingID, OfferingCode, OfferingName, 
  PhenomenonUOMID, UnitOfMeasureCode, UnitOfMeasureUnit
) s
GO
PRINT N'Update complete.';


GO
