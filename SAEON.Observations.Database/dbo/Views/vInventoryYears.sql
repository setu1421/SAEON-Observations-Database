CREATE VIEW [dbo].[vInventoryYears]
AS 
Select
  Cast(ValueYear as VarChar(10))+'~'+IsNull(StatusName,'') SurrogateKey,
  ValueYear Year, IsNull(StatusName,'No status') Status, 
  Count(*) Count, Min(DataValue) Minimum, Max(DataValue) Maximum, Avg(DataValue) Average, StDev(DataValue) StandardDeviation, Var(DataValue) Variance
from
  vObservationExpansion
group by
  ValueYear, StatusName
