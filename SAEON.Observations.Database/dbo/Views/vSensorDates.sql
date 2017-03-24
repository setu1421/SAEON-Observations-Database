--> Added 2.0.29 20170324 TimPN
CREATE VIEW [dbo].[vSensorDates]
AS 
Select
  Sensor.ID SensorID,
  Sensor.Name SensorName,
  Instrument_Sensor.StartDate InstrumenSensorStartDate, Instrument_Sensor.EndDate InstrumenSensorEndDate,
  Instrument.Name InstrumentName, Instrument.StartDate InstrumentStartDate, Instrument.EndDate InstrumentEndDate,
  Station_Instrument.StartDate StationInstrumentStartDate, Station_Instrument.EndDate StationInstrumentEndDate,
  Station.Name StationName, Station.StartDate StationStartDate, Station.EndDate StationEndDate,
  Site.Name SiteName, Site.StartDate SiteStartDate, Site.EndDate SiteEndDate
from
  Sensor
  left join Instrument_Sensor
    on (Instrument_Sensor.SensorID = Sensor.ID)
  left join Instrument
    on (Instrument_Sensor.InstrumentID = Instrument.ID)
  left join Station_Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID)
  left join Station
    on (Station_Instrument.StationID = Station.ID)
  left join Site
    on (Station.SiteID = Site.ID)
--< Added 2.0.29 20170324 TimPN

