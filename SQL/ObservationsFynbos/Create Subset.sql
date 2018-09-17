/*
Order of execution is important. Please dont change order
*/
use ObservationsFynbos
declare @List Table (Code VarChar(100))
Insert into 
  @List (Code)
Values
  ('CONSTB_CWS_W'),('ENG CED AWS'),('HSB_BAB_SM'),('HSB_BAT_SM'),('HSB_BDB_SM'),('HSB_BDT_SM'),('JNHK_DWS_W'),
  ('JNHK_LANGFOG_500'),('JNHK_LANGFOG_600'),('JNHK_LANGFOG_700'),('JNHK_LANGFOG_800')

Declare @Msg VarChar(100)
Declare @BatchSize int = 1000000
Declare @BatchNum int
Declare @Done bit
Declare @Updated int = 0
Set @BatchNum = 1
Set @Done = 0
WHILE (@Done = 0)
  BEGIN 
	Set @Msg = Convert(VarChar(20), GetDate()) + ' Observations ' + Convert(VarChar(20), @BatchNum) + ' ' + Convert(VarChar(20), @BatchNum * @BatchSize)
	RAISERROR(@msg, 0, 1) WITH NOWAIT
    BEGIN TRANSACTION 
	BEGIN TRY
	    Delete Top (1000000)
		  Observation
		from
		  Observation
		  inner join vObservationExpansion
		    on (Observation.ID = vObservationExpansion.ID)
		where
		  (vObservationExpansion.StationCode not in ((Select Code from @List)))
		Set @Updated = @@RowCount 
		IF @Updated = 0 Set @Done = 1
	END TRY
	BEGIN  CATCH
		SELECT 
			GetDate()  
			,ERROR_NUMBER() AS ErrorNumber  
			,ERROR_SEVERITY() AS ErrorSeverity  
			,ERROR_STATE() AS ErrorState  
			,ERROR_PROCEDURE() AS ErrorProcedure  
			,ERROR_LINE() AS ErrorLine  
			,ERROR_MESSAGE() AS ErrorMessage;  

		IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
	END CATCH
    IF @@TRANCOUNT > 0 COMMIT TRANSACTION 
	--CheckPoint
	Set @Msg = Convert(VarChar(20), GetDate()) + ' Observations Deleted ' + Convert(VarChar(20), @Updated) + ' ' + Convert(VarChar(20), @BatchNum) + ' ' + Convert(VarChar(20), @BatchNum * @BatchSize)
	RAISERROR(@msg, 0, 1) WITH NOWAIT
	set @BatchNum = @BatchNum + 1
	WAITFOR DELAY '00:00:30'
  END 

RAISERROR('DataLog', 0, 1) WITH NOWAIT
Delete
  DataLog
from
  DataLog
  left join vImportBatchSummary
    on (DataLog.ImportBatchID = vImportBatchSummary.ImportBatchID)
where
  (vImportBatchSummary.ID is null) or (vImportBatchSummary.StationCode not in ((Select Code from @List)))
RAISERROR('SchemaColumn from DataSource', 0, 1) WITH NOWAIT
Delete
  SchemaColumn
from
  SchemaColumn
  inner join DataSchema
    on (SchemaColumn.DataSchemaID = DataSchema.ID)
  inner join DataSource
    on (DataSource.DataSchemaID = DataSchema.ID)
  inner join Sensor
    on (Sensor.DataSourceID = DataSource.ID)
  left join vImportBatchSummary
    on (vImportBatchSummary.SensorID = Sensor.ID)
where
  (vImportBatchSummary.ID is null) or (vImportBatchSummary.StationCode not in ((Select Code from @List)))
RAISERROR('SchemaColumn from Sensor', 0, 1) WITH NOWAIT
Delete
  SchemaColumn
from
  SchemaColumn
  inner join DataSchema
    on (SchemaColumn.DataSchemaID = DataSchema.ID)
  inner join Sensor
    on (Sensor.DataSchemaID = DataSchema.ID)
  left join vImportBatchSummary
    on (vImportBatchSummary.SensorID = Sensor.ID)
where
  (vImportBatchSummary.ID is null) or (vImportBatchSummary.StationCode not in ((Select Code from @List)))
RAISERROR('DataSchema from DataSource', 0, 1) WITH NOWAIT
Update
  DataSource
set
  DataSchemaID = null
from
  DataSchema
  inner join DataSource
    on (DataSource.DataSchemaID = DataSchema.ID)
  inner join Sensor
    on (Sensor.DataSourceID = DataSource.ID)
  left join vImportBatchSummary
    on (vImportBatchSummary.SensorID = Sensor.ID)
where
  (vImportBatchSummary.ID is null) or (vImportBatchSummary.StationCode not in ((Select Code from @List)))
Delete
  DataSchema
from
  DataSchema
  inner join DataSource
    on (DataSource.DataSchemaID = DataSchema.ID)
  inner join Sensor
    on (Sensor.DataSourceID = DataSource.ID)
  left join vImportBatchSummary
    on (vImportBatchSummary.SensorID = Sensor.ID)
where
  (vImportBatchSummary.ID is null) or (vImportBatchSummary.StationCode not in ((Select Code from @List)))
RAISERROR('DataSchema from Sensor', 0, 1) WITH NOWAIT
Update
  Sensor
Set
  DataSchemaID = null
from
  DataSchema
  inner join Sensor
    on (Sensor.DataSchemaID = DataSchema.ID)
  left join vImportBatchSummary
    on (vImportBatchSummary.SensorID = Sensor.ID)
where
  (vImportBatchSummary.ID is null) or (vImportBatchSummary.StationCode not in ((Select Code from @List)))
Delete
  DataSchema
from
  DataSchema
  inner join Sensor
    on (Sensor.DataSchemaID = DataSchema.ID)
  left join vImportBatchSummary
    on (vImportBatchSummary.SensorID = Sensor.ID)
where
  (vImportBatchSummary.ID is null) or (vImportBatchSummary.StationCode not in ((Select Code from @List)))
RAISERROR('DataSchema with no columns', 0, 1) WITH NOWAIT
Update
  DataSource
set
  DataSchemaID = null
from
  DataSchema
  inner join DataSource
    on (DataSource.DataSchemaID = DataSchema.ID)
where
  not Exists(Select * from SchemaColumn where SchemaColumn.DataSchemaID = DataSchema.ID)
Delete
  DataSchema
where
  not Exists(Select * from SchemaColumn where SchemaColumn.DataSchemaID = DataSchema.ID)
RAISERROR('Instrument_Sensor', 0, 1) WITH NOWAIT
Delete
  Instrument_Sensor
from
  Instrument_Sensor
  left join Sensor
    on (Instrument_Sensor.SensorID = Sensor.ID)
  left join vImportBatchSummary
    on (vImportBatchSummary.SensorID = Sensor.ID)
where
  (vImportBatchSummary.ID is null) or (vImportBatchSummary.StationCode not in ((Select Code from @List)))
RAISERROR('DataSourceTransformation from Sensor', 0, 1) WITH NOWAIT
Delete
  DataSourceTransformation
from
  DataSourceTransformation
  left join Sensor
    on (DataSourceTransformation.SensorID = Sensor.ID)
  left join vImportBatchSummary
    on (vImportBatchSummary.SensorID = Sensor.ID)
where
  (vImportBatchSummary.ID is null) or (vImportBatchSummary.StationCode not in ((Select Code from @List)))
RAISERROR('ImportBatchSummary', 0, 1) WITH NOWAIT
Delete
  ImportBatchSummary
from
  ImportBatchSummary
  inner join vImportBatchSummary
    on (vImportBatchSummary.ImportBatchID = ImportBatchSummary.ImportBatchID)
where
  (vImportBatchSummary.StationCode not in ((Select Code from @List)))
RAISERROR('ImportBatch', 0, 1) WITH NOWAIT
Delete
  ImportBatch
from
  ImportBatch
  left join vImportBatchSummary
    on (vImportBatchSummary.ImportBatchID = ImportBatch.ID)
where
  (vImportBatchSummary.ID is null) or (vImportBatchSummary.StationCode not in ((Select Code from @List)))
RAISERROR('Sensor', 0, 1) WITH NOWAIT
Delete 
  Sensor
from
  Sensor 
  left join vImportBatchSummary
    on (vImportBatchSummary.SensorID = Sensor.ID)
where
  (vImportBatchSummary.ID is null) or (vImportBatchSummary.StationCode not in ((Select Code from @List)))
RAISERROR('DataSourceTransformation from DataSource', 0, 1) WITH NOWAIT
Delete
  DataSourceTransformation
from
  DataSourceTransformation
  inner join DataSource
    on (DataSourceTransformation.DataSourceID = DataSource.ID)
  inner join Sensor
    on (Sensor.DataSourceID = DataSource.ID)
  left join vImportBatchSummary
    on (vImportBatchSummary.SensorID = Sensor.ID)
where
  (vImportBatchSummary.ID is null) or (vImportBatchSummary.StationCode not in ((Select Code from @List)))
RAISERROR('DataSource', 0, 1) WITH NOWAIT
Delete
  DataSource
from
  DataSource
  left join ImportBatch
    on (ImportBatch.DataSourceID = DataSource.ID)
  left join vImportBatchSummary
    on (vImportBatchSummary.ImportBatchID = ImportBatch.ID)
where
  (vImportBatchSummary.ID is null) or (vImportBatchSummary.StationCode not in ((Select Code from @List)))
RAISERROR('SchemaColumn not used', 0, 1) WITH NOWAIT
Delete 
  SchemaColumn
from
  SchemaColumn
  inner join DataSchema
    on (SchemaColumn.DataSchemaID = DataSchema.ID)
where
  not exists(Select * from DataSource where DataSource.DataSchemaID = DataSchema.ID) and
  not exists(Select * from Sensor where Sensor.DataSchemaID = DataSchema.ID)
RAISERROR('DataSchema not used', 0, 1) WITH NOWAIT
Delete 
  DataSchema
where
  not exists(Select * from DataSource where DataSource.DataSchemaID = DataSchema.ID) and
  not exists(Select * from Sensor where Sensor.DataSchemaID = DataSchema.ID)
RAISERROR('Organisation_Instrument', 0, 1) WITH NOWAIT
Delete
  Organisation_Instrument
from
  Organisation_Instrument
  inner join Instrument
    on (Organisation_Instrument.InstrumentID = Instrument.ID)
  left join vImportBatchSummary
    on (vImportBatchSummary.InstrumentID = Instrument.ID)
where
  (vImportBatchSummary.ID is null) or (vImportBatchSummary.StationCode not in ((Select Code from @List)))
RAISERROR('Station_Instrument', 0, 1) WITH NOWAIT
Delete
  Station_Instrument
from
  Station_Instrument
  inner join Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID)
  left join vImportBatchSummary
    on (vImportBatchSummary.InstrumentID = Instrument.ID)
where
  (vImportBatchSummary.ID is null) or (vImportBatchSummary.StationCode not in ((Select Code from @List)))
RAISERROR('Instrument', 0, 1) WITH NOWAIT
Delete 
  Instrument
from
  Instrument 
  left join vImportBatchSummary
    on (vImportBatchSummary.InstrumentID = Instrument.ID)
where
  (vImportBatchSummary.ID is null) or (vImportBatchSummary.StationCode not in ((Select Code from @List)))
RAISERROR('Project_Station', 0, 1) WITH NOWAIT
Delete
  Project_Station
from
  Project_Station
  inner join Station
    on (Project_Station.StationID = Station.ID)
  left join vImportBatchSummary
    on (vImportBatchSummary.StationID = Station.ID)
where
  (vImportBatchSummary.ID is null) or (vImportBatchSummary.StationCode not in ((Select Code from @List)))
RAISERROR('Organisation_Station', 0, 1) WITH NOWAIT
Delete
  Organisation_Station
from
  Organisation_Station
  inner join Station
    on (Organisation_Station.StationID = Station.ID)
  left join vImportBatchSummary
    on (vImportBatchSummary.StationID = Station.ID)
where
  (vImportBatchSummary.ID is null) or (vImportBatchSummary.StationCode not in ((Select Code from @List)))
RAISERROR('Station', 0, 1) WITH NOWAIT
Delete 
  Station 
from
  Station
  left join vImportBatchSummary
    on (vImportBatchSummary.StationID = Station.ID)
where
  (vImportBatchSummary.ID is null) or (vImportBatchSummary.StationCode not in ((Select Code from @List)))
RAISERROR('Organisation_Site', 0, 1) WITH NOWAIT
Delete
  Organisation_Site
from
  Organisation_Site
  inner join Site
    on (Organisation_Site.SiteID = Site.ID)
  left join vImportBatchSummary
    on (vImportBatchSummary.SiteID = Site.ID)
where
  (vImportBatchSummary.ID is null) or (vImportBatchSummary.StationCode not in ((Select Code from @List)))
RAISERROR('Site', 0, 1) WITH NOWAIT
Delete 
  Site 
from
  Site
  left join vImportBatchSummary
    on (vImportBatchSummary.SiteID = Site.ID)
where
  (vImportBatchSummary.ID is null) or (vImportBatchSummary.StationCode not in ((Select Code from @List)))
RAISERROR('PhenomenonOffering', 0, 1) WITH NOWAIT
Delete
  PhenomenonOffering
from
  PhenomenonOffering
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
where
  (Offering.Code like 'Depth_%')
RAISERROR('Offering', 0, 1) WITH NOWAIT
Delete Offering where (Code like 'Depth_%')
RAISERROR('PhenomenonOffering', 0, 1) WITH NOWAIT
Delete
  PhenomenonOffering
from
  PhenomenonOffering
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
where
  (Offering.Code like 'Interval%')
RAISERROR('Offering', 0, 1) WITH NOWAIT
Delete Offering where (Code like 'Interval%')
Print 'Done'
