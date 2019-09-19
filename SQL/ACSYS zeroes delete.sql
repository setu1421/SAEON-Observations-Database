use Observations;
Declare @Title VarChar(100) = 'ACCSYS leading zeroes'
Declare @Msg VarChar(100)
Declare @BatchSize int = 50000
Declare @BatchNum int
Declare @Done bit
Declare @BatchCount int = 0
Set @BatchNum = 1
Set @Done = 0
WHILE (@Done = 0)
  BEGIN 
	Set @Msg = Convert(VarChar(20), GetDate()) + ' ' + @Title + ' ' + Convert(VarChar(20), @BatchNum) + ' ' + Convert(VarChar(20), @BatchNum * @BatchSize)
	RAISERROR(@msg, 0, 1) WITH NOWAIT
    BEGIN TRANSACTION 
	BEGIN TRY
		Select
		  Sensor.ID, 
		  (Select Min(ValueDate) from Observation where Observation.SensorID = Sensor.ID) StartDate,
		  (Select Max(ValueDate) from Observation where Observation.SensorID = Sensor.ID) EndDate,
		  (Select Min(case when DataValue > 0 then ValueDate end) from Observation where Observation.SensorID = Sensor.ID) FirstNonZero,
		  (Select Max(case when DataValue > 0 then ValueDate end) from Observation where Observation.SensorID = Sensor.ID) LastNonZero
		into
		  #SensorDates
		from
		  Sensor
		Select
		  #SensorDates.*,
		  (Select Count(*) from Observation where ((Observation.SensorID = #SensorDates.ID) and (Observation.ValueDate < FirstNonZero))) FirstZeroes,
		  (Select Count(*) from Observation where ((Observation.SensorID = #SensorDates.ID) and (Observation.ValueDate > LastNonZero))) LastZeroes
		into
		  #SensorZeroes
		from
		  #SensorDates
		where
		  (#SensorDates.StartDate is not null)

		Delete Top(@BatchSize)
			Observation
		from
			Observation
			inner join vObservationExpansion
				on (Observation.ID = vObservationExpansion.ID)
			inner join #SensorZeroes
				on (vObservationExpansion.SensorID = #SensorZeroes.ID)
		where
		  (PhenomenonName = 'Streamflow volume') and
		  (StatusName = 'Unverified - Staging') and
		--  ((ValueDate <= FirstNonZero) or (ValueDate >= LastNonZero))
		  ((DateDiff(Month, Observation.ValueDate, FirstNonZero) >= 1) or (DateDiff(Month, LastNonZero, Observation.ValueDate) >= 1))
		Set @BatchCount = @@RowCount 
		IF @BatchCount = 0 Set @Done = 1

		Drop Table #SensorDates
		Drop Table #SensorZeroes
	END TRY
	BEGIN CATCH
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
	Set @Msg = Convert(VarChar(20), GetDate()) + ' ' + @Title + ' deleted ' + Convert(VarChar(20), @BatchCount) + ' ' + Convert(VarChar(20), @BatchNum) + ' ' + Convert(VarChar(20), @BatchNum * @BatchSize)
	RAISERROR(@msg, 0, 1) WITH NOWAIT
	set @BatchNum = @BatchNum + 1
	WAITFOR DELAY '00:00:30'
  END 
