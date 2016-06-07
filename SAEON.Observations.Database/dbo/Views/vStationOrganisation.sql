﻿--> Added 2.0.6 20160607 TimPN
CREATE VIEW [dbo].[vStationOrganisation]
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
  0 [Weight],
  CAST(1 as bit) [IsReadOnly]
from
  [Organisation_Site]
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
  1 [Weight],
  CAST(0 as bit) [IsReadOnly]
from 
  [Organisation_Station]
  inner join [Organisation]
    on ([Organisation_Station].OrganisationID = [Organisation].ID)
  inner join [Station] 
    on ([Organisation_Station].StationID = [Station].ID)
  inner join [OrganisationRole]
    on ([Organisation_Station].OrganisationRoleID = [OrganisationRole].ID)
union
Select
  [Organisation_Instrument].ID,
  OrganisationID,
  [Organisation].Code OrganisationCode,
  [Organisation].Name OrganisationName,
  StationID,
  [Station].Code StationCode,
  [Station].Name StationName,
  OrganisationRoleID,
  [OrganisationRole].Code OrganisationRoleCode,
  [OrganisationRole].Name OrganisationRoleName,
  [Organisation_Instrument].StartDate,
  [Organisation_Instrument].EndDate,
  'Instrument' [Level],
  2 [Weight],
  CAST(1 as bit) [IsReadOnly]
from
  [Organisation_Instrument]
  inner join [Organisation]
    on ( [Organisation_Instrument].OrganisationID = [Organisation].ID)
  inner join [Station_Instrument]
    on ( [Organisation_Instrument].InstrumentID = [Station_Instrument].InstrumentID)
  inner join [Station]
    on ( [Station_Instrument].StationID = [Station].ID)
  inner join [OrganisationRole]
    on ( [Organisation_Instrument].OrganisationRoleID = [OrganisationRole].ID)
--< Added 2.0.6 20160607 TimPN

