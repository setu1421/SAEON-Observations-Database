use ObservationsElwandle
RAISERROR('Deleting Observation', 0, 1) WITH NOWAIT
Delete Observation
RAISERROR('Deleting Datalog', 0, 1) WITH NOWAIT
Delete Datalog
RAISERROR('Deleting ImportBatchSummary', 0, 1) WITH NOWAIT
Delete ImportBatchSummary
RAISERROR('Deleting ImportBatch', 0, 1) WITH NOWAIT
Delete ImportBatch
RAISERROR('Deleting Instrument_Sensor', 0, 1) WITH NOWAIT
Delete Instrument_Sensor
RAISERROR('Deleting Sensor', 0, 1) WITH NOWAIT
Delete Sensor
RAISERROR('Deleting Organisation_Instrument', 0, 1) WITH NOWAIT
Delete Organisation_Instrument
RAISERROR('Deleting Station_Instrument', 0, 1) WITH NOWAIT
Delete Station_Instrument
RAISERROR('Deleting Instrument', 0, 1) WITH NOWAIT
Delete Instrument
RAISERROR('Deleting Organisation_Station', 0, 1) WITH NOWAIT
Delete Organisation_Station
RAISERROR('Deleting Project_Station', 0, 1) WITH NOWAIT
Delete Project_Station
RAISERROR('Deleting Station', 0, 1) WITH NOWAIT
Delete Station
RAISERROR('Deleting Organisation_Site', 0, 1) WITH NOWAIT
Delete Organisation_Site
RAISERROR('Deleting Site', 0, 1) WITH NOWAIT
Delete Site
RAISERROR('Deleting Project', 0, 1) WITH NOWAIT
Delete Project
RAISERROR('Deleting Programme', 0, 1) WITH NOWAIT
Delete Programme
RAISERROR('Deleting Organisation', 0, 1) WITH NOWAIT
Delete Organisation where Code <> 'SAEON'
RAISERROR('Deleting AuditLog', 0, 1) WITH NOWAIT
Delete AuditLog
RAISERROR('Deleting DataSource', 0, 1) WITH NOWAIT
Delete DataSource
RAISERROR('Deleting SchemaColumn', 0, 1) WITH NOWAIT
Delete SchemaColumn
RAISERROR('Deleting DataSchema', 0, 1) WITH NOWAIT
Delete DataSchema
--RAISERROR('Deleting PhenomenonOffering', 0, 1) WITH NOWAIT
--Delete PhenomenonOffering
--RAISERROR('Deleting PhenomenonUOM', 0, 1) WITH NOWAIT
--Delete PhenomenonUOM
--RAISERROR('Deleting Phenomenon', 0, 1) WITH NOWAIT
--Delete Phenomenon
--RAISERROR('Deleting Offering', 0, 1) WITH NOWAIT
--Delete Offering
--RAISERROR('Deleting UnitOfMeasure', 0, 1) WITH NOWAIT
--Delete UnitOfMeasure
RAISERROR('Done', 0, 1) WITH NOWAIT
CheckPoint;

