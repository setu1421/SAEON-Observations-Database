Select distinct
  Phenomenon.Name Phenomenon, 
  SubString(Offering.Name,CharIndex(' at ',Offering.Name),10000) Offering, 
  Replace(SubString(Offering.Name,CharIndex(' at ',Offering.Name)+4,10000),' Interval','') Interval
from
  PhenomenonOffering
  inner join Phenomenon
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
where
  (CharIndex(' at ',Offering.Name) > 0)
order by
   Phenomenon.Name, SubString(Offering.Name,CharIndex(' at ',Offering.Name),10000)

