Select distinct
  Phenomenon.Name Phenomenon, Offering.Name Offering
from
  PhenomenonOffering
  inner join Phenomenon
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
where
  (CharIndex(' at ',Offering.Name) > 0)
order by
   Phenomenon.Name, Offering.Name
