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
IF (SELECT is_default
    FROM   [$(DatabaseName)].[sys].[filegroups]
    WHERE  [name] = N'Documents') = 0
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            MODIFY FILEGROUP [Documents] DEFAULT;
    END


GO
USE [$(DatabaseName)];


GO
PRINT N'Dropping [dbo].[Station].[IX_Station_SiteID]...';


GO
DROP INDEX [IX_Station_SiteID]
    ON [dbo].[Station];


GO
PRINT N'Dropping [dbo].[FK_Station_Site]...';


GO
ALTER TABLE [dbo].[Station] DROP CONSTRAINT [FK_Station_Site];


GO
PRINT N'Dropping [dbo].[UX_Station_SiteID_Name]...';


GO
ALTER TABLE [dbo].[Station] DROP CONSTRAINT [UX_Station_SiteID_Name];


GO
PRINT N'Dropping [dbo].[UX_Station_SiteID_Code]...';


GO
ALTER TABLE [dbo].[Station] DROP CONSTRAINT [UX_Station_SiteID_Code];


GO
PRINT N'Altering [dbo].[Station]...';


GO
ALTER TABLE [dbo].[Station] ALTER COLUMN [SiteID] UNIQUEIDENTIFIER NOT NULL;


GO
PRINT N'Creating [dbo].[UX_Station_SiteID_Name]...';


GO
ALTER TABLE [dbo].[Station]
    ADD CONSTRAINT [UX_Station_SiteID_Name] UNIQUE NONCLUSTERED ([SiteID] ASC, [Name] ASC);


GO
PRINT N'Creating [dbo].[UX_Station_SiteID_Code]...';


GO
ALTER TABLE [dbo].[Station]
    ADD CONSTRAINT [UX_Station_SiteID_Code] UNIQUE NONCLUSTERED ([SiteID] ASC, [Code] ASC);


GO
PRINT N'Creating [dbo].[Station].[IX_Station_SiteID]...';


GO
CREATE NONCLUSTERED INDEX [IX_Station_SiteID]
    ON [dbo].[Station]([SiteID] ASC);


GO
PRINT N'Creating [dbo].[FK_Station_Site]...';


GO
ALTER TABLE [dbo].[Station] WITH NOCHECK
    ADD CONSTRAINT [FK_Station_Site] FOREIGN KEY ([SiteID]) REFERENCES [dbo].[Site] ([ID]);


GO
PRINT N'Refreshing [dbo].[vDataLog]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vDataLog]';


GO
PRINT N'Refreshing [dbo].[vDataQuery]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vDataQuery]';


GO
PRINT N'Refreshing [dbo].[vInstrumentOrganisation]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vInstrumentOrganisation]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Refreshing [dbo].[vInventory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vInventory]';


GO
PRINT N'Refreshing [dbo].[vObservation]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vObservation]';


GO
PRINT N'Refreshing [dbo].[vOrganisationStation]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vOrganisationStation]';


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
PRINT N'Altering [dbo].[vSensor]...';


GO
--> Changed 2.0.3 20160503 TimPN
--Renamed SensorProcedure to Sensor
--< Changed 2.0.3 20160503 TimPN
--> Removed 2.0.17 20161128 TimPN
--CREATE VIEW [dbo].[vSensor]
--AS
--SELECT 
--s.ID,
--s.Code,
--s.Name,
--s.[Description],
--s.Url,
--s.StationID,
--s.DataSourceID,
--s.PhenomenonID,
----> Added 2.0.7 20160609 TimPN
--  [Phenomenon].Name PhenomenonName,
----< Added 2.0.7 20160609 TimPN
--s.UserId,
--st.Name AS StationName,
--d.Name AS DataSourceName,
----> Added 20161110 TimPN
--s.DataSchemaID,
----< Added 20161110 TimPN
----> Changed 20161110 TimPN
----ds.[Description] DataSchemaName
--ds.[Name] DataSchemaName
----< Changed 20161110 TimPN
--FROM Sensor s
--INNER JOIN DataSource d
--    on s.DataSourceID = d.ID
--INNER JOIN Station st
--    on s.StationID = st.ID
--LEFT JOIN DataSchema ds
--    on s.DataSchemaID = ds.ID
----> Added 2.0.7 20160609 TimPN
--  inner join [Phenomenon]
--    on (s.PhenomenonID = [Phenomenon].ID)
----< Added 2.0.7 20160609 TimPN
--< Removed 2.0.17 20161128 TimPN
--> Added 2.0.17 20161128 TimPN
ALTER VIEW [dbo].[vSensor]
AS
SELECT 
  Sensor.ID,
  Sensor.Code,
  Sensor.Name,
  Sensor.[Description],
  Sensor.Url,
  Sensor.DataSourceID,
  Sensor.PhenomenonID,
  [Phenomenon].Name PhenomenonName,
  Sensor.UserId,
  Site.Name Site,
  Station.Name Station,
  Instrument.Name Instrument,
  d.Name AS DataSourceName,
  Sensor.DataSchemaID,
  ds.[Name] DataSchemaName
FROM 
  Sensor 
  left join Instrument_Sensor
	on (Instrument_Sensor.SensorID = Sensor.ID)
  left join Instrument
	on (Instrument_Sensor.InstrumentID = Instrument.ID)
  left join Station_Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID)
  left join Station 
    on (Station_Instrument.StationID = Station.ID)
  inner join Site
    on (Station.SiteID = Site.ID)
INNER JOIN DataSource d
    on Sensor.DataSourceID = d.ID
LEFT JOIN DataSchema ds
    on Sensor.DataSchemaID = ds.ID
  inner join [Phenomenon]
    on (Sensor.PhenomenonID = [Phenomenon].ID)
--< Added 2.0.17 20161128 TimPN
GO
PRINT N'Refreshing [dbo].[vSiteOrganisation]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vSiteOrganisation]';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[vStation]...';


GO
ALTER VIEW [dbo].[vStation]
AS
SELECT 
  Station.*,
  s.Code SiteCode,
  s.Name SiteName
FROM 
  Station
  LEFT JOIN [Site] s 
    on (Station.SiteID = s.ID)
GO
PRINT N'Refreshing [dbo].[vStationInstrument]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vStationInstrument]';


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
PRINT N'Refreshing [dbo].[vObservationRoles]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vObservationRoles]';


GO
PRINT N'Checking existing data against newly created constraints';


GO
USE [$(DatabaseName)];


GO
ALTER TABLE [dbo].[Station] WITH CHECK CHECK CONSTRAINT [FK_Station_Site];


GO
PRINT N'Update complete.';


GO
