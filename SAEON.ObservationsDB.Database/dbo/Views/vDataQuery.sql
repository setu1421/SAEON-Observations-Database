--> Changed 2.0.0.3 20160503 TimPN
--Renamed SensorProcedure to Sensor
--< Changed 2.0.0.3 20160503 TimPN
CREATE VIEW [dbo].[vDataQuery]
AS
SELECT     TOP (100) PERCENT NEWID() AS ID, dbo.Organisation.ID AS OrganisationID, dbo.Organisation.Name AS Organisation, 
                      dbo.Organisation.Description AS OrganisationDesc, dbo.ProjectSite.ID AS ProjectSiteID, dbo.ProjectSite.Name AS ProjectSite, 
                      dbo.ProjectSite.Description AS ProjectSiteDesc, dbo.Station.ID AS StationID, dbo.Station.Name AS Station, dbo.Station.Description AS StationDesc, 
                      dbo.Sensor.ID AS SensorID, dbo.Sensor.Name AS Sensor, dbo.Sensor.Description AS SensorDesc, 
                      dbo.Phenomenon.ID AS PhenomenonID, dbo.Phenomenon.Name AS Phenomenon, dbo.Phenomenon.Description AS PhenomenonDesc, dbo.Offering.ID AS OfferingID, 
                      dbo.Offering.Name AS Offering, dbo.Offering.Description AS OfferingDesc
FROM         dbo.Station INNER JOIN
                      dbo.Sensor ON dbo.Sensor.StationID = dbo.Station.ID INNER JOIN
                      dbo.Phenomenon ON dbo.Phenomenon.ID = dbo.Sensor.PhenomenonID INNER JOIN
                      dbo.PhenomenonOffering ON dbo.PhenomenonOffering.PhenomenonID = dbo.Phenomenon.ID INNER JOIN
                      dbo.Offering ON dbo.Offering.ID = dbo.PhenomenonOffering.OfferingID INNER JOIN
                      dbo.ProjectSite ON dbo.ProjectSite.ID = dbo.Station.ProjectSiteID INNER JOIN
                      dbo.Organisation ON dbo.Organisation.ID = dbo.ProjectSite.OrganisationID
ORDER BY Organisation, ProjectSite, Station, Sensor, Phenomenon, Offering

