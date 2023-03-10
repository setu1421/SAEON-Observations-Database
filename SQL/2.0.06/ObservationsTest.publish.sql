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
:setvar DefaultDataPath "C:\Program Files\Microsoft SQL Server\MSSQL12.SAEON\MSSQL\DATA\"
:setvar DefaultLogPath "C:\Program Files\Microsoft SQL Server\MSSQL12.SAEON\MSSQL\DATA\"

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
PRINT N'Altering [dbo].[vInstrumentOrganisation]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
--> Added 2.0.6 20160607 TimPN
ALTER VIEW [dbo].[vInstrumentOrganisation]
AS
Select
  [Organisation_Site].ID,
  OrganisationID,
  [Organisation].Code OrganisationCode,
  [Organisation].Name OrganisationName,
  [Instrument].ID InstrumentID,
  [Instrument].Code InstrumentCode,
  [Instrument].Name InstrumentName,
  OrganisationRoleID, 
  [OrganisationRole].Code OrganisationRoleCode,
  [OrganisationRole].Name OrganisationRoleName,
  [Organisation_Site].StartDate,
  [Organisation_Site].EndDate,
  'Site' [Level],
  [Site].Code LevelCode,
  [Site].Name LevelName,
  0 [Weight],
  CAST(1 as bit) [IsReadOnly]
from
  [Organisation_Site]
  inner join [Site]
    on ([Organisation_Site].SiteID = [Site].ID)
  inner join [Organisation]
    on ([Organisation_Site].OrganisationID = [Organisation].ID)
  inner join [Station] 
    on ([Organisation_Site].SiteID = [Station].SiteID)
  inner join [Station_Instrument]
    on ([Station_Instrument].StationID = [Station].ID)
  inner join [Instrument]
    on ([Station_Instrument].InstrumentID = [Instrument].ID)
  inner join [OrganisationRole]
    on ([Organisation_Site].OrganisationRoleID = [OrganisationRole].ID)
union
Select 
  [Organisation_Station].ID,
  OrganisationID,
  [Organisation].Code OrganisationCode,
  [Organisation].Name OrganisationName,
  InstrumentID,
  [Instrument].Code InstrumentCode,
  [Instrument].Name InstrumentName,
  OrganisationRoleID,
  [OrganisationRole].Code OrganisationRoleCode,
  [OrganisationRole].Name OrganisationRoleName,
  [Organisation_Station].StartDate,
  [Organisation_Station].EndDate,
  'Station' [Level],
  [Station].Code LevelCode,
  [Station].Name LevelName,
  1 [Weight],
  CAST(1 as bit) [IsReadOnly]
from 
  [Organisation_Station]
  inner join [Station] 
    on ([Organisation_Station].StationID = [Station].ID)
  inner join [Station_Instrument]
    on ([Organisation_Station].StationID = [Station_Instrument].StationID)
  inner join Instrument
    on ([Station_Instrument].InstrumentID = [Instrument].ID)
  inner join [Organisation]
    on ([Organisation_Station].OrganisationID = [Organisation].ID)
  inner join [OrganisationRole]
    on ([Organisation_Station].OrganisationRoleID = [OrganisationRole].ID)
union
Select
  [Organisation_Instrument].ID,
  OrganisationID,
  [Organisation].Code OrganisationCode,
  [Organisation].Name OrganisationName,
  InstrumentID,
  [Instrument].Code InstrumentCode,
  [Instrument].Name InstrumentName,
  OrganisationRoleID,
  [OrganisationRole].Code OrganisationRoleCode,
  [OrganisationRole].Name OrganisationRoleName,
  [Organisation_Instrument].StartDate,
  [Organisation_Instrument].EndDate,
  'Instrument' [Level],
  [Instrument].Code LevelCode,
  [Instrument].Name LevelName,
  2 [Weight],
  CAST(0 as bit) [IsReadOnly]
from
  [Organisation_Instrument]
  inner join [Instrument]
    on ( [Organisation_Instrument].InstrumentID = [Instrument].ID)
  inner join [Organisation]
    on ( [Organisation_Instrument].OrganisationID = [Organisation].ID)
  inner join [OrganisationRole]
    on ( [Organisation_Instrument].OrganisationRoleID = [OrganisationRole].ID)
--< Added 2.0.6 20160607 TimPN
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[vSiteOrganisation]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
--> Added 2.0.6 20160607 TimPN
ALTER VIEW [dbo].[vSiteOrganisation]
AS 
Select
  [Organisation_Site].ID,
  OrganisationID,
  [Organisation].Code OrganisationCode,
  [Organisation].Name OrganisationName,
  SiteID,
  [Site].Code SiteCode,
  [Site].Name SiteName,
  OrganisationRoleID, 
  [OrganisationRole].Code OrganisationRoleCode,
  [OrganisationRole].Name OrganisationRoleName,
  [Organisation_Site].StartDate,
  [Organisation_Site].EndDate,
  'Site' [Level],
  [Site].Code LevelCode,
  [Site].Name LevelName,
  0 [Weight],
  CAST(0 as bit) [IsReadOnly]
from
  [Organisation_Site]
  inner join [Site]
    on ([Organisation_Site].SiteID = [Site].ID)
  inner join [Organisation]
    on ([Organisation_Site].OrganisationID = [Organisation].ID)
  inner join [OrganisationRole]
    on ([Organisation_Site].OrganisationRoleID = [OrganisationRole].ID)
union
Select
  [Organisation_Station].ID,
  OrganisationID,
  [Organisation].Code OrganisationCode,
  [Organisation].Name OrganisationName,
  SiteID,
  [Site].Code SiteCode,
  [Site].Name SiteName,
  OrganisationRoleID,
  [OrganisationRole].Code OrganisationRoleCode,
  [OrganisationRole].Name OrganisationRoleName,
  [Organisation_Station].StartDate,
  [Organisation_Station].EndDate,
  'Station' [Level],
  [Station].Code LevelCode,
  [Station].Name LevelName,
  1 [Weight],
  CAST(1 as bit) [IsReadOnly]
from
  [Organisation_Station]
  inner join [Station]
    on ([Organisation_Station].StationID = [Station].ID)
  inner join [Organisation]
    on ([Organisation_Station].OrganisationID = [Organisation].ID)
  inner join [Site]
    on ([Station].SiteID = [Site].ID)
  inner join [OrganisationRole]
    on ([Organisation_Station].OrganisationRoleID = [OrganisationRole].ID)
union
Select
  [Organisation_Instrument].ID,
  OrganisationID,
  [Organisation].Code OrganisationCode,
  [Organisation].Name OrganisationName,
  SiteID,
  [Site].Code SiteCode,
  [Site].Name SiteName,
  OrganisationRoleID,
  [OrganisationRole].Code OrganisationRoleCode,
  [OrganisationRole].Name OrganisationRoleName,
  [Organisation_Instrument].StartDate,
  [Organisation_Instrument].EndDate,
  'Instrument' [Level],
  [Instrument].Code LevelCode,
  [Instrument].Name LevelName,
  2 [Weight],
  CAST(1 as bit) [IsReadOnly]
from
  [Organisation_Instrument]
  inner join [Instrument]
    on ([Organisation_Instrument].InstrumentID = [Instrument].ID)
  inner join [Organisation]
    on ([Organisation_Instrument].OrganisationID = [Organisation].ID)
  inner join [Station_Instrument]
    on ([Organisation_Instrument].InstrumentID = [Station_Instrument].InstrumentID)
  inner join [Station]
    on ([Station_Instrument].StationID = [Station].ID)
  inner join [Site]
    on ([Station].SiteID = [Site].ID)
  inner join [OrganisationRole]
    on ([Organisation_Instrument].OrganisationRoleID = [OrganisationRole].ID)
--< Added 2.0.6 20160607 TimPN
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[vStationOrganisation]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
--> Added 2.0.6 20160607 TimPN
ALTER VIEW [dbo].[vStationOrganisation]
AS
Select
  [Organisation_Site].ID,
  OrganisationID,
  [Organisation].Code OrganisationCode,
  [Organisation].Name OrganisationName,
  [Station].ID StationID,
  [Station].Code StationCode,
  [Station].Name StationName,
  OrganisationRoleID, 
  [OrganisationRole].Code OrganisationRoleCode,
  [OrganisationRole].Name OrganisationRoleName,
  [Organisation_Site].StartDate,
  [Organisation_Site].EndDate,
  'Site' [Level],
  [Site].Code LevelCode,
  [Site].Name LevelName,
  0 [Weight],
  CAST(1 as bit) [IsReadOnly]
from
  [Organisation_Site]
  inner join [Site]
    on ([Organisation_Site].SiteID = [Site].ID)
  inner join [Organisation]
    on ([Organisation_Site].OrganisationID = [Organisation].ID)
  inner join [Station] 
    on ([Organisation_Site].SiteID = [Station].SiteID)
  inner join [OrganisationRole]
    on ([Organisation_Site].OrganisationRoleID = [OrganisationRole].ID)
union
Select 
  [Organisation_Station].ID,
  OrganisationID,
  [Organisation].Code OrganisationCode,
  [Organisation].Name OrganisationName,
  StationID,
  [Station].Code StationCode,
  [Station].Name StationName,
  OrganisationRoleID,
  [OrganisationRole].Code OrganisationRoleCode,
  [OrganisationRole].Name OrganisationRoleName,
  [Organisation_Station].StartDate,
  [Organisation_Station].EndDate,
  'Station' [Level],
  [Station].Code LevelCode,
  [Station].Name LevelName,
  1 [Weight],
  CAST(0 as bit) [IsReadOnly]
from 
  [Organisation_Station]
  inner join [Station] 
    on ([Organisation_Station].StationID = [Station].ID)
  inner join [Organisation]
    on ([Organisation_Station].OrganisationID = [Organisation].ID)
  inner join [OrganisationRole]
    on ([Organisation_Station].OrganisationRoleID = [OrganisationRole].ID)
union
Select
  [Organisation_Instrument].ID,
  OrganisationID,
  [Organisation].Code OrganisationCode,
  [Organisation].Name OrganisationName,
  [Station].ID StationID,
  [Station].Code StationCode,
  [Station].Name StationName,
  OrganisationRoleID,
  [OrganisationRole].Code OrganisationRoleCode,
  [OrganisationRole].Name OrganisationRoleName,
  [Organisation_Instrument].StartDate,
  [Organisation_Instrument].EndDate,
  'Instrument' [Level],
  [Instrument].Code LevelCode,
  [Instrument].Name LevelName,
  2 [Weight],
  CAST(1 as bit) [IsReadOnly]
from
  [Organisation_Instrument]
  inner join [Instrument]
    on ([Organisation_Instrument].InstrumentID = [Instrument].ID)
  inner join [Organisation]
    on ( [Organisation_Instrument].OrganisationID = [Organisation].ID)
  inner join [Station_Instrument]
    on ( [Organisation_Instrument].InstrumentID = [Station_Instrument].InstrumentID)
  inner join [Station]
    on ( [Station_Instrument].StationID = [Station].ID)
  inner join [OrganisationRole]
    on ( [Organisation_Instrument].OrganisationRoleID = [OrganisationRole].ID)
--< Added 2.0.6 20160607 TimPN
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Update complete.';


GO
