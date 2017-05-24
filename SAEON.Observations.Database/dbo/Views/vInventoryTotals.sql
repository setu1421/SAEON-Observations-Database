--> Added 20170523 2.0.32 TimPN
CREATE VIEW [dbo].[vInventoryTotals]
AS 
Select
  Status.Name Status, Count(*) Count
from  
  Observation
  left join Status
    on (Observation.StatusID = Status.ID)
group by 
  Status.Name
--< Added 20170523 2.0.32 TimPN
