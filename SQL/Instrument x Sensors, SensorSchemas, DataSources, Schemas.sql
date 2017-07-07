select
  Instrument.Name,
  (
  Select count(SensorID) from Instrument_Sensor where InstrumentID = Instrument.ID
  ) Sensors,
  (
  Select count(distinct SensorID) from Instrument_Sensor where InstrumentID = Instrument.ID
  ) DistinctSensors,
  (
  Select 
    count(DataSchemaID) 
  from 
    Instrument_Sensor 
	inner join Sensor
	  on (Instrument_Sensor.SensorID = Sensor.ID)

  where 
    (InstrumentID = Instrument.ID)
  ) SensorSchemas,
  (
  Select 
    count(Distinct DataSchemaID) 
  from 
    Instrument_Sensor 
	inner join Sensor
	  on (Instrument_Sensor.SensorID = Sensor.ID)

  where 
    (InstrumentID = Instrument.ID)
  ) DistinctSensorSchemas,
  (
  Select 
    count(Distinct DataSourceID) 
  from 
    Instrument_Sensor 
	inner join Sensor
	  on (Instrument_Sensor.SensorID = Sensor.ID)

  where 
    (InstrumentID = Instrument.ID)
  ) DataSources,
  (
  Select 
    count(Distinct DataSource.DataSchemaID) 
  from 
    Instrument_Sensor 
	inner join Sensor
	  on (Instrument_Sensor.SensorID = Sensor.ID)
	inner join DataSource
	  on (Sensor.DataSourceID = DataSource.ID)
  where 
    (InstrumentID = Instrument.ID)
  ) Schemas
from  
  Instrument
order by
  Instrument.Name