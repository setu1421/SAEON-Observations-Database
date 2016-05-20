--> Changed 2.0.0.3 20160503 TimPN
--Renamed SensorProcedure to Sensor
--< Changed 2.0.0.3 20160503 TimPN
CREATE VIEW [dbo].[vObservationRoles]
AS
--> Changed 2.0.0.3 20160421 TimPN
--SELECT     vo.ID, vo.SensorID, vo.PhenonmenonOfferingID, vo.PhenonmenonUOMID, vo.UserId, vo.RawValue, vo.DataValue, vo.ImportBatchID, vo.ValueDate, 
SELECT     vo.ID, vo.SensorID, vo.PhenomenonOfferingID, vo.PhenomenonUOMID, vo.UserId, vo.RawValue, vo.DataValue, vo.ImportBatchID, vo.ValueDate, 
--> Changed 2.0.0.3 20160421 TimPN
                      vo.spCode, vo.spDesc, vo.spName, vo.spURL, vo.DataSchemaID, vo.DataSourceID, vo.PhenomenonID, vo.StationID, vo.phName, vo.stName, vo.dsName, 
                      vo.dschemaName, vo.offName, vo.offID, vo.psName, vo.psID, vo.orgName, vo.orgID, vo.uomUnit, vo.uomSymbol, vo.UserName,
                      dr.DataSourceID AS Expr2, dr.DateStart, dr.DateEnd,
                      ur.UserId AS Expr5, vo.Comment
FROM         dbo.vObservation AS vo 
INNER JOIN 
(
 SELECT dr.DataSourceID,
        ur.UserId,
        MIN(dr.DateStart) DateStart,
        MAX(dr.DateEnd) DateEnd
 FROM DataSourceRole dr
 INNER JOIN    dbo.aspnet_Roles AS ar ON dr.RoleId = ar.RoleId INNER JOIN
               dbo.aspnet_UsersInRoles AS ur ON ar.RoleId = ur.RoleId
 GROUP By dr.DataSourceID,ur.UserId
) dr
ON vo.DataSourceID = dr.DataSourceID 
AND vo.ValueDate >= dr.DateStart AND vo.ValueDate <= dr.DateEnd
INNER JOIN aspnet_Users ur
 ON dr.UserId = ur.UserId
 
