Select
  p.Code, p.Name, o.Code, o.Name
from
  Phenomenon p
  inner join PhenomenonOffering po
    on (po.PhenomenonID = p.ID)
  inner join Offering o
    on (po.OfferingID = o.ID)
order by
  p.Code, o.Code
