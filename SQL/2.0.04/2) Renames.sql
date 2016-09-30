EXEC sp_rename @objname = 'DataLog.PhenonmenonUOMID', @newname = 'PhenomenonUOMID', @objtype = 'COLUMN'
EXEC sp_rename @objname = 'Observation.PhenonmenonOfferingID', @newname = 'PhenomenonOfferingID', @objtype = 'COLUMN'
EXEC sp_rename @objname = 'Observation.PhenonmenonUOMID', @newname = 'PhenomenonUOMID', @objtype = 'COLUMN'
EXEC sp_rename @objname = 'Progress.PhenonmenonOfferingID', @newname = 'PhenomenonOfferingID', @objtype = 'COLUMN'
EXEC sp_rename @objname = 'SensorProcedure', @newname = 'Sensor'
EXEC sp_rename @objname = 'DataLog.SensorProcedureID', @newname = 'SensorID', @objtype = 'COLUMN'
EXEC sp_rename @objname = 'Observation.SensorProcedureID', @newname = 'SensorID', @objtype = 'COLUMN'
EXEC sp_rename @objname = 'Progress.SensorProcedureID', @newname = 'SensorID', @objtype = 'COLUMN'
--drop view [vSensorProcedure]
EXEC sp_rename @objname = 'DataSourceTransformation.SensorProcedureID', @newname = 'SensorID', @objtype = 'COLUMN'

drop index Sensor.ix_sensor_datasourceid
alter table Sensor alter column DataSourceID uniqueidentifier not null
drop table DataSource_Organisation
alter table DataSource drop constraint FK_DataSource_StationID
alter table DataSource drop column StationID
