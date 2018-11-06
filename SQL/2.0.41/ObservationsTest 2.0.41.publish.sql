﻿/*
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


--GO
--IF (SELECT is_default
--    FROM   [$(DatabaseName)].[sys].[filegroups]
--    WHERE  [name] = N'Documents') = 0
--    BEGIN
--        ALTER DATABASE [$(DatabaseName)]
--            MODIFY FILEGROUP [Documents] DEFAULT;
--    END


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
PRINT N'Dropping [dbo].[ImportBatchSummary]...';


GO
DROP TABLE [dbo].[ImportBatchSummary];


GO
PRINT N'Dropping [dbo].[DF_DataLog_ID]...';


GO
ALTER TABLE [dbo].[DataLog] DROP CONSTRAINT [DF_DataLog_ID];


GO
PRINT N'Dropping [dbo].[DF_DataLog_ImportDate]...';


GO
ALTER TABLE [dbo].[DataLog] DROP CONSTRAINT [DF_DataLog_ImportDate];


GO
PRINT N'Dropping [dbo].[DF_DataLog_AddedAt]...';


GO
ALTER TABLE [dbo].[DataLog] DROP CONSTRAINT [DF_DataLog_AddedAt];


GO
PRINT N'Dropping [dbo].[DF_DataLog_UpdatedAt]...';


GO
ALTER TABLE [dbo].[DataLog] DROP CONSTRAINT [DF_DataLog_UpdatedAt];


GO
PRINT N'Dropping [dbo].[FK_DataLog_DataSourceTransformation]...';


GO
ALTER TABLE [dbo].[DataLog] DROP CONSTRAINT [FK_DataLog_DataSourceTransformation];


GO
PRINT N'Dropping [dbo].[FK_DataLog_ImportBatch]...';


GO
ALTER TABLE [dbo].[DataLog] DROP CONSTRAINT [FK_DataLog_ImportBatch];


GO
PRINT N'Dropping [dbo].[FK_DataLog_PhenomenonOffering]...';


GO
ALTER TABLE [dbo].[DataLog] DROP CONSTRAINT [FK_DataLog_PhenomenonOffering];


GO
PRINT N'Dropping [dbo].[FK_DataLog_PhenomenonUOM]...';


GO
ALTER TABLE [dbo].[DataLog] DROP CONSTRAINT [FK_DataLog_PhenomenonUOM];


GO
PRINT N'Dropping [dbo].[FK_DataLog_Sensor]...';


GO
ALTER TABLE [dbo].[DataLog] DROP CONSTRAINT [FK_DataLog_Sensor];


GO
PRINT N'Dropping [dbo].[FK_DataLog_Status]...';


GO
ALTER TABLE [dbo].[DataLog] DROP CONSTRAINT [FK_DataLog_Status];


GO
PRINT N'Dropping [dbo].[FK_DataLog_StatusReason]...';


GO
ALTER TABLE [dbo].[DataLog] DROP CONSTRAINT [FK_DataLog_StatusReason];


GO
PRINT N'Dropping [dbo].[FK_DataLog_aspnet_Users]...';


GO
ALTER TABLE [dbo].[DataLog] DROP CONSTRAINT [FK_DataLog_aspnet_Users];


--GO
--PRINT N'Dropping [dbo].[FK_Observation_ImportBatch]...';


--GO
--ALTER TABLE [dbo].[Observation] DROP CONSTRAINT [FK_Observation_ImportBatch];


--GO
--PRINT N'Dropping [dbo].[FK_Observation_Status]...';


--GO
--ALTER TABLE [dbo].[Observation] DROP CONSTRAINT [FK_Observation_Status];


--GO
--PRINT N'Dropping [dbo].[FK_Observation_StatusReason]...';


--GO
--ALTER TABLE [dbo].[Observation] DROP CONSTRAINT [FK_Observation_StatusReason];


--GO
--PRINT N'Dropping [dbo].[FK_Observation_aspnet_Users]...';


--GO
--ALTER TABLE [dbo].[Observation] DROP CONSTRAINT [FK_Observation_aspnet_Users];


--GO
--PRINT N'Dropping [dbo].[FK_Observation_PhenomenonOffering]...';


--GO
--ALTER TABLE [dbo].[Observation] DROP CONSTRAINT [FK_Observation_PhenomenonOffering];


--GO
--PRINT N'Dropping [dbo].[FK_Observation_PhenomenonUOM]...';


--GO
--ALTER TABLE [dbo].[Observation] DROP CONSTRAINT [FK_Observation_PhenomenonUOM];


--GO
--PRINT N'Dropping [dbo].[FK_Observation_Sensor]...';


--GO
--ALTER TABLE [dbo].[Observation] DROP CONSTRAINT [FK_Observation_Sensor];


GO
PRINT N'Starting rebuilding table [dbo].[DataLog]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_DataLog] (
    [ID]                         UNIQUEIDENTIFIER CONSTRAINT [DF_DataLog_ID] DEFAULT newid() NOT NULL,
    [SensorID]                   UNIQUEIDENTIFIER NULL,
    [ImportDate]                 DATETIME         CONSTRAINT [DF_DataLog_ImportDate] DEFAULT (getdate()) NOT NULL,
    [ValueDate]                  DATETIME         NULL,
    [ValueTime]                  DATETIME         NULL,
    [ValueDay]                   AS               CAST (ValueDate AS DATE),
    [ValueText]                  VARCHAR (50)     NOT NULL,
    [TransformValueText]         VARCHAR (50)     NULL,
    [RawValue]                   FLOAT (53)       NULL,
    [DataValue]                  FLOAT (53)       NULL,
    [Comment]                    VARCHAR (250)    NULL,
    [Latitude]                   FLOAT (53)       NULL,
    [Longitude]                  FLOAT (53)       NULL,
    [Elevation]                  FLOAT (53)       NULL,
    [InvalidDateValue]           VARCHAR (50)     NULL,
    [InvalidTimeValue]           VARCHAR (50)     NULL,
    [InvalidOffering]            VARCHAR (50)     NULL,
    [InvalidUOM]                 VARCHAR (50)     NULL,
    [DataSourceTransformationID] UNIQUEIDENTIFIER NULL,
    [StatusID]                   UNIQUEIDENTIFIER NOT NULL,
    [StatusReasonID]             UNIQUEIDENTIFIER NULL,
    [ImportStatus]               VARCHAR (500)    NOT NULL,
    [UserId]                     UNIQUEIDENTIFIER NULL,
    [PhenomenonOfferingID]       UNIQUEIDENTIFIER NULL,
    [PhenomenonUOMID]            UNIQUEIDENTIFIER NULL,
    [ImportBatchID]              UNIQUEIDENTIFIER NOT NULL,
    [RawRecordData]              VARCHAR (500)    NULL,
    [RawFieldValue]              VARCHAR (50)     NOT NULL,
    [CorrelationID]              UNIQUEIDENTIFIER NULL,
    [AddedAt]                    DATETIME         CONSTRAINT [DF_DataLog_AddedAt] DEFAULT GetDate() NULL,
    [UpdatedAt]                  DATETIME         CONSTRAINT [DF_DataLog_UpdatedAt] DEFAULT GetDate() NULL,
    [RowVersion]                 ROWVERSION       NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_DataLog1] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [tmp_ms_xx_constraint_UX_DataLog1] UNIQUE NONCLUSTERED ([ImportBatchID] ASC, [SensorID] ASC, [ValueDate] ASC, [RawValue] ASC, [PhenomenonOfferingID] ASC, [PhenomenonUOMID] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[DataLog])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_DataLog] ([ID], [SensorID], [ImportDate], [ValueDate], [ValueTime], [ValueText], [TransformValueText], [RawValue], [DataValue], [Comment], [Latitude], [Longitude], [Elevation], [InvalidDateValue], [InvalidTimeValue], [InvalidOffering], [InvalidUOM], [DataSourceTransformationID], [StatusID], [StatusReasonID], [ImportStatus], [UserId], [PhenomenonOfferingID], [PhenomenonUOMID], [ImportBatchID], [RawRecordData], [RawFieldValue], [CorrelationID], [AddedAt], [UpdatedAt])
        SELECT   [ID],
                 [SensorID],
                 [ImportDate],
                 [ValueDate],
                 [ValueTime],
                 [ValueText],
                 [TransformValueText],
                 [RawValue],
                 [DataValue],
                 [Comment],
                 [Latitude],
                 [Longitude],
                 [Elevation],
                 [InvalidDateValue],
                 [InvalidTimeValue],
                 [InvalidOffering],
                 [InvalidUOM],
                 [DataSourceTransformationID],
                 [StatusID],
                 [StatusReasonID],
                 [ImportStatus],
                 [UserId],
                 [PhenomenonOfferingID],
                 [PhenomenonUOMID],
                 [ImportBatchID],
                 [RawRecordData],
                 [RawFieldValue],
                 [CorrelationID],
                 [AddedAt],
                 [UpdatedAt]
        FROM     [dbo].[DataLog]
        ORDER BY [ID] ASC;
    END

DROP TABLE [dbo].[DataLog];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_DataLog]', N'DataLog';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_DataLog1]', N'PK_DataLog', N'OBJECT';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_UX_DataLog1]', N'UX_DataLog', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating [dbo].[DataLog].[IX_DataLog_ImportBatchID]...';


GO
CREATE NONCLUSTERED INDEX [IX_DataLog_ImportBatchID]
    ON [dbo].[DataLog]([ImportBatchID] ASC);


GO
PRINT N'Creating [dbo].[DataLog].[IX_DataLog_SensorID]...';


GO
CREATE NONCLUSTERED INDEX [IX_DataLog_SensorID]
    ON [dbo].[DataLog]([SensorID] ASC);


GO
PRINT N'Creating [dbo].[DataLog].[IX_DataLog_ValueDay]...';


GO
CREATE NONCLUSTERED INDEX [IX_DataLog_ValueDay]
    ON [dbo].[DataLog]([ValueDay] ASC);


GO
PRINT N'Creating [dbo].[DataLog].[IX_DataLog_DataSourceTransformationID]...';


GO
CREATE NONCLUSTERED INDEX [IX_DataLog_DataSourceTransformationID]
    ON [dbo].[DataLog]([DataSourceTransformationID] ASC);


GO
PRINT N'Creating [dbo].[DataLog].[IX_DataLog_PhenomenonOfferingID]...';


GO
CREATE NONCLUSTERED INDEX [IX_DataLog_PhenomenonOfferingID]
    ON [dbo].[DataLog]([PhenomenonOfferingID] ASC);


GO
PRINT N'Creating [dbo].[DataLog].[IX_DataLog_PhenomenonUOMID]...';


GO
CREATE NONCLUSTERED INDEX [IX_DataLog_PhenomenonUOMID]
    ON [dbo].[DataLog]([PhenomenonUOMID] ASC);


GO
PRINT N'Creating [dbo].[DataLog].[IX_DataLog_StatusID]...';


GO
CREATE NONCLUSTERED INDEX [IX_DataLog_StatusID]
    ON [dbo].[DataLog]([StatusID] ASC);


GO
PRINT N'Creating [dbo].[DataLog].[IX_DataLog_UserId]...';


GO
CREATE NONCLUSTERED INDEX [IX_DataLog_UserId]
    ON [dbo].[DataLog]([UserId] ASC);


GO
PRINT N'Creating [dbo].[DataLog].[IX_DataLog_StatusReasonID]...';


GO
CREATE NONCLUSTERED INDEX [IX_DataLog_StatusReasonID]
    ON [dbo].[DataLog]([StatusReasonID] ASC);


GO
PRINT N'Creating [dbo].[ImportBatchSummary]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
CREATE TABLE [dbo].[ImportBatchSummary] (
    [ID]                   UNIQUEIDENTIFIER CONSTRAINT [DF_ImportBatchSummary_ID] DEFAULT newid() ROWGUIDCOL NOT NULL,
    [ImportBatchID]        UNIQUEIDENTIFIER NOT NULL,
    [SensorID]             UNIQUEIDENTIFIER NOT NULL,
    [InstrumentID]         UNIQUEIDENTIFIER NOT NULL,
    [StationID]            UNIQUEIDENTIFIER NOT NULL,
    [SiteID]               UNIQUEIDENTIFIER NOT NULL,
    [PhenomenonOfferingID] UNIQUEIDENTIFIER NOT NULL,
    [PhenomenonUOMID]      UNIQUEIDENTIFIER NOT NULL,
    [Count]                INT              NOT NULL,
    [Minimum]              FLOAT (53)       NULL,
    [Maximum]              FLOAT (53)       NULL,
    [Average]              FLOAT (53)       NULL,
    [StandardDeviation]    FLOAT (53)       NULL,
    [Variance]             FLOAT (53)       NULL,
    [TopLatitude]          FLOAT (53)       NULL,
    [BottomLatitude]       FLOAT (53)       NULL,
    [LeftLongitude]        FLOAT (53)       NULL,
    [RightLongitude]       FLOAT (53)       NULL,
    CONSTRAINT [PK_ImportBatchSummary] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UX_ImportBatchSummary] UNIQUE NONCLUSTERED ([ImportBatchID] ASC, [SensorID] ASC, [PhenomenonOfferingID] ASC, [PhenomenonUOMID] ASC)
);


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;

GO
PRINT N'Creating [dbo].[ImportBatchSummary].[IX_ImportBatchSummary_ImportBatchID]...';


GO
CREATE NONCLUSTERED INDEX [IX_ImportBatchSummary_ImportBatchID]
    ON [dbo].[ImportBatchSummary]([ImportBatchID] ASC);


GO
PRINT N'Creating [dbo].[ImportBatchSummary].[IX_ImportBatchSummary_SensorID]...';


GO
CREATE NONCLUSTERED INDEX [IX_ImportBatchSummary_SensorID]
    ON [dbo].[ImportBatchSummary]([SensorID] ASC);


GO
PRINT N'Creating [dbo].[ImportBatchSummary].[IX_ImportBatchSummary_InstrumentID]...';


GO
CREATE NONCLUSTERED INDEX [IX_ImportBatchSummary_InstrumentID]
    ON [dbo].[ImportBatchSummary]([InstrumentID] ASC);


GO
PRINT N'Creating [dbo].[ImportBatchSummary].[IX_ImportBatchSummary_StationID]...';


GO
CREATE NONCLUSTERED INDEX [IX_ImportBatchSummary_StationID]
    ON [dbo].[ImportBatchSummary]([StationID] ASC);


GO
PRINT N'Creating [dbo].[ImportBatchSummary].[IX_ImportBatchSummary_SiteID]...';


GO
CREATE NONCLUSTERED INDEX [IX_ImportBatchSummary_SiteID]
    ON [dbo].[ImportBatchSummary]([SiteID] ASC);


GO
PRINT N'Creating [dbo].[ImportBatchSummary].[IX_ImportBatchSummary_PhenomenonOfferingID]...';


GO
CREATE NONCLUSTERED INDEX [IX_ImportBatchSummary_PhenomenonOfferingID]
    ON [dbo].[ImportBatchSummary]([PhenomenonOfferingID] ASC);


GO
PRINT N'Creating [dbo].[ImportBatchSummary].[IX_ImportBatchSummary_PhenomenonUOMID]...';


GO
CREATE NONCLUSTERED INDEX [IX_ImportBatchSummary_PhenomenonUOMID]
    ON [dbo].[ImportBatchSummary]([PhenomenonUOMID] ASC);



GO
--PRINT N'Starting rebuilding table [dbo].[Observation]...';


--GO
--BEGIN TRANSACTION;

--SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

--SET XACT_ABORT ON;

--CREATE TABLE [dbo].[tmp_ms_xx_Observation] (
--    [ID]                   INT                  IDENTITY (1, 1) NOT NULL,
--    [SensorID]             UNIQUEIDENTIFIER     NOT NULL,
--    [ValueDate]            DATETIME             NOT NULL,
--    [ValueDay]             AS                   CAST (ValueDate AS DATE),
--    [ValueYear]            AS                   Year(ValueDate),
--    [ValueDecade]          AS                   Year(ValueDate) / 10,
--    [TextValue]            VARCHAR (10)         NULL,
--    [RawValue]             FLOAT (53)           NULL,
--    [DataValue]            FLOAT (53)           NULL,
--    [Comment]              VARCHAR (250) SPARSE NULL,
--    [PhenomenonOfferingID] UNIQUEIDENTIFIER     NOT NULL,
--    [PhenomenonUOMID]      UNIQUEIDENTIFIER     NOT NULL,
--    [ImportBatchID]        UNIQUEIDENTIFIER     NOT NULL,
--    [StatusID]             UNIQUEIDENTIFIER     NULL,
--    [StatusReasonID]       UNIQUEIDENTIFIER     NULL,
--    [CorrelationID]        UNIQUEIDENTIFIER     NULL,
--    [Latitude]             FLOAT (53)           NULL,
--    [Longitude]            FLOAT (53)           NULL,
--    [Elevation]            FLOAT (53)           NULL,
--    [UserId]               UNIQUEIDENTIFIER     NOT NULL,
--    [AddedDate]            DATETIME             CONSTRAINT [DF_Observation_AddedDate] DEFAULT GetDate() NOT NULL,
--    [AddedAt]              DATETIME             CONSTRAINT [DF_Observation_AddedAt] DEFAULT GetDate() NULL,
--    [UpdatedAt]            DATETIME             CONSTRAINT [DF_Observation_UpdatedAt] DEFAULT GetDate() NULL,
--    [RowVersion]           ROWVERSION           NOT NULL,
--    CONSTRAINT [tmp_ms_xx_constraint_PK_Observation1] PRIMARY KEY CLUSTERED ([ID] ASC) ON [Observations],
--    CONSTRAINT [tmp_ms_xx_constraint_UX_Observation1] UNIQUE NONCLUSTERED ([SensorID] ASC, [ValueDate] ASC, [RawValue] ASC, [PhenomenonOfferingID] ASC, [PhenomenonUOMID] ASC) ON [Observations]
--);

--IF EXISTS (SELECT TOP 1 1 
--           FROM   [dbo].[Observation])
--    BEGIN
--        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Observation] ON;
--        INSERT INTO [dbo].[tmp_ms_xx_Observation] ([ID], [SensorID], [ValueDate], [TextValue], [RawValue], [DataValue], [Comment], [PhenomenonOfferingID], [PhenomenonUOMID], [ImportBatchID], [StatusID], [StatusReasonID], [CorrelationID], [Latitude], [Longitude], [Elevation], [UserId], [AddedDate], [AddedAt], [UpdatedAt])
--        SELECT   [ID],
--                 [SensorID],
--                 [ValueDate],
--                 [TextValue],
--                 [RawValue],
--                 [DataValue],
--                 [Comment],
--                 [PhenomenonOfferingID],
--                 [PhenomenonUOMID],
--                 [ImportBatchID],
--                 [StatusID],
--                 [StatusReasonID],
--                 [CorrelationID],
--                 [Latitude],
--                 [Longitude],
--                 [Elevation],
--                 [UserId],
--                 [AddedDate],
--                 [AddedAt],
--                 [UpdatedAt]
--        FROM     [dbo].[Observation]
--        ORDER BY [ID] ASC;
--        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Observation] OFF;
--    END

--DROP TABLE [dbo].[Observation];

--EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Observation]', N'Observation';

--EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_Observation1]', N'PK_Observation', N'OBJECT';

--EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_UX_Observation1]', N'UX_Observation', N'OBJECT';

--COMMIT TRANSACTION;

--SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_ImportBatchID]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_ImportBatchID]
--    ON [dbo].[Observation]([ImportBatchID] ASC)
--    INCLUDE([ValueDate], [RawValue], [DataValue], [Comment], [CorrelationID])
--    ON [Observations];


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_SensorID]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_SensorID]
--    ON [dbo].[Observation]([SensorID] ASC)
--    ON [Observations];


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_SensorID_PhenomenonOfferingID]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_SensorID_PhenomenonOfferingID]
--    ON [dbo].[Observation]([SensorID] ASC, [PhenomenonOfferingID] ASC)
--    ON [Observations];


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_SensorID_PhenomenonUOMID]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_SensorID_PhenomenonUOMID]
--    ON [dbo].[Observation]([SensorID] ASC, [PhenomenonUOMID] ASC)
--    ON [Observations];


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_PhenomenonOfferingID]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_PhenomenonOfferingID]
--    ON [dbo].[Observation]([PhenomenonOfferingID] ASC)
--    ON [Observations];


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_PhenomenonUOMID]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_PhenomenonUOMID]
--    ON [dbo].[Observation]([PhenomenonUOMID] ASC)
--    ON [Observations];


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_UserId]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_UserId]
--    ON [dbo].[Observation]([UserId] ASC)
--    ON [Observations];


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_AddedDate]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_AddedDate]
--    ON [dbo].[Observation]([AddedDate] ASC)
--    ON [Observations];


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_ValueDate]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_ValueDate]
--    ON [dbo].[Observation]([ValueDate] ASC)
--    ON [Observations];


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_ValueDateDesc]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_ValueDateDesc]
--    ON [dbo].[Observation]([ValueDate] DESC)
--    ON [Observations];


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_ValueDay]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_ValueDay]
--    ON [dbo].[Observation]([ValueDay] ASC);


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_ValueYear]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_ValueYear]
--    ON [dbo].[Observation]([ValueYear] ASC);


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_ValueDecade]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_ValueDecade]
--    ON [dbo].[Observation]([ValueDecade] ASC);


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_StatusID]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_StatusID]
--    ON [dbo].[Observation]([StatusID] ASC)
--    ON [Observations];


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_StatusReasonID]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_StatusReasonID]
--    ON [dbo].[Observation]([StatusReasonID] ASC)
--    ON [Observations];


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_CorrelationID]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_CorrelationID]
--    ON [dbo].[Observation]([CorrelationID] ASC)
--    ON [Observations];


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_Latitude]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_Latitude]
--    ON [dbo].[Observation]([Latitude] ASC)
--    ON [Observations];


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_Longitude]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_Longitude]
--    ON [dbo].[Observation]([Longitude] ASC)
--    ON [Observations];


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_Elevation]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_Elevation]
--    ON [dbo].[Observation]([Elevation] ASC)
--    ON [Observations];


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_SensorID_ValueDate_Latitude]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_SensorID_ValueDate_Latitude]
--    ON [dbo].[Observation]([SensorID] ASC, [ValueDate] ASC, [Latitude] ASC)
--    ON [Observations];


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_SensorID_ValueDate_Longitude]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_SensorID_ValueDate_Longitude]
--    ON [dbo].[Observation]([SensorID] ASC, [ValueDate] ASC, [Longitude] ASC)
--    ON [Observations];


--GO
--PRINT N'Creating [dbo].[Observation].[IX_Observation_SensorID_ValueDate_Elevation]...';


--GO
--CREATE NONCLUSTERED INDEX [IX_Observation_SensorID_ValueDate_Elevation]
--    ON [dbo].[Observation]([SensorID] ASC, [ValueDate] ASC, [Elevation] ASC)
--    ON [Observations];


GO
PRINT N'Creating [dbo].[FK_DataLog_DataSourceTransformation]...';


GO
ALTER TABLE [dbo].[DataLog] WITH NOCHECK
    ADD CONSTRAINT [FK_DataLog_DataSourceTransformation] FOREIGN KEY ([DataSourceTransformationID]) REFERENCES [dbo].[DataSourceTransformation] ([ID]);


GO
PRINT N'Creating [dbo].[FK_DataLog_ImportBatch]...';


GO
ALTER TABLE [dbo].[DataLog] WITH NOCHECK
    ADD CONSTRAINT [FK_DataLog_ImportBatch] FOREIGN KEY ([ImportBatchID]) REFERENCES [dbo].[ImportBatch] ([ID]);


GO
PRINT N'Creating [dbo].[FK_DataLog_PhenomenonOffering]...';


GO
ALTER TABLE [dbo].[DataLog] WITH NOCHECK
    ADD CONSTRAINT [FK_DataLog_PhenomenonOffering] FOREIGN KEY ([PhenomenonOfferingID]) REFERENCES [dbo].[PhenomenonOffering] ([ID]);


GO
PRINT N'Creating [dbo].[FK_DataLog_PhenomenonUOM]...';


GO
ALTER TABLE [dbo].[DataLog] WITH NOCHECK
    ADD CONSTRAINT [FK_DataLog_PhenomenonUOM] FOREIGN KEY ([PhenomenonUOMID]) REFERENCES [dbo].[PhenomenonUOM] ([ID]);


GO
PRINT N'Creating [dbo].[FK_DataLog_Sensor]...';


GO
ALTER TABLE [dbo].[DataLog] WITH NOCHECK
    ADD CONSTRAINT [FK_DataLog_Sensor] FOREIGN KEY ([SensorID]) REFERENCES [dbo].[Sensor] ([ID]);


GO
PRINT N'Creating [dbo].[FK_DataLog_Status]...';


GO
ALTER TABLE [dbo].[DataLog] WITH NOCHECK
    ADD CONSTRAINT [FK_DataLog_Status] FOREIGN KEY ([StatusID]) REFERENCES [dbo].[Status] ([ID]);


GO
PRINT N'Creating [dbo].[FK_DataLog_StatusReason]...';


GO
ALTER TABLE [dbo].[DataLog] WITH NOCHECK
    ADD CONSTRAINT [FK_DataLog_StatusReason] FOREIGN KEY ([StatusReasonID]) REFERENCES [dbo].[StatusReason] ([ID]);


GO
PRINT N'Creating [dbo].[FK_DataLog_aspnet_Users]...';


GO
ALTER TABLE [dbo].[DataLog] WITH NOCHECK
    ADD CONSTRAINT [FK_DataLog_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]);

GO
PRINT N'Creating [dbo].[FK_ImportBatchSummary_ImportBatchID]...';


GO
ALTER TABLE [dbo].[ImportBatchSummary] WITH NOCHECK
    ADD CONSTRAINT [FK_ImportBatchSummary_ImportBatchID] FOREIGN KEY ([ImportBatchID]) REFERENCES [dbo].[ImportBatch] ([ID]);


GO
PRINT N'Creating [dbo].[FK_ImportBatchSummary_SensorID]...';


GO
ALTER TABLE [dbo].[ImportBatchSummary] WITH NOCHECK
    ADD CONSTRAINT [FK_ImportBatchSummary_SensorID] FOREIGN KEY ([SensorID]) REFERENCES [dbo].[Sensor] ([ID]);


GO
PRINT N'Creating [dbo].[FK_ImportBatchSummary_PhenomenonOfferingID]...';


GO
ALTER TABLE [dbo].[ImportBatchSummary] WITH NOCHECK
    ADD CONSTRAINT [FK_ImportBatchSummary_PhenomenonOfferingID] FOREIGN KEY ([PhenomenonOfferingID]) REFERENCES [dbo].[PhenomenonOffering] ([ID]);


GO
PRINT N'Creating [dbo].[FK_ImportBatchSummary_PhenomenonUOMID]...';


GO
ALTER TABLE [dbo].[ImportBatchSummary] WITH NOCHECK
    ADD CONSTRAINT [FK_ImportBatchSummary_PhenomenonUOMID] FOREIGN KEY ([PhenomenonUOMID]) REFERENCES [dbo].[PhenomenonUOM] ([ID]);


GO
PRINT N'Creating [dbo].[FK_ImportBatchSummary_InstrumentID]...';


GO
ALTER TABLE [dbo].[ImportBatchSummary] WITH NOCHECK
    ADD CONSTRAINT [FK_ImportBatchSummary_InstrumentID] FOREIGN KEY ([InstrumentID]) REFERENCES [dbo].[Instrument] ([ID]);


GO
PRINT N'Creating [dbo].[FK_ImportBatchSummary_StationID]...';


GO
ALTER TABLE [dbo].[ImportBatchSummary] WITH NOCHECK
    ADD CONSTRAINT [FK_ImportBatchSummary_StationID] FOREIGN KEY ([StationID]) REFERENCES [dbo].[Station] ([ID]);


GO
PRINT N'Creating [dbo].[FK_ImportBatchSummary_SiteID]...';


GO
ALTER TABLE [dbo].[ImportBatchSummary] WITH NOCHECK
    ADD CONSTRAINT [FK_ImportBatchSummary_SiteID] FOREIGN KEY ([SiteID]) REFERENCES [dbo].[Site] ([ID]);




--GO
--PRINT N'Creating [dbo].[FK_Observation_ImportBatch]...';


--GO
--ALTER TABLE [dbo].[Observation] WITH NOCHECK
--    ADD CONSTRAINT [FK_Observation_ImportBatch] FOREIGN KEY ([ImportBatchID]) REFERENCES [dbo].[ImportBatch] ([ID]);


--GO
--PRINT N'Creating [dbo].[FK_Observation_Status]...';


--GO
--ALTER TABLE [dbo].[Observation] WITH NOCHECK
--    ADD CONSTRAINT [FK_Observation_Status] FOREIGN KEY ([StatusID]) REFERENCES [dbo].[Status] ([ID]);


--GO
--PRINT N'Creating [dbo].[FK_Observation_StatusReason]...';


--GO
--ALTER TABLE [dbo].[Observation] WITH NOCHECK
--    ADD CONSTRAINT [FK_Observation_StatusReason] FOREIGN KEY ([StatusReasonID]) REFERENCES [dbo].[StatusReason] ([ID]);


--GO
--PRINT N'Creating [dbo].[FK_Observation_aspnet_Users]...';


--GO
--ALTER TABLE [dbo].[Observation] WITH NOCHECK
--    ADD CONSTRAINT [FK_Observation_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]);


--GO
--PRINT N'Creating [dbo].[FK_Observation_PhenomenonOffering]...';


--GO
--ALTER TABLE [dbo].[Observation] WITH NOCHECK
--    ADD CONSTRAINT [FK_Observation_PhenomenonOffering] FOREIGN KEY ([PhenomenonOfferingID]) REFERENCES [dbo].[PhenomenonOffering] ([ID]);


--GO
--PRINT N'Creating [dbo].[FK_Observation_PhenomenonUOM]...';


--GO
--ALTER TABLE [dbo].[Observation] WITH NOCHECK
--    ADD CONSTRAINT [FK_Observation_PhenomenonUOM] FOREIGN KEY ([PhenomenonUOMID]) REFERENCES [dbo].[PhenomenonUOM] ([ID]);


--GO
--PRINT N'Creating [dbo].[FK_Observation_Sensor]...';


--GO
--ALTER TABLE [dbo].[Observation] WITH NOCHECK
--    ADD CONSTRAINT [FK_Observation_Sensor] FOREIGN KEY ([SensorID]) REFERENCES [dbo].[Sensor] ([ID]);


GO
PRINT N'Creating [dbo].[TR_DataLog_Insert]...';


GO
CREATE TRIGGER [dbo].[TR_DataLog_Insert] ON [dbo].[DataLog]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        DataLog src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
PRINT N'Creating [dbo].[TR_DataLog_Update]...';


GO
CREATE TRIGGER [dbo].[TR_DataLog_Update] ON [dbo].[DataLog]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
--> Changed 2.0.19 20161205 TimPN
--		AddedAt = del.AddedAt,
        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate ()),
--< Changed 2.0.19 20161205 TimPN
        UpdatedAt = GETDATE()
    from
        DataLog src
        inner join inserted ins 
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160708 TimPN
GO
PRINT N'Creating [dbo].[TR_Observation_Insert]...';


--GO
--CREATE TRIGGER [dbo].[TR_Observation_Insert] ON [dbo].[Observation]
--FOR INSERT
--AS
--BEGIN
--    SET NoCount ON
--    Update
--        src
--    set
--        AddedAt = GETDATE(),
--        UpdatedAt = NULL
--    from
--        Observation src
--        inner join inserted ins
--            on (ins.ID = src.ID)
--END
--GO
--PRINT N'Creating [dbo].[TR_Observation_Update]...';


--GO
--CREATE TRIGGER [dbo].[TR_Observation_Update] ON [dbo].[Observation]
--FOR UPDATE
--AS
--BEGIN
--    SET NoCount ON
--    Update
--        src
--    set
----> Changed 2.0.19 20161205 TimPN
----		AddedAt = del.AddedAt,
--        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate ()),
----< Changed 2.0.19 20161205 TimPN
--        UpdatedAt = GETDATE()
--    from
--        Observation src
--        inner join inserted ins
--            on (ins.ID = src.ID)
--        inner join deleted del
--            on (del.ID = src.ID)
--END
----< Changed 2.0.15 20161102 TimPN
----< Added 2.0.8 20160718 TimPN
GO
PRINT N'Refreshing [dbo].[vDataLog]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vDataLog]';


GO
PRINT N'Altering [dbo].[vImportBatchSummary]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
--> Added 2.0.28 20180423 TimPN
ALTER VIEW [dbo].[vImportBatchSummary]
AS 
Select
  ImportBatchSummary.*, 
  Phenomenon.Code PhenomenonCode, Phenomenon.Name PhenomenonName,
  Offering.Code OfferingCode, Offering.Name OfferingName,
  UnitOfMeasure.Code UnitOfMeasureCode, UnitOfMeasure.Unit UnitOfMeasureUnit, UnitOfMeasure.UnitSymbol UnitOfMeasureSymbol,
  Sensor.Code SensorCode, Sensor.Name SensorName,
  Instrument.Code InstrumentCode, Instrument.Name InstrumentName,
  Station.Code StationCode, Station.Name StationName,
  Site.Code SiteCode, Site.Name SiteName
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
--< Added 2.0.28 20180423 TimPN
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vInventory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vInventory]';


GO
PRINT N'Refreshing [dbo].[vInventoryPhenomenaOfferings]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vInventoryPhenomenaOfferings]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vInventoryStations]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vInventoryStations]';


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
PRINT N'Refreshing [dbo].[vSensorThingsDatastreams]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSensorThingsDatastreams]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vApiDataDownload]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vApiDataDownload]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vApiDataQuery]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vApiDataQuery]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vApiInventory]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vApiInventory]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vApiSpacialCoverage]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vApiSpacialCoverage]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vApiTemporalCoverage]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vApiTemporalCoverage]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vInventoryInstruments]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vInventoryInstruments]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vInventoryOrganisations]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vInventoryOrganisations]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vInventoryTotals]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vInventoryTotals]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vInventoryYears]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vInventoryYears]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vObservation]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vObservation]';


GO
PRINT N'Refreshing [dbo].[vObservationsList]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vObservationsList]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Checking existing data against newly created constraints';


GO
USE [$(DatabaseName)];


GO
ALTER TABLE [dbo].[DataLog] WITH CHECK CHECK CONSTRAINT [FK_DataLog_DataSourceTransformation];

ALTER TABLE [dbo].[DataLog] WITH CHECK CHECK CONSTRAINT [FK_DataLog_ImportBatch];

ALTER TABLE [dbo].[DataLog] WITH CHECK CHECK CONSTRAINT [FK_DataLog_PhenomenonOffering];

ALTER TABLE [dbo].[DataLog] WITH CHECK CHECK CONSTRAINT [FK_DataLog_PhenomenonUOM];

ALTER TABLE [dbo].[DataLog] WITH CHECK CHECK CONSTRAINT [FK_DataLog_Sensor];

ALTER TABLE [dbo].[DataLog] WITH CHECK CHECK CONSTRAINT [FK_DataLog_Status];

ALTER TABLE [dbo].[DataLog] WITH CHECK CHECK CONSTRAINT [FK_DataLog_StatusReason];

ALTER TABLE [dbo].[DataLog] WITH CHECK CHECK CONSTRAINT [FK_DataLog_aspnet_Users];

ALTER TABLE [dbo].[ImportBatchSummary] WITH CHECK CHECK CONSTRAINT [FK_ImportBatchSummary_SensorID];

ALTER TABLE [dbo].[ImportBatchSummary] WITH CHECK CHECK CONSTRAINT [FK_ImportBatchSummary_ImportBatchID];

ALTER TABLE [dbo].[ImportBatchSummary] WITH CHECK CHECK CONSTRAINT [FK_ImportBatchSummary_InstrumentID];

ALTER TABLE [dbo].[ImportBatchSummary] WITH CHECK CHECK CONSTRAINT [FK_ImportBatchSummary_StationID];

ALTER TABLE [dbo].[ImportBatchSummary] WITH CHECK CHECK CONSTRAINT [FK_ImportBatchSummary_SiteID];

ALTER TABLE [dbo].[ImportBatchSummary] WITH CHECK CHECK CONSTRAINT [FK_ImportBatchSummary_PhenomenonOfferingID];

ALTER TABLE [dbo].[ImportBatchSummary] WITH CHECK CHECK CONSTRAINT [FK_ImportBatchSummary_PhenomenonUOMID];

ALTER TABLE [dbo].[Observation] WITH CHECK CHECK CONSTRAINT [FK_Observation_ImportBatch];

ALTER TABLE [dbo].[Observation] WITH CHECK CHECK CONSTRAINT [FK_Observation_Status];

ALTER TABLE [dbo].[Observation] WITH CHECK CHECK CONSTRAINT [FK_Observation_StatusReason];

ALTER TABLE [dbo].[Observation] WITH CHECK CHECK CONSTRAINT [FK_Observation_aspnet_Users];

ALTER TABLE [dbo].[Observation] WITH CHECK CHECK CONSTRAINT [FK_Observation_PhenomenonOffering];

ALTER TABLE [dbo].[Observation] WITH CHECK CHECK CONSTRAINT [FK_Observation_PhenomenonUOM];

ALTER TABLE [dbo].[Observation] WITH CHECK CHECK CONSTRAINT [FK_Observation_Sensor];


GO
PRINT N'Update complete.';


GO
