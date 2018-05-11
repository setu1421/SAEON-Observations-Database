use ObservationsSACTN
Select 
  (Select count(*) from Site where Name like 'SACTN %') Sites,
  (Select count(*) from Station where Name like 'SACTN %') Stations,
  (Select count(*) from Instrument where Name like 'SACTN %') Instruments,
  (
  Select 
    count(*)
  from
    Station_Instrument
    inner join Station
      on (Station_Instrument.StationID = Station.ID)
    inner join Instrument
      on (Station_Instrument.InstrumentID = Instrument.ID)
    where 
      (Station.Name like 'SACTN %') and (Instrument.Name like 'SACTN %')
  ) StationsInstruments,
  (Select count(*) from Sensor where Name like 'SACTN %') Sensors,
  (
  Select 
    count(*)
  from
    Instrument_Sensor
    inner join Instrument
      on (Instrument_Sensor.InstrumentID = Instrument.ID)
    inner join Sensor
      on (Instrument_Sensor.SensorID = Sensor.ID)
    where 
      (Instrument.Name like 'SACTN %') and (Sensor.Name like 'SACTN %') 
  ) InstrumentsSensors,
  (Select count(*) from DataSource where Name like 'SACTN %') DataSources,
  (
  Select
    count(*)
  from
    Sensor
    inner join DataSource
      on (Sensor.DataSourceID = DataSource.ID)
    where 
      (Sensor.Name like 'SACTN %') and (DataSource.Name like 'SACTN %') 
  ) SensorsDataSources,
  (
  Select
    Count(Distinct ImportBatch.ID)
  from
    ImportBatch
    inner join DataSource
      on (ImportBatch.DataSourceID = DataSource.ID)
  where
    (DataSource.Name like 'SACTN %')
  ) ImportBatches,
  (
  Select
    Count(*)
  from
    Observation
    inner join ImportBatch
      on (Observation.ImportBatchID = ImportBatch.ID)
    inner join DataSource
      on (ImportBatch.DataSourceID = DataSource.ID)
  where
    (DataSource.Name like 'SACTN %')
  ) Observations,
  (Select count(*) from Sensor where (Name like 'SACTN %') and (Name like '% Annual %')) AnnualSensors,
  (
  Select 
    count(*)
  from
    Instrument_Sensor
    inner join Instrument
      on (Instrument_Sensor.InstrumentID = Instrument.ID)
    inner join Sensor
      on (Instrument_Sensor.SensorID = Sensor.ID)
    where 
      (Instrument.Name like 'SACTN %') and (Sensor.Name like 'SACTN %') and (Sensor.Name like '% Annual %')
  ) AnnualInstrumentsSensors,
  (Select count(*) from DataSource where Name like 'SACTN %' and (Name like '% Annual %')) AnnualDataSources,
  (
  Select
    count(*)
  from
    Sensor
    inner join DataSource
      on (Sensor.DataSourceID = DataSource.ID)
    where 
      ((Sensor.Name like 'SACTN %') and (Sensor.Name like '% Annual %')) and
      ((DataSource.Name like 'SACTN %') and (DataSource.Name like '% Annual %'))
  ) AnnualSensorsDataSources,
  (
  Select
    Count(Distinct ImportBatch.ID)
  from
    ImportBatch
    inner join DataSource
      on (ImportBatch.DataSourceID = DataSource.ID)
  where
    (DataSource.Name like 'SACTN %') and (DataSource.Name like '% Annual %')
  ) AnnualImportBatches,
  (
  Select
    Count(*)
  from
    Observation
    inner join ImportBatch
      on (Observation.ImportBatchID = ImportBatch.ID)
    inner join DataSource
      on (ImportBatch.DataSourceID = DataSource.ID)
  where
    (DataSource.Name like 'SACTN %') and
    (DataSource.Name like '% Annual %')
  ) AnnualObservations,
  (Select count(*) from Sensor where (Name like 'SACTN %') and (Name like '% Monthly %')) MonthlySensors,
  (
  Select 
    count(*)
  from
    Instrument_Sensor
    inner join Instrument
      on (Instrument_Sensor.InstrumentID = Instrument.ID)
    inner join Sensor
      on (Instrument_Sensor.SensorID = Sensor.ID)
    where 
      (Instrument.Name like 'SACTN %') and (Sensor.Name like 'SACTN %') and (Sensor.Name like '% Monthly %')
  ) MonthlyInstrumentsSensors,
  (Select count(*) from DataSource where Name like 'SACTN %' and (Name like '% Monthly %')) MonthlyDataSources,
  (
  Select
    count(*)
  from
    Sensor
    inner join DataSource
      on (Sensor.DataSourceID = DataSource.ID)
    where 
      ((Sensor.Name like 'SACTN %') and (Sensor.Name like '% Monthly %')) and
      ((DataSource.Name like 'SACTN %') and (DataSource.Name like '% Monthly %'))
  ) MonthlySensorsDataSources,
  (
  Select
    Count(Distinct ImportBatch.ID)
  from
    ImportBatch
    inner join DataSource
      on (ImportBatch.DataSourceID = DataSource.ID)
  where
    (DataSource.Name like 'SACTN %') and (DataSource.Name like '% Monthly %')
  ) MonthlyImportBatches,
  (
  Select
    Count(*)
  from
    Observation
    inner join ImportBatch
      on (Observation.ImportBatchID = ImportBatch.ID)
    inner join DataSource
      on (ImportBatch.DataSourceID = DataSource.ID)
  where
    (DataSource.Name like 'SACTN %') and (DataSource.Name like '% Monthly %')
  ) MonthlyObservations,
  (Select count(*) from Sensor where (Name like 'SACTN %') and (Name like '% Daily %')) DailySensors,
  (
  Select 
    count(*)
  from
    Instrument_Sensor
    inner join Instrument
      on (Instrument_Sensor.InstrumentID = Instrument.ID)
    inner join Sensor
      on (Instrument_Sensor.SensorID = Sensor.ID)
    where 
      (Instrument.Name like 'SACTN %') and (Sensor.Name like 'SACTN %') and (Sensor.Name like '% Daily %')
  ) DailyInstrumentsSensors,
  (Select count(*) from DataSource where Name like 'SACTN %' and (Name like '% Daily %')) DailyDataSources,
  (
  Select
    count(*)
  from
    Sensor
    inner join DataSource
      on (Sensor.DataSourceID = DataSource.ID)
    where 
      ((Sensor.Name like 'SACTN %') and (Sensor.Name like '% Daily %')) and
      ((DataSource.Name like 'SACTN %') and (DataSource.Name like '% Daily %'))
  ) DailySensorsDataSources,
  (
  Select
    Count(Distinct ImportBatch.ID)
  from
    ImportBatch
    inner join DataSource
      on (ImportBatch.DataSourceID = DataSource.ID)
  where
    (DataSource.Name like 'SACTN %') and (DataSource.Name like '% Daily %')
  ) DailyImportBatches,
  (
  Select
    Count(*)
  from
    Observation
    inner join ImportBatch
      on (Observation.ImportBatchID = ImportBatch.ID)
    inner join DataSource
      on (ImportBatch.DataSourceID = DataSource.ID)
  where
    (DataSource.Name like 'SACTN %') and (DataSource.Name like '% Daily %')
  ) DailyObservations,
  (Select count(*) from Sensor where (Name like 'SACTN %') and (Name like '% Hourly %')) HourlySensors,
  (
  Select 
    count(*)
  from
    Instrument_Sensor
    inner join Instrument
      on (Instrument_Sensor.InstrumentID = Instrument.ID)
    inner join Sensor
      on (Instrument_Sensor.SensorID = Sensor.ID)
    where 
      (Instrument.Name like 'SACTN %') and (Sensor.Name like 'SACTN %') and (Sensor.Name like '% Hourly %')
  ) HourlyInstrumentsSensors,
  (Select count(*) from DataSource where Name like 'SACTN %' and (Name like '% Hourly %')) HourlyDataSources,
  (
  Select
    count(*)
  from
    Sensor
    inner join DataSource
      on (Sensor.DataSourceID = DataSource.ID)
    where 
      ((Sensor.Name like 'SACTN %') and (Sensor.Name like '% Hourly %')) and
      ((DataSource.Name like 'SACTN %') and (DataSource.Name like '% Hourly %'))
  ) HourlySensorsDataSources,
  (
  Select
    Count(Distinct ImportBatch.ID)
  from
    ImportBatch
    inner join DataSource
      on (ImportBatch.DataSourceID = DataSource.ID)
  where
    (DataSource.Name like 'SACTN %') and (DataSource.Name like '% Hourly %')
  ) HourlyImportBatches,
  (
  Select
    Count(*)
  from
    Observation
    inner join ImportBatch
      on (Observation.ImportBatchID = ImportBatch.ID)
    inner join DataSource
      on (ImportBatch.DataSourceID = DataSource.ID)
  where
    (DataSource.Name like 'SACTN %') and (DataSource.Name like '% Hourly %')
  ) HourlyObservations