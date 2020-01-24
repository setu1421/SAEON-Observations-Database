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
  -- EndDate
  -- Instrument
  EndDate InstrumentEndDate,
  -- Instrument_Sensor
  (Select Max(Instrument_Sensor.EndDate) from Instrument_Sensor where Instrument_Sensor.InstrumentID = Instrument.ID) InstrumentSensorEndDate
from
  Instrument
)
Select
  --*,
  ID, Name,
  (Select Min(Value) from (Values (InstrumentStartDate), (InstrumentSensorStartDate)) as x(Value)) StartDate,
  (Select Max(Value) from (Values (InstrumentEndDate), (InstrumentSensorEndDate)) as x(Value)) EndDate
from
  Dates
