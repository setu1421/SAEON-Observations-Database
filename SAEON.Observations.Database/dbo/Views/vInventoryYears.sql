--> Added 20170523 2.0.32 TimPN
CREATE VIEW [dbo].[vInventoryYears]
AS 
Select
  Cast(Observation.ValueYear as VarChar(10))+'~'+IsNull(Status.Name,'') SurrogateKey,
  Observation.ValueYear Year, Status.Name Status, 
  Count(*) Count, Min(DataValue) Minimum, Max(DataValue) Maximum, Avg(DataValue) Average, StDev(DataValue) StandardDeviation, Var(DataValue) Variance
from
  Observation
  left join Status
    on (Observation.StatusID = Status.ID)
group by
  Observation.ValueYear, Status.Name
--< Added 20170523 2.0.32 TimPN
