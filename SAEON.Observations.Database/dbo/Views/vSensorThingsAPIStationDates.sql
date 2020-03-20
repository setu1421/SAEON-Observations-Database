CREATE VIEW [dbo].[vSensorThingsAPIStationDates]
AS
with Dates
as
(
Select
  ID, Name,
  -- StartDate
  -- Station
  StartDate StationStartDate,
  -- Station_Instrument
  (Select Min(Station_Instrument.StartDate) from Station_Instrument where Station_Instrument.StationID = Station.ID) StationInstrumentStartDate,
  -- Instrument
  (
  Select
    Min(Instrument.StartDate)
  from
    Station_Instrument
    inner join Instrument
      on (Station_Instrument.InstrumentID = Instrument.ID)
    where
      (Station_Instrument.StationID = Station.ID)
  ) InstrumentStartDate,
  -- Instrument_Sensor
  (
  Select
    Min(Instrument_Sensor.StartDate)
  from
    Station_Instrument
    inner join Instrument
      on (Station_Instrument.InstrumentID = Instrument.ID)
    inner join Instrument_Sensor
      on (Instrument_Sensor.InstrumentID = Instrument.ID)
    where
      (Station_Instrument.StationID = Station.ID)
  ) InstrumentSensorStartDate,
  -- EndDate
  -- Station
  EndDate StationEndDate,
  -- Station_Instrument
  (Select Max(Station_Instrument.EndDate) from Station_Instrument where Station_Instrument.StationID = Station.ID) StationInstrumentEndDate,
  -- Instrument
  (
  Select
    Max(Instrument.EndDate)
  from
    Station_Instrument
    inner join Instrument
      on (Station_Instrument.InstrumentID = Instrument.ID)
    where
      (Station_Instrument.StationID = Station.ID)
  ) InstrumentEndDate,
  -- Instrument_Sensor
  (
  Select
    Max(Instrument_Sensor.EndDate)
  from
    Station_Instrument
    inner join Instrument
      on (Station_Instrument.InstrumentID = Instrument.ID)
    inner join Instrument_Sensor
      on (Instrument_Sensor.InstrumentID = Instrument.ID)
    where
      (Station_Instrument.StationID = Station.ID)
  ) InstrumentSensorEndDate
from
  Station
)
Select
  --*,
  ID, Name,
  (Select Min(Value) from (Values (StationStartDate), (StationInstrumentStartDate), (InstrumentStartDate), (InstrumentSensorStartDate)) as x(Value)) StartDate,
  (Select Max(Value) from (Values (StationEndDate), (StationInstrumentEndDate), (InstrumentEndDate), (InstrumentSensorEndDate)) as x(Value)) EndDate
from
  Dates
