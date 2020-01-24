CREATE VIEW [dbo].[vSensorThingsAPILocations]
AS
-- Stations
Select
  Station.ID, Station.Code, Station.Name, Station.Description, Station.Latitude, Station.Longitude, Station.Elevation,
  vSensorThingsAPIStationDates.StartDate, vSensorThingsAPIStationDates.EndDate
from
  vInventory
  inner join Station
    on (vInventory.StationID = Station.ID)
  left join vSensorThingsAPIStationDates
    on (vSensorThingsAPIStationDates.ID = Station.ID)
  where
    (Station.Latitude is not null) and (Station.Longitude is not null)
union
-- Instruments
Select
  Instrument.ID, Instrument.Code, Instrument.Name, Instrument.Description, Instrument.Latitude, Instrument.Longitude, Instrument.Elevation,
  vSensorThingsAPIInstrumentDates.StartDate, vSensorThingsAPIInstrumentDates.EndDate
from
  vInventory
  inner join Instrument
    on (vInventory.InstrumentID = Instrument.ID)
  left join vSensorThingsAPIInstrumentDates
    on (vSensorThingsAPIInstrumentDates.ID = Instrument.ID)
  where
    (Instrument.Latitude is not null) and (Instrument.Longitude is not null)
