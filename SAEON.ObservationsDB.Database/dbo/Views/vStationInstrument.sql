--> Added 2.0.0.5 20160512 TimPN
CREATE VIEW [dbo].[vStationInstrument] AS 
SELECT 
  src.*, [Instrument].Name InstrumentName
FROM 
  [Station_Instrument] src
  inner join [Instrument] 
    on (src.InstrumentID = [Instrument].ID)
--< Added 2.0.0.5 20160512 TimPN

