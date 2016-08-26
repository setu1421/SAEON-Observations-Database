use [ObservationDB_IMP]
Declare @TimUserId UniqueIdentifier = (Select UserId from aspnet_Users where LoweredUserName = 'tim')
Declare @StatusID UniqueIdentifier = (Select ID from Status where Code = 'QA-98')
Declare @StatusReasonID UniqueIdentifier = (Select ID from StatusReason where Code = 'QAR-38')
-- Import batch
Update ImportBatch Set StatusID = @StatusID, StatusReasonID = @StatusReasonID where StatusID is null
Checkpoint
-- Observation
Update Observation Set StatusID = @StatusID, StatusReasonID = @StatusReasonID where StatusID is null
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
	Update top(@BatchSize) ImportBatch Set StatusID = @StatusID, StatusReasonID = @StatusReasonID where StatusID is null
    -- no more records to update
    IF @@ROWCOUNT = 0 
      BEGIN 
        COMMIT TRANSACTION 
		Checkpoint
        BREAK 
      END 
    COMMIT TRANSACTION 
	Checkpoint
	set @BatchNum = @BatchNum + 1
	WAITFOR DELAY '00:00:01'
  END 
-- Observation
Set @BatchNum = 1
WHILE (1 = 1)
  BEGIN 
	Print @BatchNum * @BatchSize
    BEGIN TRANSACTION 
    -- Update
	Update top(@BatchSize) Observation Set StatusID = @StatusID, StatusReasonID = @StatusReasonID where StatusID is null
    -- no more records to update
    IF @@ROWCOUNT = 0 
      BEGIN 
        COMMIT TRANSACTION 
		Checkpoint
        BREAK 
      END 
    COMMIT TRANSACTION 
	Checkpoint
	set @BatchNum = @BatchNum + 1
	WAITFOR DELAY '00:00:01'
  END 
*/


