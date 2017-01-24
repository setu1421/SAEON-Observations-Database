use Observations
-- Observation
print 'Observation'
go
disable trigger TR_Observation_Update on Observation
--Update Observation set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
--Update Observation set AddedAt = UpdatedAt where AddedAt is null
Declare @BatchSize Int = 1000000
Declare @RowCount Int = @BatchSize
Declare @StartDate DateTime
Declare @GroupCount Int = 1
Set @StartDate = (Select top(1) Min(AddedAt) from Observation group by AddedAt having (Count(ID) > @GroupCount))
while (@StartDate is not null)
begin
  Declare @BatchNum Int = 0
  Declare @LastAdded DateTime = @StartDate
  Declare @Msg VarChar(100)
  while (@RowCount > 0)
  begin
	Set @BatchNum += 1
	Set @Msg = Convert(varchar(100), @StartDate,121)+' Batch #'+Cast(@BatchNum as varchar(100))
	RAISERROR(@msg, 0, 1) WITH NOWAIT
	begin transaction
	Update top (@BatchSize)
	  Observation
	Set
	  @LastAdded = AddedAt = DATEADD(ms,10,@LastAdded)
	from
	  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt, ValueDate) RowNum from Observation where AddedAt = @StartDate) src
	where
	  (Observation.ID = src.ID) and (src.RowNum > 1)
	Set @RowCount = @@RowCount
	commit transaction
	checkpoint
	Set @Msg = Convert(varchar(100), @StartDate,121)+' Batch #'+Cast(@BatchNum as varchar(100))+' added '+Cast(@RowCount as varchar(100))+' rows'
	RAISERROR(@msg, 0, 1) WITH NOWAIT
  end
  Set @RowCount = @BatchSize
  Set @StartDate = (Select top(1) Min(AddedAt) from Observation group by AddedAt having (Count(ID) > @GroupCount))
end;
enable trigger TR_Observation_Update on Observation
