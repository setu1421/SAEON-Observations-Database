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
		  inner join vImportBatchSummary
			on (Observation.ImportBatchID = vImportBatchSummary.ImportBatchID)
		where
		  (vImportBatchSummary.StationCode not in ((Select Code from @List)))
		Set @Updated = @@RowCount 
		IF @Updated = 0 Set @Done = 1
	END TRY
	BEGIN  CATCH
		SELECT   
			ERROR_NUMBER() AS ErrorNumber  
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

Delete
  DataLog
from
  DataLog
  inner join vImportBatchSummary
    on (DataLog.ImportBatchID = vImportBatchSummary.ImportBatchID)
where
  (vImportBatchSummary.StationCode not in ((Select Code from @List)))
Delete
  ImportBatchSummary
from
  ImportBatchSummary
  inner join vImportBatchSummary
    on (ImportBatchSummary.ID = vImportBatchSummary.ID)
where
  (vImportBatchSummary.StationCode not in ((Select Code from @List)))
Delete
  ImportBatch
from
  ImportBatch
  inner join DataSource
    on (ImportBatch.DataSourceID = DataSource.ID)
  inner join Sensor
    on (Sensor.DataSourceID = DataSource.ID)
  inner join vImportBatchSummary
    on (vImportBatchSummary.SensorID = Sensor.ID)
where
  (vImportBatchSummary.StationCode not in ((Select Code from @List)))
Delete
  DataSourceTransformation
from
  DataSourceTransformation
  inner join Sensor
    on (DataSourceTransformation.SensorID = Sensor.ID)
  inner join vImportBatchSummary
    on (vImportBatchSummary.SensorID = Sensor.ID)
where
  (vImportBatchSummary.StationCode not in ((Select Code from @List)))
Delete
  DataSourceTransformation
from
  DataSourceTransformation
  inner join DataSource
    on (DataSourceTransformation.DataSourceID = DataSource.ID)
  inner join Sensor
    on (Sensor.DataSourceID = DataSource.ID)
  inner join vImportBatchSummary
    on (vImportBatchSummary.SensorID = Sensor.ID)
where
  (vImportBatchSummary.StationCode not in ((Select Code from @List)))
Delete
  Instrument_Sensor
from
  Instrument_Sensor
  inner join Sensor
    on (Instrument_Sensor.SensorID = Sensor.ID)
  inner join vImportBatchSummary
    on (vImportBatchSummary.SensorID = Sensor.ID)
where
  (vImportBatchSummary.StationCode not in ((Select Code from @List)))
Delete 
  Sensor
from
  Sensor 
  inner join vImportBatchSummary
    on (vImportBatchSummary.SensorID = Sensor.ID)
where
  (vImportBatchSummary.StationCode not in ((Select Code from @List)))
-- DataSources not used in ImportBatch nor Sensor
Delete
  DataSource
from
  DataSource
  left join ImportBatch
    on (ImportBatch.DataSourceID = DataSource.ID)
  left join Sensor
    on (Sensor.DataSourceID = DataSource.ID)
where
  (ImportBatch.ID is null) and
  (Sensor.ID is null)
-- DataSchemas not uses in DataSource nor Sensor
Delete
  DataSchema
from
  DataSchema
  left join DataSource
    on (DataSource.DataSchemaID = DataSchema.ID)
  left join Sensor
    on (Sensor.DataSchemaID = DataSchema.ID)
where
  (DataSource.ID is null) and
  (Sensor.ID is null)
Delete
  Organisation_Instrument
from
  Organisation_Instrument
  inner join Instrument
    on (Organisation_Instrument.InstrumentID = Instrument.ID)
  inner join vImportBatchSummary
    on (vImportBatchSummary.InstrumentID = Instrument.ID)
where
  (vImportBatchSummary.StationCode not in ((Select Code from @List)))
Delete
  Station_Instrument
from
  Station_Instrument
  inner join Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID)
  inner join vImportBatchSummary
    on (vImportBatchSummary.InstrumentID = Instrument.ID)
where
  (vImportBatchSummary.StationCode not in ((Select Code from @List)))
Delete 
  Instrument
from
  Instrument 
  inner join vImportBatchSummary
    on (vImportBatchSummary.InstrumentID = Instrument.ID)
where
  (vImportBatchSummary.StationCode not in ((Select Code from @List)))
Delete
  Project_Station
from
  Project_Station
  inner join Station
    on (Project_Station.StationID = Station.ID)
  inner join vImportBatchSummary
    on (vImportBatchSummary.StationID = Station.ID)
where
  (vImportBatchSummary.StationCode not in ((Select Code from @List)))
Delete
  Organisation_Station
from
  Organisation_Station
  inner join Station
    on (Organisation_Station.StationID = Station.ID)
  inner join vImportBatchSummary
    on (vImportBatchSummary.StationID = Station.ID)
where
  (vImportBatchSummary.StationCode not in ((Select Code from @List)))
Delete 
  Station 
from
  Station
  inner join vImportBatchSummary
    on (vImportBatchSummary.StationID = Station.ID)
where
  (vImportBatchSummary.StationCode not in ((Select Code from @List)))
Delete
  Organisation_Site
from
  Organisation_Site
  inner join Site
    on (Organisation_Site.SiteID = Site.ID)
  inner join vImportBatchSummary
    on (vImportBatchSummary.SiteID = Site.ID)
where
  (vImportBatchSummary.StationCode not in ((Select Code from @List)))
Delete 
  Site 
from
  Site
  inner join vImportBatchSummary
    on (vImportBatchSummary.SiteID = Site.ID)
where
  (vImportBatchSummary.StationCode not in ((Select Code from @List)))
Delete
  PhenomenonOffering
from
  PhenomenonOffering
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
where
  (Offering.Code like 'Depth_%')
Delete Offering where (Code like 'Depth_%')
Delete
  PhenomenonOffering
from
  PhenomenonOffering
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
where
  (Offering.Code like 'Interval%')
Delete Offering where (Code like 'Interval%')

