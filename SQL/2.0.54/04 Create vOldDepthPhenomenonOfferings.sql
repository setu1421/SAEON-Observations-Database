use Observations;
Go
Create View vOldDepthPhenomenonOfferings as
--Alter View vOldDepthPhenomenonOfferings as
Select distinct
  OldOfferingID,
  OldOfferingCode,
  OldOfferingName,
  OldOfferingDescription,
  Source,
  Phenomenon.ID PhenomenonID,
  Phenomenon.Code PhenomenonCode, 
  Phenomenon.Name PhenomenonName,
  PhenomenonOffering.ID OldPhenomenonOfferingID
from
  PhenomenonOffering
  inner join Phenomenon
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
  inner join vOldDepthOfferings
    on (PhenomenonOffering.OfferingID = vOldDepthOfferings.OldOfferingID)
