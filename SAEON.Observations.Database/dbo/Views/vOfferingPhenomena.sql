--> Added 2.0.36 20171220 TimPN
CREATE VIEW [dbo].[vOfferingPhemomena]
AS 
Select
  PhenomenonOffering.OfferingID, Phenomenon.Code, Phenomenon.Name, Phenomenon.Description
from
  PhenomenonOffering
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
  inner join Phenomenon
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
--< Added 2.0.36 20171220 TimPN

