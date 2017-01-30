use Observations
-- Observation
print 'Observation'
go
Declare @BatchSize Int = 1000000
Declare @RowCount Int = @BatchSize
Declare @BatchStart DateTime
Declare @StartDate DateTime
Declare @LastDate DateTime
Declare @GroupCount Int = 1
Declare @BatchNum Int = 0
Declare @Msg VarChar(100)
Select
  @RowCount = Count(*)
from
  (Select Count(AddedAt) Count from Observation group by AddedAt having (Count(AddedAt) > 1)) a 
while (@RowCount > 0)
begin
  Set @BatchNum += 1
  Set @BatchStart = GetDate()
  Set @Msg = Convert(varchar(100),GetDate(),113)+' Batch '+Cast(@BatchNum as varchar(100))+' Processed '+Cast((@BatchNum-1)*@BatchSize as varchar(100))
  RAISERROR(@msg, 0, 1) WITH NOWAIT
  Set @StartDate = null
  Set @LastDate = null
  begin transaction;
  disable trigger TR_Observation_Update on Observation
  Select top (@BatchSize)
    AddedAt, Count(AddedAt) Count
  into 
    #StartDates
  from
    Observation
  group by
    AddedAt
  having 
    (Count(AddedAt) > 1)
  order by
    AddedAt
  Update 
    Observation
  Set
    @StartDate = case when (@StartDate is null) then Observation.AddedAt else @StartDate end,
    @LastDate = case when (Observation.AddedAt = @StartDate) then @LastDate else Observation.AddedAt end,
    @StartDate = Observation.AddedAt,
    @LastDate = Observation.AddedAt = DATEADD(ms,10,@LastDate)
  from 
    (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt, ValueDate) RowNum, AddedAt from Observation) src
    inner join #StartDates
      on (src.AddedAt = #StartDates.AddedAt)
  where
    (Observation.ID = src.ID) and (src.RowNum > 1)
  Set @RowCount = @@RowCount
  drop table #StartDates;
  enable trigger TR_Observation_Update on Observation
  commit transaction
  checkpoint
  Set @Msg = Convert(varchar(100),GetDate(),113)+' Batch '+Cast(@BatchNum as varchar(100))+' '+Cast(DateDiff(minute,@BatchStart,GetDate()) as varchar(100))+' mins'
  RAISERROR(@msg, 0, 1) WITH NOWAIT
  Select
    @RowCount = Count(*)
  from
    (Select Count(AddedAt) Count from Observation group by AddedAt having (Count(AddedAt) > 1)) a 
end