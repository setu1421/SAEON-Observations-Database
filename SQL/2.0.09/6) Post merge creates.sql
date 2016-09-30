--use [ObservationDB]
Declare @TimUserId UniqueIdentifier = (Select UserId from aspnet_Users where LoweredUserName = 'tim')
-- Programme
print 'Programmes'
Insert into Programme (Code, Name, Description, UserId) 
Values ('SAEON','SAEON','South African Environmental Obersvation Network',@TimUserId)
-- Project
print 'Projects'
Insert into Project (ProgrammeID, Code, Name, Description, UserId) 
Values ((Select ID from Programme where Code = 'SAEON'), 'SAEON','SAEON','South African Environmental Obersvation Network',@TimUserId)
-- Site
print 'Sites'
Insert into Site (Code, Name, Description, UserId)
Select Code, Name, Description, @TimUserId from ProjectSite
-- Organisation Site link
print 'Organisation Site link'
Insert into Organisation_Site (OrganisationID,SiteID,OrganisationRoleID,UserId)
Select OrganisationID, (Select ID from Site where Site.Code = ProjectSite.Code), (Select ID from OrganisationRole where Code = 'Owner'), @TimUserId  from ProjectSite
-- Station Site link
print 'Station Site link'
Update Station set SiteID = (Select ID from Site where Code = ps.code)
from
  Station s
  inner join ProjectSite ps
    on (s.ProjectSiteID = ps.ID)
-- Project Station link
print 'Project Station link'
Insert into Project_Station (ProjectID, StationID, UserId)
Select (Select ID from Project where Code = 'SAEON'),Station.ID,@TimUserId from Station
-- Instruments
print 'Instruments'
Insert into Instrument (Code, Name, Description, Url, UserId)
Select Code, Name, Description, Url, @TimUserId from Station
-- Instrument Station link
print 'Instrument Station link'
Insert into Station_Instrument (StationID, InstrumentID, UserId)
Select 
  Station.ID, Instrument.ID, @TimUserId
from
  Station
  inner join Instrument
    on (Station.Code = Instrument.Code)
-- Instrument Sensor link
print 'Instrument Sensor link'
Insert into Instrument_Sensor (InstrumentID, SensorID, UserId)
Select
  Instrument.ID, Sensor.ID, @TimUserId
from
  Sensor
  inner join Station
    on (Sensor.StationID = Station.ID)
  inner join Instrument
    on (Station.Code = Instrument.Code)
-- Instrument DataSource link
print 'Instrument DataSource link'
Insert into Instrument_DataSource (InstrumentID, DataSourceID, UserId)
Select Distinct
  Instrument.ID, DataSource.ID, @TimUserId
from
  Sensor
  inner join DataSource
    on (Sensor.DataSourceID = DataSource.ID)
  inner join Station
    on (Sensor.StationID = Station.ID)
  inner join Instrument
    on (Station.Code = Instrument.Code)
