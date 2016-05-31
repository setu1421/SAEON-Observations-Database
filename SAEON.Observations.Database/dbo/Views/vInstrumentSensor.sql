--> Added 2.0.5 20160531 TimPN
CREATE VIEW [dbo].[vInstrumentSensor] AS 
SELECT 
  src.*, 
  [Instrument].Code InstrumentCode,
  [Instrument].Name InstrumentName,
  [Sensor].Code SensorCode,
  [Sensor].Name SensorName
FROM 
  [Instrument_Sensor] src
  inner join [Instrument] 
    on (src.InstrumentID = [Instrument].ID)
  inner join [Sensor]
    on (src.SensorID = [Sensor].ID)
--< Added 2.0.5 20160531 TimPN
