-- Links
Select
  Station.Name StationName, Instrument.Name InstrumentName, Sensor.Name SensorName
from
  Station
  inner join Station_Instrument
    on (Station_Instrument.StationID = Station.ID) 
  inner join Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID) 
  inner join Instrument_Sensor
    on (Instrument_Sensor.InstrumentID = Instrument.ID) 
  inner join Sensor
    on (Instrument_Sensor.SensorID = Sensor.ID)
where 
  ((Station.Code like 'ELW%') and ((Instrument.Code not like 'ELW%') or (Sensor.Code not like 'ELW%'))) or
  ((Station.Code not like 'ELW%') and ((Instrument.Code like 'ELW%') or (Sensor.Code like 'ELW%'))) or
  ((Instrument.Code like 'ELW%') and ((Station.Code not like 'ELW%') or (Sensor.Code not like 'ELW%'))) or
  ((Instrument.Code not like 'ELW%') and ((Station.Code like 'ELW%') or (Sensor.Code like 'ELW%')))

-- Import Batches
Select 
  StationName, InstrumentName, DataSource.Name DataSourceName
from 
  vImportBatchSummary
  inner join ImportBatch
    on (vImportBatchSummary.ImportBatchID = ImportBatch.ID)
  inner join DataSource
    on (ImportBatch.DataSourceID = DataSource.ID)
where 
  ((StationCode like 'ELW%') and ((InstrumentCode not like 'ELW%') or (SensorCode not like 'ELW%') or (DataSource.Code not like 'ELW%'))) or
  ((StationCode not like 'ELW%') and ((InstrumentCode like 'ELW%') or (SensorCode like 'ELW%') or (DataSource.Code like 'ELW%'))) or
  ((InstrumentCode like 'ELW%') and ((StationCode not like 'ELW%') or (SensorCode not like 'ELW%') or (DataSource.Code not like 'ELW%'))) or
  ((InstrumentCode not like 'ELW%') and ((StationCode like 'ELW%') or (SensorCode like 'ELW%') or (DataSource.Code like 'ELW%')))

/*
-- Observations
Select 
  StationName, InstrumentName, DataSource.Name DataSourceName
from 
  vObservationExpansion o
  inner join ImportBatch
    on (o.ImportBatchID = ImportBatch.ID)
  inner join DataSource
    on (ImportBatch.DataSourceID = DataSource.ID)
where 
  ((StationCode like 'ELW%') and ((InstrumentCode not like 'ELW%') or (SensorCode not like 'ELW%') or (DataSource.Code not like 'ELW%'))) or
  ((StationCode not like 'ELW%') and ((InstrumentCode like 'ELW%') or (SensorCode like 'ELW%') or (DataSource.Code like 'ELW%'))) or
  ((InstrumentCode like 'ELW%') and ((StationCode not like 'ELW%') or (SensorCode not like 'ELW%') or (DataSource.Code not like 'ELW%'))) or
  ((InstrumentCode not like 'ELW%') and ((StationCode like 'ELW%') or (SensorCode like 'ELW%') or (DataSource.Code like 'ELW%')))
*/
