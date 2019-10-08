CREATE VIEW [dbo].[vSensorThingsAPIThings]
AS
-- Sites
Select
  SiteID ID, SiteName Name, Site.Description, 'Site' Kind, Site.Url, Site.StartDate, Site.EndDate
from
  vInventory
  inner join Site
    on (vInventory.SiteID = Site.ID)
union
-- Stations
Select
  StationID ID, StationName Name, Station.Description, 'Station' Kind, Station.Url, Station.StartDate, Station.EndDate
from
  vInventory
  inner join Station
    on (vInventory.StationID = Station.ID)
union
-- Instruments
Select
  InstrumentID ID, InstrumentName Name, Instrument.Description, 'Instrument' Kind, Instrument.Url, Instrument.StartDate, Instrument.EndDate
from
  vInventory
  inner join Instrument
    on (vInventory.InstrumentID = Instrument.ID)
