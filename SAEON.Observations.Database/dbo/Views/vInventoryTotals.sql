CREATE VIEW [dbo].[vInventoryTotals]
AS 
Select
  IsNull(StatusName,'') SurrogateKey,
  IsNull(StatusName,'No status') Status, 
  Count(*) Count, Min(DataValue) Minimum, Max(DataValue) Maximum, Avg(DataValue) Average, StDev(DataValue) StandardDeviation, Var(DataValue) Variance
from  
  vObservationExpansion
group by 
  StatusName
