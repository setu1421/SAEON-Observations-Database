--use [ObservationDB]
Declare @TimUserId UniqueIdentifier = (Select UserId from aspnet_Users where LoweredUserName = 'tim')
Declare @StatusID UniqueIdentifier = (Select ID from Status where Code = 'QA-98')
Declare @StatusReasonID UniqueIdentifier = (Select ID from StatusReason where Code = 'QAR-38')
-- Import batch
Update ImportBatch 
Set StatusID = @StatusID, StatusReasonID = @StatusReasonID 
from
  ImportBatch  
  inner join DataSource
    on (ImportBatch.DataSourceID = DataSource.ID)
where 
  (ImportBatch.StatusID is null) and
  ((DataSource.Name like '%CSIR%') or (DataSource.Name like '%ACSYS%'))
Checkpoint
-- Observation
Update Observation 
Set StatusID = @StatusID, StatusReasonID = @StatusReasonID 
from
  Observation
  inner join Sensor
    on (Observation.SensorID = Sensor.ID)
  inner join Station
    on (Sensor.StationID = Station.ID)
  inner join ProjectSite
    on (Station.ProjectSiteID = ProjectSite.ID)
where
  (Observation.StatusID is null) and
  ((Station.Name like '%CSIR%') or (Station.Name like '%ACSYS%') or 
   (ProjectSite.Name like '%CSIR%') or (ProjectSite.Name like '%ACSYS%'))
Checkpoint
Update Observation 
Set StatusID = @StatusID, StatusReasonID = @StatusReasonID 
from
  Observation
  inner join ImportBatch
    on (Observation.ImportBatchID = ImportBatch.ID)
  inner join DataSource
    on (ImportBatch.DataSourceID = DataSource.ID)
where
  (Observation.StatusID is null) and
  ((DataSource.Name like '%CSIR%') or (DataSource.Name like '%ACSYS%'))
Checkpoint
/*
Declare @BatchSize int = 100000
Declare @BatchNum int
-- Import batch
Set @BatchNum = 1
WHILE (1 = 1)
  BEGIN 
	Print @BatchNum * @BatchSize
    BEGIN TRANSACTION 
    -- Update
	Update top(@BatchSize) ImportBatch 
	Set StatusID = @StatusID, StatusReasonID = @StatusReasonID 
	from
	  ImportBatch  
	  inner join DataSource
		on (ImportBatch.DataSourceID = DataSource.ID)
	where 
	  (ImportBatch.StatusID is null) and
	  ((DataSource.Name like '%CSIR%') or (DataSource.Name like '%ACSYS%'))
    -- no more records to update
    IF @@ROWCOUNT = 0 
      BEGIN 
        COMMIT TRANSACTION 
        BREAK 
      END 
    COMMIT TRANSACTION 
	CheckPoint
	set @BatchNum = @BatchNum + 1
	WAITFOR DELAY '00:00:01'
  END 
CheckPoint
-- Observation via Station
Set @BatchNum = 1
WHILE (1 = 1)
  BEGIN 
	Print @BatchNum * @BatchSize
    BEGIN TRANSACTION 
    -- Update
	Update top(@BatchSize) Observation 
	Set StatusID = @StatusID, StatusReasonID = @StatusReasonID 
	from
	  Observation
	  inner join Sensor
		on (Observation.SensorID = Sensor.ID)
	  inner join Station
		on (Sensor.StationID = Station.ID)
	  inner join ProjectSite
		on (Station.ProjectSiteID = ProjectSite.ID)
	where
	  (Observation.StatusID is null) and
	  ((Station.Name like '%CSIR%') or (Station.Name like '%ACSYS%') or 
	   (ProjectSite.Name like '%CSIR%') or (ProjectSite.Name like '%ACSYS%'))
    -- no more records to update
    IF @@ROWCOUNT = 0 
      BEGIN 
        COMMIT TRANSACTION 
        BREAK 
      END 
    COMMIT TRANSACTION 
	CheckPoint
	set @BatchNum = @BatchNum + 1
	WAITFOR DELAY '00:00:01'
  END 
CheckPoint
-- Observation via ImportBatch
Set @BatchNum = 1
WHILE (1 = 1)
  BEGIN 
	Print @BatchNum * @BatchSize
    BEGIN TRANSACTION 
    -- Update
    Update top(@BatchSize) Observation 
    Set StatusID = @StatusID, StatusReasonID = @StatusReasonID 
    from
      Observation
      inner join ImportBatch
        on (Observation.ImportBatchID = ImportBatch.ID)
      inner join DataSource
        on (ImportBatch.DataSourceID = DataSource.ID)
      where
		(Observation.StatusID is null) and
		((DataSource.Name like '%CSIR%') or (DataSource.Name like '%ACSYS%'))
    -- no more records to update
    IF @@ROWCOUNT = 0 
      BEGIN 
        COMMIT TRANSACTION 
        BREAK 
      END 
    COMMIT TRANSACTION 
	CheckPoint
	set @BatchNum = @BatchNum + 1
	WAITFOR DELAY '00:00:01'
  END 
CheckPoint
*/
