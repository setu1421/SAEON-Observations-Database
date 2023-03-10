/*
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
  Max(TopLatitude) TopLatitude, Min(BottomLatitude) BottomLatitude,
  Min(LeftLongitude) LeftLongitude, Max(RightLongitude) RightLongitude
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
