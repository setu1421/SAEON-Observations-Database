--> Added 20170523 2.0.32 TimPN
CREATE VIEW [dbo].[vInventoryYears]
AS 
Select
  Observation.ValueYear Year, Status.Name Status, Count(*) Count
from
  Observation
  left join Status
    on (Observation.StatusID = Status.ID)
group by
  Observation.ValueYear, Status.Name
--< Added 20170523 2.0.32 TimPN
