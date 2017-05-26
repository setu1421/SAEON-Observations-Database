--> Added 20170523 2.0.32 TimPN
CREATE VIEW [dbo].[vInventoryPhenomenaOfferings]
AS
Select
  Phenomenon.Name+'~'+Offering.Name+'~'+IsNull(Status.Name,'') SurrogateKey,
  Phenomenon.Name Phenomenon, Offering.Name Offering, Status.Name Status, 
  Count(*) Count, Min(DataValue) Minimum, Max(DataValue) Maximum, Avg(DataValue) Average, StDev(DataValue) StandardDeviation, Var(DataValue) Variance
from  
  Observation
  left join Status
    on (Observation.StatusID = Status.ID)
  inner join Sensor
    on (Observation.SensorID = Sensor.ID)
  inner join PhenomenonOffering
    on (Observation.PhenomenonOfferingID = PhenomenonOffering.ID)
  inner join Phenomenon
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
group by 
  Phenomenon.Name, Offering.Name, Status.Name
--< Added 20170523 2.0.32 TimPN




