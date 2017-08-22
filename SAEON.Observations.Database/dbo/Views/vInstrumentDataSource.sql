--> Removed 2.0.34 TimPN 20170821
/*
--> Added 2.0.9 20160727 TimPN
CREATE VIEW [dbo].[vInstrumentDataSource] AS 
SELECT 
  src.*, 
  [Instrument].Code InstrumentCode,
  [Instrument].Name InstrumentName,
  [DataSource].Code DataSourceCode,
  [DataSource].Name DataSourceName
FROM 
  [Instrument_DataSource] src
  inner join [Instrument] 
    on (src.InstrumentID = [Instrument].ID)
  inner join [DataSource]
    on (src.DataSourceID = [DataSource].ID)
--< Added 2.0.9 20160727 TimPN
*/
--< Removed 2.0.34 TimPN 20170821
