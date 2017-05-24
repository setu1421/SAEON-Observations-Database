--> Added 20170523 2.0.32 TimPN
CREATE VIEW [dbo].[vInventoryPhenomena]
AS
Select
  Phenomenon.Name Phenomenon, Offering.Name Offering, Status.Name Status, count(*) Count
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




