CREATE VIEW [dbo].[vObservation]
AS
--> Changed 2.0.0.3 20160421 TimPN
--SELECT     o.ID, o.SensorProcedureID, o.PhenonmenonOfferingID, o.PhenonmenonUOMID, o.UserId, o.RawValue, o.DataValue, o.ImportBatchID, o.ValueDate, 
SELECT     o.ID, o.SensorProcedureID, o.PhenomenonOfferingID, o.PhenonmenonUOMID, o.UserId, o.RawValue, o.DataValue, o.ImportBatchID, o.ValueDate, 
--< Changed 2.0.0.3 20160421 TimPN
                      sp.Code AS spCode, sp.Description AS spDesc, sp.Name AS spName, sp.Url AS spURL, sp.DataSchemaID, sp.DataSourceID, sp.PhenomenonID, sp.StationID, 
                      ph.Name AS phName, st.Name AS stName, ds.Name AS dsName, ISNULL(dschema.Name,dschema1.Name) AS dschemaName, offer.Name AS offName, offer.ID AS offID, ps.Name AS psName, 
                      ps.ID AS psID, org.Name AS orgName, org.ID AS orgID, uom.Unit AS uomUnit, uom.UnitSymbol AS uomSymbol, users.UserName,
                      o.Comment
FROM         dbo.Observation AS o INNER JOIN
                      dbo.SensorProcedure AS sp ON sp.ID = o.SensorProcedureID INNER JOIN
                      dbo.Phenomenon AS ph ON ph.ID = sp.PhenomenonID INNER JOIN
--> Changed 2.0.0.3 20160421 TimPN
--                      dbo.PhenomenonOffering AS phOff ON phOff.ID = o.PhenonmenonOfferingID INNER JOIN  
                      dbo.PhenomenonOffering AS phOff ON phOff.ID = o.PhenomenonOfferingID INNER JOIN  
--< Changed 2.0.0.3 20160421 TimPN
                      dbo.Offering AS offer ON offer.ID = phOff.OfferingID INNER JOIN
                      dbo.PhenomenonUOM AS puom ON puom.ID = o.PhenonmenonUOMID INNER JOIN
                      dbo.Station AS st ON st.ID = sp.StationID INNER JOIN
                      dbo.DataSource AS ds ON ds.ID = sp.DataSourceID LEFT JOIN
                      dbo.DataSchema AS dschema1 ON dschema1.ID = ds.DataSchemaID LEFT JOIN
                      dbo.DataSchema AS dschema ON dschema.ID = sp.DataSchemaID INNER JOIN                   
                      dbo.ProjectSite AS ps ON ps.ID = st.ProjectSiteID INNER JOIN
                      dbo.Organisation AS org ON org.ID = ps.OrganisationID INNER JOIN
                      dbo.UnitOfMeasure AS uom ON uom.ID = puom.UnitOfMeasureID INNER JOIN
                      dbo.aspnet_Users AS users ON users.UserId = o.UserId 
GO
