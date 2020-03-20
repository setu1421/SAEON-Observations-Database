CREATE VIEW [dbo].[vSensorThingsAPILocations]
AS
With StationLocations
as
(
Select Distinct
  Station.ID, Station.Code, Station.Name, Station.Description,
  Coalesce(Station.Latitude, Station_Instrument.Latitude, Instrument.Latitude, Instrument_Sensor.Latitude, Sensor.Latitude) Latitude,
  Coalesce(Station.Longitude, Station_Instrument.Longitude, Instrument.Longitude, Instrument_Sensor.Longitude, Sensor.Longitude) Longitude,
  Coalesce(Station.Elevation, Station_Instrument.Elevation, Instrument.Elevation, Instrument_Sensor.Elevation, Sensor.Elevation) Elevation,
  vSensorThingsAPIInstrumentDates.StartDate, vSensorThingsAPIInstrumentDates.EndDate
from
  vInventory
  inner join Station
    on (vInventory.StationID = Station.ID)
  inner join Station_Instrument
    on (Station_Instrument.StationID = Station.ID)
  inner join Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID)
  inner join Instrument_Sensor
    on (Instrument_Sensor.InstrumentID = Instrument.ID)
  inner join Sensor
    on (Instrument_Sensor.SensorID = Sensor.ID)
  left join vSensorThingsAPIInstrumentDates
    on (vSensorThingsAPIInstrumentDates.ID = Instrument.ID)
),
InstrumentLocations
as
(
Select Distinct
  Instrument.ID, Instrument.Code, Instrument.Name, Instrument.Description,
  Coalesce(Instrument.Latitude, Instrument_Sensor.Latitude, Sensor.Latitude, Station_Instrument.Latitude, Station.Latitude) Latitude,
  Coalesce(Instrument.Longitude, Instrument_Sensor.Longitude, Sensor.Longitude, Station_Instrument.Longitude, Station.Longitude) Longitude,
  Coalesce(Instrument.Elevation, Instrument_Sensor.Elevation, Sensor.Elevation, Station_Instrument.Elevation, Station.Elevation) Elevation,
  vSensorThingsAPIInstrumentDates.StartDate, vSensorThingsAPIInstrumentDates.EndDate
from
  vInventory
  inner join Instrument
    on (vInventory.InstrumentID = Instrument.ID)
  inner join Instrument_Sensor
    on (Instrument_Sensor.InstrumentID = Instrument.ID)
  inner join Sensor
    on (Instrument_Sensor.SensorID = Sensor.ID)
  inner join Station_Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID)
  inner join Station
    on(Station_Instrument.StationID = Station.ID)
  left join vSensorThingsAPIInstrumentDates
    on (vSensorThingsAPIInstrumentDates.ID = Instrument.ID)
)
Select * from StationLocations where (Latitude is not null) and (Longitude is not null)
union
Select * from InstrumentLocations where (Latitude is not null) and (Longitude is not null)
