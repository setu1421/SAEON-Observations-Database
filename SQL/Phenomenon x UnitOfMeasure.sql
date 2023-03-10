Select
  p.Code, p.Name, u.Code, u.Unit
from
  Phenomenon p
  inner join PhenomenonUOM pu
    on (pu.PhenomenonID = p.ID)
  inner join UnitOfMeasure u
    on (pu.UnitOfMeasureID = u.ID)
order by
  p.Code, u.Code
