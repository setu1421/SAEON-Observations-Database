CREATE VIEW [dbo].[vSensorThingsAPILocations]
AS
-- Stations
Select
  StationID ID, StationName Name, Station.Description, Station.Latitude, Station.Longitude, Station.Elevation
from
  vInventory
  inner join Station
    on (vInventory.StationID = Station.ID)
  where
    (Station.Latitude is not null) and (Station.Longitude is not null)
union
-- Instruments
Select
  InstrumentID ID, InstrumentName Name, Instrument.Description, Instrument.Latitude, Instrument.Longitude, Instrument.Elevation
from
  vInventory
  inner join Instrument
    on (vInventory.InstrumentID = Instrument.ID)
  where
    (Instrument.Latitude is not null) and (Instrument.Longitude is not null)
