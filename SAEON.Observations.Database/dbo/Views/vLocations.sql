CREATE VIEW [dbo].[vLocations]
AS
-- Organisation Stations from Sites
Select
  OrganisationID, Organisation.Name OrganisationName, Station.SiteID, Site.Name SiteName, Station.ID StationID, Station.Name StationName, Station.Latitude, Station.Longitude, Station.Elevation, Station.Url
from
  Organisation_Site
  inner join Organisation
    on (Organisation_Site.OrganisationID = Organisation.ID)
  inner join Site  
    on (Organisation_Site.SiteID = Site.ID)
  inner join Station
    on (Organisation_Site.SiteID = Station.SiteID)
where
  Exists(Select * from ImportBatchSummary where (SiteID = Station.SiteID) and (StationID = Station.ID))
union
-- Organisation Stations
Select
  OrganisationID, Organisation.Name OrganisationName, SiteID, Site.Name SiteName, StationID, Station.Name StationName, Station.Latitude, Station.Longitude, Station.Elevation, Station.Url
from
  Organisation_Station
  inner join Organisation
    on (Organisation_Station.OrganisationID = Organisation.ID)
  inner join Station
    on (Organisation_Station.StationID = Station.ID)
  inner join Site
    on (Station.SiteID = Site.ID)
where
  Exists(Select * from ImportBatchSummary where (SiteID = Station.SiteID) and (StationID = Station.ID))
union
-- Organisation Stations from Instruments
Select
  OrganisationID, Organisation.Name OrganisationName, SiteID, Site.Name SiteName, StationID, Station.Name StationName, Station.Latitude, Station.Longitude, Station.Elevation, Station.Url
from
  Organisation_Instrument
  inner join Organisation
    on (Organisation_Instrument.OrganisationID = Organisation.ID)
  inner join Station_Instrument
    on (Organisation_Instrument.InstrumentID = Station_Instrument.InstrumentID)
  inner join Station
    on (Station_Instrument.StationID = Station.ID)
  inner join Site
    on (Station.SiteID = Site.ID)
where
  Exists(Select * from ImportBatchSummary where (SiteID = Station.SiteID) and (StationID = Station.ID))
