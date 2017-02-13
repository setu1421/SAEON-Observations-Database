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
--> Changed 20170213 TimPN
--        vObservation.*, DataSourceRoles.RoleUserId
      vObservation.*, DataSourceRoles.RoleUserId, DataSourceRoles.DateStart RoleStartDate, DataSourceRoles.DateEnd RoleEndDate
--< Changed 20170213 TimPN
    from
        vObservation
--> Changed 20170213 TimPN
--        inner join
        left join
--< Changed 20170213 TimPN
        (
            Select
                dsr.DataSourceID, aspnet_UsersInRoles.UserId RoleUserId, Min(dsr.DateStart) DateStart, Max(dsr.DateEnd) DateEnd
            from
                DataSourceRole dsr
            inner join aspnet_UsersInRoles
                on (dsr.RoleId = aspnet_UsersInRoles.RoleId)
            group by
                dsr.DataSourceID, aspnet_UsersInRoles.UserId
        ) DataSourceRoles
--> Changed 20170213 TimPN
        --on (vObservation.DataSourceID = DataSourceRoles.DataSourceID) and
        --   (vObservation.ValueDate >= DataSourceRoles.DateStart) and 
        --   (vObservation.ValueDate <= DataSourceRoles.DateEnd)
          on (vObservation.DataSourceID = DataSourceRoles.DataSourceID) 
        where
           ((DataSourceRoles.DateStart is null) or (vObservation.ValueDate >= DataSourceRoles.DateStart)) and 
           ((DataSourceRoles.DateEnd is null) or (vObservation.ValueDate <= DataSourceRoles.DateEnd))
--< Changed 20170213 TimPN
--< Changed 2.0.16 20161107 TimPN
