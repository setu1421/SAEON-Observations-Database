CREATE VIEW [dbo].[vDataQuery]
AS
SELECT Top (100) Percent    
  Site.ID SiteID, Site.Name SiteName, Site.Description SiteDesc,
  Station.ID StationID, Station.Name StationName, Station.Description StationDesc,
  Instrument.ID InstrumentID, Instrument.Name InstrumentName, Instrument.Description InstrumentDesc,
  Sensor.ID SensorID, Sensor.Name Sensor, Sensor.Description SensorDesc, 
  Phenomenon.ID PhenomenonID, Phenomenon.Name Phenomenon, Phenomenon.Description PhenomenonDesc, 
  Offering.ID OfferingID, Offering.Name Offering, Offering.Description OfferingDesc
FROM
  Sensor 
  inner join Instrument_Sensor
    on (Instrument_Sensor.SensorID = Sensor.ID) 
  inner join Instrument
    on (Instrument_Sensor.InstrumentID = Instrument.ID)
  inner join Station_Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID)
  inner join Station 
    on (Station_Instrument.StationID = Station.ID)
  inner join Site
    on (Station.SiteID = Site.ID)
  INNER JOIN Phenomenon ON Phenomenon.ID = Sensor.PhenomenonID 
  INNER JOIN PhenomenonOffering ON PhenomenonOffering.PhenomenonID = Phenomenon.ID 
  INNER JOIN Offering ON Offering.ID = PhenomenonOffering.OfferingID 
ORDER BY 
  Site.Name, Station.Name, Instrument.Name, Sensor, Phenomenon, Offering

