CREATE VIEW [dbo].[vStationInstrument] AS 
SELECT 
  src.*, 
  [Station].Code StationCode,
  [Station].Name StationName,
  [Instrument].Code InstrumentCode,
  [Instrument].Name InstrumentName
FROM 
  [Station_Instrument] src
  inner join [Station]
    on (src.StationID = [Station].ID)
  inner join [Instrument] 
    on (src.InstrumentID = [Instrument].ID)
