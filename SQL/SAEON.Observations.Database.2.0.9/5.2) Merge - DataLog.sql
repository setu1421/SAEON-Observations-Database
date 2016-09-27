With Live
as
(
Select
  DataLog.*,
  ImportBatch__DataSource.Code ImportBatch__DataSource__Code,
  ImportBatch.ImportDate ImportBatch__ImportDate,
  ImportBatch.FileName ImportBatch__FileName,
  Sensor.Code Sensor__Code,
  PhenomenonOffering__Phenomenon.Code PhenomenonOffering__Phenomenon__Code,
  PhenomenonOffering__Offering.Code PhenomenonOffering__Offering__Code,
  PhenomenonUOM__Phenomenon.Code PhenomenonUOM__Phenomenon__Code,
  PhenomenonUOM__UnitOfMeasure.Code PhenomenonUOM__UnitOfMeasure__Code,
  DataSourceTransformation__DataSource.Code DataSourceTransformation__DataSource__Code,
  DataSourceTransformation__Sensor.Code DataSourceTransformation__Sensor__Code,
  DataSourceTransformation.Rank DataSourceTransformation__Rank,
  DataSourceTransformation__TransformationType.Code DataSourceTransformation__TransformationType__Code,
  DataSourceTransformation__Phenomenon.Code DataSourceTransformation__Phenomenon__Code,
  DataSourceTransformation__PhenomenonOffering__Phenomenon.Code DataSourceTransformation__PhenomenonOffering__Phenomenon__Code,
  DataSourceTransformation__PhenomenonOffering__Offering.Code DataSourceTransformation__PhenomenonOffering__Offering__Code,
  DataSourceTransformation__PhenomenonUOM__Phenomenon.Code DataSourceTransformation__PhenomenonUOM__Phenomenon__Code,
  DataSourceTransformation__PhenomenonUOM__UnitOfMeasure.Code DataSourceTransformation__PhenomenonUOM__UnitOfMeasure__Code,
  DataSourceTransformation__NewPhenomenonOffering__Phenomenon.Code DataSourceTransformation__NewPhenomenonOffering__Phenomenon__Code,
  DataSourceTransformation__NewPhenomenonOffering__Offering.Code DataSourceTransformation__NewPhenomenonOffering__Offering__Code,
  DataSourceTransformation__NewPhenomenonUOM__Phenomenon.Code DataSourceTransformation__NewPhenomenonUOM__Phenomenon__Code,
  DataSourceTransformation__NewPhenomenonUOM__UnitOfMeasure.Code DataSourceTransformation__NewPhenomenonUOM__UnitOfMeasure__Code,
  DataSourceTransformation.StartDate DataSourceTransformation__StartDate,
  DataSourceTransformation.EndDate DataSourceTransformation__EndDate,
  Status.Code Status__Code,
  StatusReason.Code StatusReason__Code,
  aspnet_Users.UserName aspnet_Users__UserName
from
  ObservationDB.dbo.DataLog
  left join ObservationDB.dbo.ImportBatch /*1*/
    on (DataLog.ImportBatchID = ImportBatch.ID)
  left join ObservationDB.dbo.DataSource ImportBatch__DataSource /*3*/
    on (ImportBatch.DataSourceID = ImportBatch__DataSource.ID)
  left join ObservationDB.dbo.Sensor /*1*/
    on (DataLog.SensorID = Sensor.ID)
  left join ObservationDB.dbo.PhenomenonOffering /*1*/
    on (DataLog.PhenomenonOfferingID = PhenomenonOffering.ID)
  left join ObservationDB.dbo.Phenomenon PhenomenonOffering__Phenomenon /*3*/
    on (PhenomenonOffering.PhenomenonID = PhenomenonOffering__Phenomenon.ID)
  left join ObservationDB.dbo.Offering PhenomenonOffering__Offering /*3*/
    on (PhenomenonOffering.OfferingID = PhenomenonOffering__Offering.ID)
  left join ObservationDB.dbo.PhenomenonUOM /*1*/
    on (DataLog.PhenomenonUOMID = PhenomenonUOM.ID)
  left join ObservationDB.dbo.Phenomenon PhenomenonUOM__Phenomenon /*3*/
    on (PhenomenonUOM.PhenomenonID = PhenomenonUOM__Phenomenon.ID)
  left join ObservationDB.dbo.UnitOfMeasure PhenomenonUOM__UnitOfMeasure /*3*/
    on (PhenomenonUOM.UnitOfMeasureID = PhenomenonUOM__UnitOfMeasure.ID)
  left join ObservationDB.dbo.DataSourceTransformation /*1*/
    on (DataLog.DataSourceTransformationID = DataSourceTransformation.ID)
  left join ObservationDB.dbo.DataSource DataSourceTransformation__DataSource /*3*/
    on (DataSourceTransformation.DataSourceID = DataSourceTransformation__DataSource.ID)
  left join ObservationDB.dbo.Sensor DataSourceTransformation__Sensor /*3*/
    on (DataSourceTransformation.SensorID = DataSourceTransformation__Sensor.ID)
  left join ObservationDB.dbo.TransformationType DataSourceTransformation__TransformationType /*3*/
    on (DataSourceTransformation.TransformationTypeID = DataSourceTransformation__TransformationType.ID)
  left join ObservationDB.dbo.Phenomenon DataSourceTransformation__Phenomenon /*3*/
    on (DataSourceTransformation.PhenomenonID = DataSourceTransformation__Phenomenon.ID)
  left join ObservationDB.dbo.PhenomenonOffering DataSourceTransformation__PhenomenonOffering /*3*/
    on (DataSourceTransformation.PhenomenonOfferingID = DataSourceTransformation__PhenomenonOffering.ID)
  left join ObservationDB.dbo.Phenomenon DataSourceTransformation__PhenomenonOffering__Phenomenon /*3*/
    on (DataSourceTransformation__PhenomenonOffering.PhenomenonID = DataSourceTransformation__PhenomenonOffering__Phenomenon.ID)
  left join ObservationDB.dbo.Offering DataSourceTransformation__PhenomenonOffering__Offering /*3*/
    on (DataSourceTransformation__PhenomenonOffering.OfferingID = DataSourceTransformation__PhenomenonOffering__Offering.ID)
  left join ObservationDB.dbo.PhenomenonUOM DataSourceTransformation__PhenomenonUOM /*3*/
    on (DataSourceTransformation.PhenomenonUOMID = DataSourceTransformation__PhenomenonUOM.ID)
  left join ObservationDB.dbo.Phenomenon DataSourceTransformation__PhenomenonUOM__Phenomenon /*3*/
    on (DataSourceTransformation__PhenomenonUOM.PhenomenonID = DataSourceTransformation__PhenomenonUOM__Phenomenon.ID)
  left join ObservationDB.dbo.UnitOfMeasure DataSourceTransformation__PhenomenonUOM__UnitOfMeasure /*3*/
    on (DataSourceTransformation__PhenomenonUOM.UnitOfMeasureID = DataSourceTransformation__PhenomenonUOM__UnitOfMeasure.ID)
  left join ObservationDB.dbo.PhenomenonOffering DataSourceTransformation__NewPhenomenonOffering /*3*/
    on (DataSourceTransformation.NewPhenomenonOfferingID = DataSourceTransformation__NewPhenomenonOffering.ID)
  left join ObservationDB.dbo.Phenomenon DataSourceTransformation__NewPhenomenonOffering__Phenomenon /*3*/
    on (DataSourceTransformation__NewPhenomenonOffering.PhenomenonID = DataSourceTransformation__NewPhenomenonOffering__Phenomenon.ID)
  left join ObservationDB.dbo.Offering DataSourceTransformation__NewPhenomenonOffering__Offering /*3*/
    on (DataSourceTransformation__NewPhenomenonOffering.OfferingID = DataSourceTransformation__NewPhenomenonOffering__Offering.ID)
  left join ObservationDB.dbo.PhenomenonUOM DataSourceTransformation__NewPhenomenonUOM /*3*/
    on (DataSourceTransformation.NewPhenomenonUOMID = DataSourceTransformation__NewPhenomenonUOM.ID)
  left join ObservationDB.dbo.Phenomenon DataSourceTransformation__NewPhenomenonUOM__Phenomenon /*3*/
    on (DataSourceTransformation__NewPhenomenonUOM.PhenomenonID = DataSourceTransformation__NewPhenomenonUOM__Phenomenon.ID)
  left join ObservationDB.dbo.UnitOfMeasure DataSourceTransformation__NewPhenomenonUOM__UnitOfMeasure /*3*/
    on (DataSourceTransformation__NewPhenomenonUOM.UnitOfMeasureID = DataSourceTransformation__NewPhenomenonUOM__UnitOfMeasure.ID)
  left join ObservationDB.dbo.Status /*1*/
    on (DataLog.StatusID = Status.ID)
  left join ObservationDB.dbo.StatusReason /*1*/
    on (DataLog.StatusReasonID = StatusReason.ID)
  left join ObservationDB.dbo.aspnet_Users /*1*/
    on (DataLog.UserId = aspnet_Users.UserId)
),
Staging
as
(
Select
  DataLog.*,
  ImportBatch__DataSource.Code ImportBatch__DataSource__Code,
  ImportBatch.ImportDate ImportBatch__ImportDate,
  ImportBatch.FileName ImportBatch__FileName,
  Sensor.Code Sensor__Code,
  PhenomenonOffering__Phenomenon.Code PhenomenonOffering__Phenomenon__Code,
  PhenomenonOffering__Offering.Code PhenomenonOffering__Offering__Code,
  PhenomenonUOM__Phenomenon.Code PhenomenonUOM__Phenomenon__Code,
  PhenomenonUOM__UnitOfMeasure.Code PhenomenonUOM__UnitOfMeasure__Code,
  DataSourceTransformation__DataSource.Code DataSourceTransformation__DataSource__Code,
  DataSourceTransformation__Sensor.Code DataSourceTransformation__Sensor__Code,
  DataSourceTransformation.Rank DataSourceTransformation__Rank,
  DataSourceTransformation__TransformationType.Code DataSourceTransformation__TransformationType__Code,
  DataSourceTransformation__Phenomenon.Code DataSourceTransformation__Phenomenon__Code,
  DataSourceTransformation__PhenomenonOffering__Phenomenon.Code DataSourceTransformation__PhenomenonOffering__Phenomenon__Code,
  DataSourceTransformation__PhenomenonOffering__Offering.Code DataSourceTransformation__PhenomenonOffering__Offering__Code,
  DataSourceTransformation__PhenomenonUOM__Phenomenon.Code DataSourceTransformation__PhenomenonUOM__Phenomenon__Code,
  DataSourceTransformation__PhenomenonUOM__UnitOfMeasure.Code DataSourceTransformation__PhenomenonUOM__UnitOfMeasure__Code,
  DataSourceTransformation__NewPhenomenonOffering__Phenomenon.Code DataSourceTransformation__NewPhenomenonOffering__Phenomenon__Code,
  DataSourceTransformation__NewPhenomenonOffering__Offering.Code DataSourceTransformation__NewPhenomenonOffering__Offering__Code,
  DataSourceTransformation__NewPhenomenonUOM__Phenomenon.Code DataSourceTransformation__NewPhenomenonUOM__Phenomenon__Code,
  DataSourceTransformation__NewPhenomenonUOM__UnitOfMeasure.Code DataSourceTransformation__NewPhenomenonUOM__UnitOfMeasure__Code,
  DataSourceTransformation.StartDate DataSourceTransformation__StartDate,
  DataSourceTransformation.EndDate DataSourceTransformation__EndDate,
  Status.Code Status__Code,
  StatusReason.Code StatusReason__Code,
  aspnet_Users.UserName aspnet_Users__UserName
from
  ObservationDB_IMP.dbo.DataLog
  left join ObservationDB_IMP.dbo.ImportBatch /*1*/
    on (DataLog.ImportBatchID = ImportBatch.ID)
  left join ObservationDB_IMP.dbo.DataSource ImportBatch__DataSource /*3*/
    on (ImportBatch.DataSourceID = ImportBatch__DataSource.ID)
  left join ObservationDB_IMP.dbo.Sensor /*1*/
    on (DataLog.SensorID = Sensor.ID)
  left join ObservationDB_IMP.dbo.PhenomenonOffering /*1*/
    on (DataLog.PhenomenonOfferingID = PhenomenonOffering.ID)
  left join ObservationDB_IMP.dbo.Phenomenon PhenomenonOffering__Phenomenon /*3*/
    on (PhenomenonOffering.PhenomenonID = PhenomenonOffering__Phenomenon.ID)
  left join ObservationDB_IMP.dbo.Offering PhenomenonOffering__Offering /*3*/
    on (PhenomenonOffering.OfferingID = PhenomenonOffering__Offering.ID)
  left join ObservationDB_IMP.dbo.PhenomenonUOM /*1*/
    on (DataLog.PhenomenonUOMID = PhenomenonUOM.ID)
  left join ObservationDB_IMP.dbo.Phenomenon PhenomenonUOM__Phenomenon /*3*/
    on (PhenomenonUOM.PhenomenonID = PhenomenonUOM__Phenomenon.ID)
  left join ObservationDB_IMP.dbo.UnitOfMeasure PhenomenonUOM__UnitOfMeasure /*3*/
    on (PhenomenonUOM.UnitOfMeasureID = PhenomenonUOM__UnitOfMeasure.ID)
  left join ObservationDB_IMP.dbo.DataSourceTransformation /*1*/
    on (DataLog.DataSourceTransformationID = DataSourceTransformation.ID)
  left join ObservationDB_IMP.dbo.DataSource DataSourceTransformation__DataSource /*3*/
    on (DataSourceTransformation.DataSourceID = DataSourceTransformation__DataSource.ID)
  left join ObservationDB_IMP.dbo.Sensor DataSourceTransformation__Sensor /*3*/
    on (DataSourceTransformation.SensorID = DataSourceTransformation__Sensor.ID)
  left join ObservationDB_IMP.dbo.TransformationType DataSourceTransformation__TransformationType /*3*/
    on (DataSourceTransformation.TransformationTypeID = DataSourceTransformation__TransformationType.ID)
  left join ObservationDB_IMP.dbo.Phenomenon DataSourceTransformation__Phenomenon /*3*/
    on (DataSourceTransformation.PhenomenonID = DataSourceTransformation__Phenomenon.ID)
  left join ObservationDB_IMP.dbo.PhenomenonOffering DataSourceTransformation__PhenomenonOffering /*3*/
    on (DataSourceTransformation.PhenomenonOfferingID = DataSourceTransformation__PhenomenonOffering.ID)
  left join ObservationDB_IMP.dbo.Phenomenon DataSourceTransformation__PhenomenonOffering__Phenomenon /*3*/
    on (DataSourceTransformation__PhenomenonOffering.PhenomenonID = DataSourceTransformation__PhenomenonOffering__Phenomenon.ID)
  left join ObservationDB_IMP.dbo.Offering DataSourceTransformation__PhenomenonOffering__Offering /*3*/
    on (DataSourceTransformation__PhenomenonOffering.OfferingID = DataSourceTransformation__PhenomenonOffering__Offering.ID)
  left join ObservationDB_IMP.dbo.PhenomenonUOM DataSourceTransformation__PhenomenonUOM /*3*/
    on (DataSourceTransformation.PhenomenonUOMID = DataSourceTransformation__PhenomenonUOM.ID)
  left join ObservationDB_IMP.dbo.Phenomenon DataSourceTransformation__PhenomenonUOM__Phenomenon /*3*/
    on (DataSourceTransformation__PhenomenonUOM.PhenomenonID = DataSourceTransformation__PhenomenonUOM__Phenomenon.ID)
  left join ObservationDB_IMP.dbo.UnitOfMeasure DataSourceTransformation__PhenomenonUOM__UnitOfMeasure /*3*/
    on (DataSourceTransformation__PhenomenonUOM.UnitOfMeasureID = DataSourceTransformation__PhenomenonUOM__UnitOfMeasure.ID)
  left join ObservationDB_IMP.dbo.PhenomenonOffering DataSourceTransformation__NewPhenomenonOffering /*3*/
    on (DataSourceTransformation.NewPhenomenonOfferingID = DataSourceTransformation__NewPhenomenonOffering.ID)
  left join ObservationDB_IMP.dbo.Phenomenon DataSourceTransformation__NewPhenomenonOffering__Phenomenon /*3*/
    on (DataSourceTransformation__NewPhenomenonOffering.PhenomenonID = DataSourceTransformation__NewPhenomenonOffering__Phenomenon.ID)
  left join ObservationDB_IMP.dbo.Offering DataSourceTransformation__NewPhenomenonOffering__Offering /*3*/
    on (DataSourceTransformation__NewPhenomenonOffering.OfferingID = DataSourceTransformation__NewPhenomenonOffering__Offering.ID)
  left join ObservationDB_IMP.dbo.PhenomenonUOM DataSourceTransformation__NewPhenomenonUOM /*3*/
    on (DataSourceTransformation.NewPhenomenonUOMID = DataSourceTransformation__NewPhenomenonUOM.ID)
  left join ObservationDB_IMP.dbo.Phenomenon DataSourceTransformation__NewPhenomenonUOM__Phenomenon /*3*/
    on (DataSourceTransformation__NewPhenomenonUOM.PhenomenonID = DataSourceTransformation__NewPhenomenonUOM__Phenomenon.ID)
  left join ObservationDB_IMP.dbo.UnitOfMeasure DataSourceTransformation__NewPhenomenonUOM__UnitOfMeasure /*3*/
    on (DataSourceTransformation__NewPhenomenonUOM.UnitOfMeasureID = DataSourceTransformation__NewPhenomenonUOM__UnitOfMeasure.ID)
  left join ObservationDB_IMP.dbo.Status /*1*/
    on (DataLog.StatusID = Status.ID)
  left join ObservationDB_IMP.dbo.StatusReason /*1*/
    on (DataLog.StatusReasonID = StatusReason.ID)
  left join ObservationDB_IMP.dbo.aspnet_Users /*1*/
    on (DataLog.UserId = aspnet_Users.UserId)
)

Insert into ObservationDB.dbo.DataLog
  (SensorID,ImportDate,ValueDate,ValueTime,ValueText,TransformValueText,RawValue,DataValue,Comment,
  InvalidDateValue,InvalidTimeValue,InvalidOffering,InvalidUOM,DataSourceTransformationID,StatusID,StatusReasonID,
  ImportStatus,UserId,PhenomenonOfferingID,PhenomenonUOMID,ImportBatchID,RawRecordData,RawFieldValue)
select 
  (Select ID from ObservationDB.dbo.Sensor where (Code = Staging.Sensor__Code)),
  Staging.ImportDate,Staging.ValueDate,Staging.ValueTime,Staging.ValueText,Staging.TransformValueText,Staging.RawValue,
  Staging.DataValue,Staging.Comment,Staging.InvalidDateValue,Staging.InvalidTimeValue,Staging.InvalidOffering,Staging.InvalidUOM,
  null/*(Select ID from ObservationDB.dbo.DataSourceTransformation where true)*/,
  (Select ID from ObservationDB.dbo.Status where (Code = Staging.Status__Code)),
  (Select ID from ObservationDB.dbo.StatusReason where (Code = Staging.Status__Code)),
  Staging.ImportStatus,
  null/*UserId*/,
  null/*PhenomenonOfferingID*/,
  null/*PhenomenonUOMID*/,
  null/*ImportBatchID*/,
  Staging.RawRecordData,Staging.RawFieldValue
from 
	Staging
	Left join Live
	on (Staging.ImportBatch__DataSource__Code = Live.ImportBatch__DataSource__Code) and
		 (Staging.ImportBatch__ImportDate = Live.ImportBatch__ImportDate) and
		 (Staging.ImportBatch__FileName = Live.ImportBatch__FileName) and
		 (Staging.Sensor__Code = Live.Sensor__Code) and
		 (Staging.ImportDate = Live.ImportDate) and
		 (Staging.ValueDate = Live.ValueDate) and
		 (Staging.ValueTime = Live.ValueTime) and
		 (Staging.PhenomenonOffering__Phenomenon__Code = Live.PhenomenonOffering__Phenomenon__Code) and
		 (Staging.PhenomenonOffering__Offering__Code = Live.PhenomenonOffering__Offering__Code) and
		 (Staging.PhenomenonUOM__Phenomenon__Code = Live.PhenomenonUOM__Phenomenon__Code) and
		 (Staging.PhenomenonUOM__UnitOfMeasure__Code = Live.PhenomenonUOM__UnitOfMeasure__Code)
where
	(Live.ID is null)


  

	