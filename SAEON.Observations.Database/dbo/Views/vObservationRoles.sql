--> Changed 2.0.3 20160503 TimPN
--Renamed SensorProcedure to Sensor
--< Changed 2.0.3 20160503 TimPN
CREATE VIEW [dbo].[vObservationRoles]
AS
--> Changed 2.0.16 20161107 TimPN
----> Changed 2.0.3 20160421 TimPN
----SELECT     vo.ID, vo.SensorID, vo.PhenonmenonOfferingID, vo.PhenonmenonUOMID, vo.UserId, vo.RawValue, vo.DataValue, vo.ImportBatchID, vo.ValueDate, 
--SELECT     vo.ID, vo.SensorID, vo.PhenomenonOfferingID, vo.PhenomenonUOMID, vo.UserId, vo.RawValue, vo.DataValue, vo.ImportBatchID, vo.ValueDate, 
----> Changed 2.0.3 20160421 TimPN
--                      vo.spCode, vo.spDesc, vo.spName, vo.spURL, vo.DataSchemaID, vo.DataSourceID, vo.PhenomenonID, vo.StationID, vo.phName, vo.stName, vo.dsName, 
--                      vo.dschemaName, vo.offName, vo.offID, vo.psName, vo.psID, vo.orgName, vo.orgID, vo.uomUnit, vo.uomSymbol, vo.UserName,
--                      dr.DataSourceID AS Expr2, dr.DateStart, dr.DateEnd,
--                      ur.UserId AS Expr5, vo.Comment
--FROM         dbo.vObservation AS vo 
--INNER JOIN 
--(
-- SELECT dr.DataSourceID,
--        ur.UserId,
--        MIN(dr.DateStart) DateStart,
--        MAX(dr.DateEnd) DateEnd
-- FROM DataSourceRole dr
-- INNER JOIN    dbo.aspnet_Roles AS ar ON dr.RoleId = ar.RoleId INNER JOIN
--               dbo.aspnet_UsersInRoles AS ur ON ar.RoleId = ur.RoleId
-- GROUP By dr.DataSourceID,ur.UserId
--) dr
--ON vo.DataSourceID = dr.DataSourceID 
--AND vo.ValueDate >= dr.DateStart AND vo.ValueDate <= dr.DateEnd
--INNER JOIN aspnet_Users ur
-- ON dr.UserId = ur.UserId
Select
  vObservation.*
from
  vObservation
  inner join
    (
	  Select
	    dr.DataSourceID RoleDataSourceID, aspnet_UsersInRoles.UserId RoleUserId, Min(dr.DateStart) DateStart, Max(dr.DateStart) DateEnd
	  from
	    DataSourceRole dr
		inner join aspnet_Roles 
		  on (dr.RoleId = aspnet_Roles.RoleId)
		inner join aspnet_UsersInRoles
		  on (aspnet_Roles.RoleId = aspnet_UsersInRoles.RoleId)
      group by
		dr.DataSourceID, aspnet_UsersInRoles.UserId
	) DataSourceRoles
	on (vObservation.DataSourceID = DataSourceRoles.RoleDataSourceID) and
	   (vObservation.ValueDate >= DataSourceRoles.DateStart) and (vObservation.ValueDate <= DataSourceRoles.DateEnd) and
       (DataSourceRoles.RoleUserId = vObservation.UserId)
--< Changed 2.0.16 20161107 TimPN
 
