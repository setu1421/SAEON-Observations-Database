--> Added 20170523 2.0.32 TimPN
CREATE VIEW [dbo].[vInventoryInstruments]
AS 
Select
  InstrumentName+'~'+IsNull(StatusName,'') SurrogateKey,
  --Station.ID StationID, PhenomenonOffering.ID PhenomenonOfferingID, 
  InstrumentName, IsNull(StatusName,'No status') Status, 
  Count(DataValue) Count, Min(DataValue) Minimum, Max(DataValue) Maximum, Avg(DataValue) Average, StDev(DataValue) StandardDeviation, Var(DataValue) Variance
from
  vObservationExpansion
group by
  InstrumentName, StatusName
--< Added 20170523 2.0.32 TimPN
