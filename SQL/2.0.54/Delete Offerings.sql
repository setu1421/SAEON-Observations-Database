with DepthOfferings
as
(
Select distinct
  Phenomenon.Name Phenomenon, 
  SubString(Offering.Name,CharIndex(' at ',Offering.Name),10000) Offering, 
  Replace(SubString(Offering.Name,CharIndex(' at ',Offering.Name)+4,10000),' Interval','s') Interval,
  'Actual_'+Replace(Replace(SubString(Offering.Name,CharIndex(' at ',Offering.Name)+4,10000),' Interval','s'),' ','_') Code,
  'Actual '+Replace(SubString(Offering.Name,CharIndex(' at ',Offering.Name)+4,10000),' Interval','s') Name,
  'Actual '+SubString(Offering.Name,CharIndex(' at ',Offering.Name),10000) Description
from
  PhenomenonOffering
  inner join Phenomenon
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
where
  (CharIndex(' at ',Offering.Name) > 0)
)

Delete
  Offering
from
  Offering
  inner join DepthOfferings
    on (DepthOfferings.Code = Offering.Code)	
