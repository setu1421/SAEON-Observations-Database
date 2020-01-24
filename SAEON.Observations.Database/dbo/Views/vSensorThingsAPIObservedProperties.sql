CREATE VIEW [dbo].[vSensorThingsAPIObservedProperties]
AS
Select Distinct
  PhenomenonOfferingID ID,
  PhenomenonCode, PhenomenonName, Phenomenon.Description PhenomenonDescription, Phenomenon.Url PhenomenonUrl,
  OfferingCode, OfferingName, Offering.Description OfferingDescription
from
  vInventory
  inner join Phenomenon
    on (vInventory.PhenomenonCode = Phenomenon.Code)
  inner join PhenomenonOffering
    on (vInventory.PhenomenonOfferingID = PhenomenonOffering.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
