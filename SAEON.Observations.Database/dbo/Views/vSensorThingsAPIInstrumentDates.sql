CREATE VIEW [dbo].[vSensorThingsAPIInstrumentDates]
AS
with Dates
as
(
Select
  ID, Name,
  -- StartDate
  -- Instrument
  StartDate InstrumentStartDate,
  -- Instrument_Sensor
  (Select Min(Instrument_Sensor.StartDate) from Instrument_Sensor where Instrument_Sensor.InstrumentID = Instrument.ID) InstrumentSensorStartDate,
  -- StationInstrument
  (Select Min(Station_Instrument.StartDate) from Station_Instrument where Station_Instrument.InstrumentID = Instrument.ID) StationInstrumentStartDate,
  -- Station
  (
  Select
    Min(Station.StartDate)
  from
    Station_Instrument
      inner join Station
        on (Station_Instrument.StationID = Station.ID)
  where
    Station_Instrument.InstrumentID = Instrument.ID) StationStartDate,
  -- EndDate
  -- Instrument
  EndDate InstrumentEndDate,
  -- Instrument_Sensor
  (Select Max(Instrument_Sensor.EndDate) from Instrument_Sensor where Instrument_Sensor.InstrumentID = Instrument.ID) InstrumentSensorEndDate,
  -- StationInstrument
  (Select Max(Station_Instrument.EndDate) from Station_Instrument where Station_Instrument.InstrumentID = Instrument.ID) StationInstrumentEndDate,
  -- Station
  (
  Select
    Max(Station.EndDate)
  from
    Station_Instrument
      inner join Station
        on (Station_Instrument.StationID = Station.ID)
  where
    Station_Instrument.InstrumentID = Instrument.ID) StationEndDate
from
  Instrument
)
Select
  --*,
  ID, Name,
  (Select Min(Value) from (Values (InstrumentStartDate), (InstrumentSensorStartDate), (StationInstrumentStartDate), (StationStartDate)) as x(Value)) StartDate,
  (Select Max(Value) from (Values (InstrumentEndDate), (InstrumentSensorEndDate), (StationInstrumentEndDate), (StationEndDate)) as x(Value)) EndDate
from
  Dates
