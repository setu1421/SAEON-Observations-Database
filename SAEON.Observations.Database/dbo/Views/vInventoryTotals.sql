--> Added 20170523 2.0.32 TimPN
CREATE VIEW [dbo].[vInventoryTotals]
AS 
Select
  IsNull(StatusName,'') SurrogateKey,
  --Station.ID StationID, PhenomenonOffering.ID PhenomenonOfferingID, 
  IsNull(StatusName,'No status') Status, 
  Count(*) Count, Min(DataValue) Minimum, Max(DataValue) Maximum, Avg(DataValue) Average, StDev(DataValue) StandardDeviation, Var(DataValue) Variance
from  
  vObservationExpansion
group by 
  StatusName
--< Added 20170523 2.0.32 TimPN
