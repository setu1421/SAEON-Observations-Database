--> Added 20170523 2.0.32 TimPN
CREATE VIEW [dbo].[vInventoryTotals]
AS 
Select
  IsNull(Status.Name,'') SurrogateKey,
  Status.Name Status, 
  Count(*) Count, Min(DataValue) Minimum, Max(DataValue) Maximum, Avg(DataValue) Average, StDev(DataValue) StandardDeviation, Var(DataValue) Variance
from  
  Observation
  left join Status
    on (Observation.StatusID = Status.ID)
group by 
  Status.Name
--< Added 20170523 2.0.32 TimPN
