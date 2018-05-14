use ObservationsSACTN
Select Code SiteCode, Name SiteName from Site where Code <> Name
Select Code StationCode, Name StationName from Station where Code <> Name
Select
  Station.Code StationCode, Station.Name StationName, Instrument.Code InstrumentCode, Instrument.Name InstrumentName, CharIndex(Station.Name, Instrument.Name)
from
  Station_Instrument
  inner join Station
    on (Station_Instrument.StationID = Station.ID)
  inner join Instrument  
    on (Station_Instrument.InstrumentID = Instrument.ID)
where
  (CharIndex(Station.Name,Instrument.Name) = 0)
Select * from Instrument where Code <> Name
Select
  Instrument.Code InstrumentCode, Instrument.Name InstrumentName, Sensor.Code SensorCode, Sensor.Name SensorName, CharIndex(Instrument.Name, Sensor.Name)
from
  Instrument_Sensor
  inner join Instrument  
    on (Instrument_Sensor.InstrumentID = Instrument.ID)
  inner join Sensor
    on (Instrument_Sensor.SensorID = Sensor.ID)
where
  (CharIndex(Instrument.Name, Sensor.Name) = 0)
--Select Code SensorCode, Name SensorName from Sensor where Code <> Name

