﻿--> Changed 2.0.3 20160503 TimPN
--Renamed SensorProcedure to Sensor
--< Changed 2.0.3 20160503 TimPN
CREATE VIEW [dbo].[vObservation]
AS
--> Changed 2.0.16 20161107 TimPN
--> Changed 2.0.3 20160421 TimPN
--SELECT     o.ID, o.SensorID, o.PhenonmenonOfferingID, o.PhenonmenonUOMID, o.UserId, o.RawValue, o.DataValue, o.ImportBatchID, o.ValueDate, 
--SELECT     o.ID, o.SensorID, o.PhenomenonOfferingID, o.PhenomenonUOMID, o.UserId, o.RawValue, o.DataValue, o.ImportBatchID, o.ValueDate, 
--< Changed 2.0.3 20160421 TimPN
--                      sp.Code AS spCode, sp.Description AS spDesc, sp.Name AS spName, sp.Url AS spURL, sp.DataSchemaID, sp.DataSourceID, sp.PhenomenonID, sp.StationID, 
--                      ph.Name AS phName, st.Name AS stName, ds.Name AS dsName, ISNULL(dschema.Name,dschema1.Name) AS dschemaName, offer.Name AS offName, offer.ID AS offID, ps.Name AS psName, 
--                      ps.ID AS psID, org.Name AS orgName, org.ID AS orgID, uom.Unit AS uomUnit, uom.UnitSymbol AS uomSymbol, users.UserName,
--                      o.Comment
--FROM         dbo.Observation AS o INNER JOIN
--                      dbo.Sensor AS sp ON sp.ID = o.SensorID INNER JOIN
--                      dbo.Phenomenon AS ph ON ph.ID = sp.PhenomenonID INNER JOIN
--> Changed 2.0.3 20160421 TimPN
--                      dbo.PhenomenonOffering AS phOff ON phOff.ID = o.PhenonmenonOfferingID INNER JOIN  
--                      dbo.PhenomenonOffering AS phOff ON phOff.ID = o.PhenomenonOfferingID INNER JOIN  
--< Changed 2.0.3 20160421 TimPN
--                      dbo.Offering AS offer ON offer.ID = phOff.OfferingID INNER JOIN
--> Changed 2.0.3 20160421 TimPN
--                      dbo.PhenomenonUOM AS puom ON puom.ID = o.PhenonmenonUOMID INNER JOIN
--                      dbo.PhenomenonUOM AS puom ON puom.ID = o.PhenomenonUOMID INNER JOIN
--< Changed 2.0.3 20160421 TimPN
--                      dbo.Station AS st ON st.ID = sp.StationID INNER JOIN
--                      dbo.DataSource AS ds ON ds.ID = sp.DataSourceID LEFT JOIN
--                      dbo.DataSchema AS dschema1 ON dschema1.ID = ds.DataSchemaID LEFT JOIN
--                      dbo.DataSchema AS dschema ON dschema.ID = sp.DataSchemaID INNER JOIN                   
--                      dbo.ProjectSite AS ps ON ps.ID = st.ProjectSiteID INNER JOIN
--                      dbo.Organisation AS org ON org.ID = ps.OrganisationID INNER JOIN
--                      dbo.UnitOfMeasure AS uom ON uom.ID = puom.UnitOfMeasureID INNER JOIN
--                      dbo.aspnet_Users AS users ON users.UserId = o.UserId 

SELECT 
  o.ID, o.SensorID, o.PhenomenonOfferingID, o.PhenomenonUOMID, o.UserId, o.RawValue, o.DataValue, o.ImportBatchID, o.ValueDate, o.Comment, aspnet_Users.UserName,
  Offering.Name OfferingName,
  UnitOfMeasure.Unit UnitOfMeasureUnit,
  UnitOfMeasure.UnitSymbol UnitOfMeasureSymbol,
  Sensor.Name SensorName,
  DataSource.ID DataSourceID,
  IsNull(SensorSchema.Name, DataSourceSchema.Name) DataSchemaName,
  Instrument.Name InstrumentName,
  Station.Name StationName,
  Site.Name SiteName,
  Organisation.Name OrganisationName
FROM
  Observation o
  inner join PhenomenonOffering
    on (o.PhenomenonOfferingID = PhenomenonOffering.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
  inner join PhenomenonUOM
    on (o.PhenomenonUOMID = PhenomenonUOM.ID)
  inner join UnitOfMeasure
    on (PhenomenonUOM.UnitOfMeasureID = UnitOfMeasure.ID)
  inner join Sensor 
    on (o.SensorID = Sensor.ID)
  left join DataSchema SensorSchema
    on (Sensor.DataSchemaID = SensorSchema.ID)
  inner join DataSource
    on (Sensor.DataSourceID = DataSource.ID)
  left join DataSchema DataSourceSchema
    on (DataSource.DataSchemaID = DataSourceSchema.ID)
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
  inner join Organisation_Site
    on (Organisation_Site.SiteID = Site.ID)
  inner join Organisation
    on (Organisation_Site.OrganisationID = Organisation.ID)
  inner join aspnet_Users
    on (o.UserId = aspnet_Users.UserId)
--< Changed 2.0.16 20161107 TimPN
GO
