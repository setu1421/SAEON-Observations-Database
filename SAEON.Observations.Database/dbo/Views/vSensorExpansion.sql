CREATE VIEW [dbo].[vSensorExpansion]
AS 
Select Distinct
  Sensor.ID SensorID, Sensor.Code SensorCode, Sensor.Name SensorName, Sensor.Description SensorDescription, Sensor.Url SensorUrl,
  Instrument_Sensor.StartDate InstrumenSensorStartDate, Instrument_Sensor.EndDate InstrumenSensorEndDate,
  Instrument.ID InstrumentID, Instrument.Code InstrumentCode, Instrument.Name InstrumentName, Instrument.Description InstrumentDescription, Instrument.Url InstrumentUrl, Instrument.StartDate InstrumentStartDate, Instrument.EndDate InstrumentEndDate,
  Station_Instrument.StartDate StationInstrumentStartDate, Station_Instrument.EndDate StationInstrumentEndDate,
  Station.ID StationID, Station.Code StationCode, Station.Name StationName, Station.Description StationDescription, Station.Url StationUrl, Station.StartDate StationStartDate, Station.EndDate StationEndDate,
  Site.ID SiteID, Site.Code SiteCode, Site.Name SiteName, Site.Description SiteDescription, Site.Url SiteUrl, Site.StartDate SiteStartDate, Site.EndDate SiteEndDate,
  Project.ID ProjectID, Project.Code ProjectCode, Project.Name ProjectName, Project.Description ProjectDescription, Project.Url ProjectUrl,
  Programme.ID ProgrammeID, Programme.Code ProgrammeCode, Programme.Name ProgrammeName, Programme.Description ProgrammeDescription, Programme.Url ProgrammeUrl,
  Organisation.ID OrganisationID, Organisation.Code OrganisationCode, Organisation.Name OrganisationName, Organisation.Description OrganisationDescription, Organisation.Url OrganisationUrl,
  Coalesce(Sensor.Latitude, Instrument_Sensor.Latitude, Instrument.Latitude, Station_Instrument.Latitude, Station.Latitude) Latitude,
  Coalesce(Sensor.Longitude, Instrument_Sensor.Longitude, Instrument.Longitude, Station_Instrument.Longitude, Station.Longitude) Longitude,
  Coalesce(Sensor.Elevation, Instrument_Sensor.Elevation, Instrument.Elevation, Station_Instrument.Elevation, Station.Elevation) Elevation,
  (
  Select
    Max(v)
  from
    (Values (Instrument_Sensor.StartDate),(Instrument.StartDate),(Station_Instrument.StartDate),(Station.StartDate),(Site.StartDate)) as Value(v)
  ) StartDate,
  (
  Select
    Min(v)
  from
    (Values (Instrument_Sensor.EndDate),(Instrument.EndDate),(Station_Instrument.EndDate),(Station.EndDate),(Site.EndDate)) as Value(v)
  ) EndDate
from
  Sensor
  inner join Instrument_Sensor
    on (Sensor.ID = Instrument_Sensor.SensorID) 
  inner join Instrument
    on (Instrument_Sensor.InstrumentID = Instrument.ID) 
  inner join Station_Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID) 
  inner join Station
    on (Station_Instrument.StationID = Station.ID) 
  inner join Site
    on (Station.SiteID = Site.ID) 
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

