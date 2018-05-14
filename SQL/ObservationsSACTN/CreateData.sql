-- Delete all non SACTN data
use ObservationsSACTN
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
		  inner join ImportBatch
			on (Observation.ImportBatchID = ImportBatch.ID)
		  inner join DataSource
			on (ImportBatch.DataSourceID = DataSource.ID)
		where
		  (DataSource.Name not like 'SACTN %')
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
--Delete
--  Observation
--from
--  Observation
--  inner join ImportBatch
--    on (Observation.ImportBatchID = ImportBatch.ID)
--  inner join DataSource
--    on (ImportBatch.DataSourceID = DataSource.ID)
--where
--  (DataSource.Name not like 'SACTN %')
Delete
  DataLog
from
  DataLog
  inner join ImportBatch
    on (DataLog.ImportBatchID = ImportBatch.ID)
  inner join DataSource
    on (ImportBatch.DataSourceID = DataSource.ID)
where
  (DataSource.Name not like 'SACTN %')
Delete
  ImportBatch
from
  ImportBatch
  inner join DataSource
    on (ImportBatch.DataSourceID = DataSource.ID)
where
  (DataSource.Name not like 'SACTN %')
Delete
  DataSourceTransformation
from
  DataSourceTransformation
  inner join Sensor
    on (DataSourceTransformation.SensorID = Sensor.ID)
where
  (Sensor.Name not like 'SACTN %')
Delete
  DataSourceTransformation
from
  DataSourceTransformation
  inner join DataSource
    on (DataSourceTransformation.DataSourceID = DataSource.ID)
where
  (DataSource.Name not like 'SACTN %')
Delete DataSourceRoleOld
Delete
  Instrument_Sensor
from
  Instrument_Sensor
  inner join Sensor
    on (Instrument_Sensor.SensorID = Sensor.ID)
where
  (Sensor.Name not like 'SACTN %')
Delete Sensor where (Name not like 'SACTN %')
Delete DataSource where (Name not like 'SACTN %')
Delete
  SchemaColumn
from
  SchemaColumn
  inner join DataSchema
    on (SchemaColumn.DataSchemaID = DataSchema.ID)
where
  (DataSchema.Name not like 'SACTN %')
Delete DataSchema where (Name not like 'SACTN %')
Delete
  Organisation_Instrument
from
  Organisation_Instrument
  inner join Instrument
    on (Organisation_Instrument.InstrumentID = Instrument.ID)
where
  (Instrument.Name not like 'SACTN %')
Delete
  Station_Instrument
from
  Station_Instrument
  inner join Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID)
where
  (Instrument.Name not like 'SACTN %')
Delete Instrument where (Name not like 'SACTN %')
Delete
  Project_Station
from
  Project_Station
  inner join Station
    on (Project_Station.StationID = Station.ID)
where
  (Station.Name not like 'SACTN %')
Delete
  Organisation_Station
from
  Organisation_Station
  inner join Station
    on (Organisation_Station.StationID = Station.ID)
where
  (Station.Name not like 'SACTN %')
Delete Station where (Name not like 'SACTN %')
Delete
  Organisation_Site
from
  Organisation_Site
  inner join Site
    on (Organisation_Site.SiteID = Site.ID)
where
  (Site.Name not like 'SACTN %')
Delete Site where (Name not like 'SACTN %')
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
