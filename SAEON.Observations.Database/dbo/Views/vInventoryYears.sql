--> Added 20170523 2.0.32 TimPN
CREATE VIEW [dbo].[vInventoryYears]
AS 
Select
  YEAR(Observation.ValueDate) Year, Status.Name Status, Count(*) Count
from
  Observation
  left join Status
    on (Observation.StatusID = Status.ID)
group by
  YEAR(Observation.ValueDate), Status.Name
--< Added 20170523 2.0.32 TimPN
