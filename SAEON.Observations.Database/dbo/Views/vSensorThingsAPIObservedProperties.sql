CREATE VIEW [dbo].[vSensorThingsAPIObservedProperties]
AS
Select Distinct
  Phenomenon.ID, Phenomenon.Code, Phenomenon.Name, Phenomenon.Description, Phenomenon.Url
from
  vInventory
  inner join Phenomenon
    on (vInventory.PhenomenonCode = Phenomenon.Code)
