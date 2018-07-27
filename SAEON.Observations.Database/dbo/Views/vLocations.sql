CREATE VIEW [dbo].[vLocations]
AS
-- Organisation Stations from Sites
Select
  OrganisationID, Station.SiteID, Station.ID StationID
from
  Organisation_Site
  inner join Station
    on (Organisation_Site.SiteID = Station.SiteID)
where
  ((Station.Latitude is not null) and (Station.Longitude is not null)) and
  Exists(Select * from ImportBatchSummary where (SiteID = Station.SiteID) and (StationID = Station.ID))
union
-- Organisation Stations
Select
  OrganisationID, SiteID, StationID
from
  Organisation_Station
  inner join Station
    on (Organisation_Station.StationID = Station.ID)
where
  ((Station.Latitude is not null) and (Station.Longitude is not null)) and
  Exists(Select * from ImportBatchSummary where (SiteID = Station.SiteID) and (StationID = Station.ID))
union
-- Organisation Stations from Instruments
Select
  OrganisationID, SiteID, StationID
from
  Organisation_Instrument
  inner join Station_Instrument
    on (Organisation_Instrument.InstrumentID = Station_Instrument.InstrumentID)
  inner join Station
    on (Station_Instrument.StationID = Station.ID)
where
  ((Station.Latitude is not null) and (Station.Longitude is not null)) and
  Exists(Select * from ImportBatchSummary where (SiteID = Station.SiteID) and (StationID = Station.ID))
