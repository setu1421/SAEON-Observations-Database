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
PRINT N'Dropping [dbo].[Observation].[IX_Observation_ValueDecade]...';


GO
DROP INDEX [IX_Observation_ValueDecade]
    ON [dbo].[Observation];


GO
PRINT N'Altering [dbo].[Observation]...';


GO
ALTER TABLE [dbo].[Observation] DROP COLUMN [ValueDecade];


GO
ALTER TABLE [dbo].[Observation]
    ADD [ValueDecade] AS (datepart(year, [ValueDate]) / 10 * 10);


GO
PRINT N'Creating [dbo].[Observation].[IX_Observation_ValueDecade]...';


GO
CREATE NONCLUSTERED INDEX [IX_Observation_ValueDecade]
    ON [dbo].[Observation]([ValueDecade] ASC)
    ON [Observations];


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
PRINT N'Refreshing [dbo].[vObservation]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vObservation]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Creating [dbo].[vObservationJSON]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
CREATE VIEW [dbo].[vObservationJSON]
AS
Select
  ID,
  ValueDate,
  ValueDay,
  ValueYear,
  ValueDecade,
  TextValue,
  RawValue,
  DataValue,
  Comment,
  CorrelationID,
  Latitude,
  Longitude,
  Elevation,
  ImportBatchID 'ImportBatch.ID',
  ImportBatchCode 'ImportBatch.Code',
  ImportBatchDate 'ImportBatch.Date',
  SiteID 'Site.ID',
  SiteCode 'Site.Code',
  SiteName 'Site.Name',
  StationID 'Station.ID',
  StationCode 'Station.Code',
  StationName 'Station.Name',
  InstrumentID 'Instrument.ID',
  InstrumentCode 'Instrument.Code',
  InstrumentName 'Instrument.Name',
  SensorID 'Sensor.ID',
  SensorCode 'Sensor.Code',
  SensorName 'Sensor.Name',
  PhenomenonID 'Phenomenon.ID',
  PhenomenonCode 'Phenomenon.Code',
  PhenomenonName 'Phenomenon.Name',
  OfferingID 'Offering.ID',
  OfferingCode 'Offering.Code',
  OfferingName 'Offering.Name',
  UnitOfMeasureID 'Unit.ID',
  UnitOfMeasureCode 'Unit.Code',
  UnitOfMeasureUnit 'Unit.Name',
  UnitOfMeasureSymbol 'Unit.Symbol',
  StatusID 'Status.ID',
  StatusCode 'Status.Code',
  StatusName 'Status.Name',
  StatusReasonID 'StatusReason.ID',
  StatusReasonCode 'StatusReason.Code',
  StatusReasonName 'StatusReason.Name'
from
  vObservationExpansion
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Update complete.';


GO
