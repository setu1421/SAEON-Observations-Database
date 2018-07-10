CREATE VIEW [dbo].[vOfferingPhenomena]
AS 
Select
  PhenomenonOffering.OfferingID, Phenomenon.Code, Phenomenon.Name, Phenomenon.Description
from
  PhenomenonOffering
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
  inner join Phenomenon
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)

