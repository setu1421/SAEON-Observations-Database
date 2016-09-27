EXEC sp_rename @objname = 'DataLog.PhenonmenonUOMID', @newname = 'PhenomenonUOMID', @objtype = 'COLUMN'
GO
EXEC sp_rename @objname = 'Observation.PhenonmenonOfferingID', @newname = 'PhenomenonOfferingID', @objtype = 'COLUMN'
GO
EXEC sp_rename @objname = 'Observation.PhenonmenonUOMID', @newname = 'PhenomenonUOMID', @objtype = 'COLUMN'
GO
--EXEC sp_rename @objname = 'Progress.PhenonmenonOfferingID', @newname = 'PhenomenonOfferingID', @objtype = 'COLUMN'
GO
EXEC sp_rename @objname = 'SensorProcedure', @newname = 'Sensor'
GO
EXEC sp_rename @objname = 'DataLog.SensorProcedureID', @newname = 'SensorID', @objtype = 'COLUMN'
GO
EXEC sp_rename @objname = 'Observation.SensorProcedureID', @newname = 'SensorID', @objtype = 'COLUMN'
GO
--EXEC sp_rename @objname = 'Progress.SensorProcedureID', @newname = 'SensorID', @objtype = 'COLUMN'
GO
EXEC sp_rename @objname = 'DataSourceTransformation.SensorProcedureID', @newname = 'SensorID', @objtype = 'COLUMN'
GO
alter table Sensor alter column DataSourceID uniqueidentifier not null
GO
drop view [vSensorProcedure]
GO

