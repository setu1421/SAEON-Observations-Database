CREATE VIEW [dbo].[vSensorThingsAPIThings]
AS
-- Sites
Select
  Site.ID, Site.Code, Site.Name, Site.Description, 'Site' Kind, Site.Url, Site.StartDate, Site.EndDate
from
  vInventorySensors
  inner join Site
    on (vInventorySensors.SiteID = Site.ID)
union
-- Stations
Select
  Station.ID, Station.Code, Station.Name, Station.Description, 'Station' Kind, Station.Url,
  vSensorThingsAPIStationDates.StartDate, vSensorThingsAPIStationDates.EndDate
from
  vInventorySensors
  inner join Station
    on (vInventorySensors.StationID = Station.ID)
  left join vSensorThingsAPIStationDates
    on (vSensorThingsAPIStationDates.ID = Station.ID)
union
-- Instruments
Select
  Instrument.ID, Instrument.Code, Instrument.Name, Instrument.Description, 'Instrument' Kind, Instrument.Url,
  vSensorThingsAPIInstrumentDates.StartDate, vSensorThingsAPIInstrumentDates.EndDate
from
  vInventorySensors
  inner join Instrument
    on (vInventorySensors.InstrumentID = Instrument.ID)
  left join vSensorThingsAPIInstrumentDates
    on (vSensorThingsAPIInstrumentDates.ID = Instrument.ID)
