Select
  Year(ValueDate) Year, Month(ValueDate) Month, Count(*) Count
from
  Observation
group by
  Year(ValueDate), Month(ValueDate)
order by
  Year(ValueDate) Desc, Month(ValueDate) Desc