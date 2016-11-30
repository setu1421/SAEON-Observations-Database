--> Changed 2.0.3 20160503 TimPN
--Renamed SensorProcedure to Sensor
--< Changed 2.0.3 20160503 TimPN
CREATE VIEW [dbo].[vDataQuery]
AS
--> Removed 2.0.17 20161128 TimPN
--SELECT     TOP (100) PERCENT NEWID()ID, Organisation.IDOrganisationID, Organisation.NameOrganisation, 
--                      Organisation.DescriptionOrganisationDesc, ProjectSite.IDProjectSiteID, ProjectSite.NameProjectSite, 
--                      ProjectSite.DescriptionProjectSiteDesc, Station.IDStationID, Station.NameStation, Station.DescriptionStationDesc, 
--                      Sensor.IDSensorID, Sensor.NameSensor, Sensor.DescriptionSensorDesc, 
--                      Phenomenon.IDPhenomenonID, Phenomenon.NamePhenomenon, Phenomenon.DescriptionPhenomenonDesc, Offering.IDOfferingID, 
--                      Offering.NameOffering, Offering.DescriptionOfferingDesc
--FROM         Station INNER JOIN
--                      Sensor ON Sensor.StationID = Station.ID INNER JOIN
--                      Phenomenon ON Phenomenon.ID = Sensor.PhenomenonID INNER JOIN
--                      PhenomenonOffering ON PhenomenonOffering.PhenomenonID = Phenomenon.ID INNER JOIN
--                      Offering ON Offering.ID = PhenomenonOffering.OfferingID INNER JOIN
--                      ProjectSite ON ProjectSite.ID = Station.ProjectSiteID INNER JOIN
--                      Organisation ON Organisation.ID = ProjectSite.OrganisationID
--ORDER BY Organisation, ProjectSite, Station, Sensor, Phenomenon, Offering
--< Removed 2.0.17 20161128 TimPN
--> Added 2.0.17 20161128 TimPN
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
--< Added 2.0.17 20161128 TimPN

