EXEC sp_rename @objname = 'DataLog.PhenonmenonUOMID', @newname = 'PhenomenonUOMID', @objtype = 'COLUMN'
EXEC sp_rename @objname = 'Observation.PhenonmenonOfferingID', @newname = 'PhenomenonOfferingID', @objtype = 'COLUMN'
EXEC sp_rename @objname = 'Observation.PhenonmenonUOMID', @newname = 'PhenomenonUOMID', @objtype = 'COLUMN'
EXEC sp_rename @objname = 'Progress.PhenonmenonOfferingID', @newname = 'PhenomenonOfferingID', @objtype = 'COLUMN'
EXEC sp_rename @objname = 'SensorProcedure', @newname = 'Sensor'
EXEC sp_rename @objname = 'DataLog.SensorProcedureID', @newname = 'SensorID', @objtype = 'COLUMN'
EXEC sp_rename @objname = 'Observation.SensorProcedureID', @newname = 'SensorID', @objtype = 'COLUMN'
EXEC sp_rename @objname = 'Progress.SensorProcedureID', @newname = 'SensorID', @objtype = 'COLUMN'
drop view [vSensorProcedure]
