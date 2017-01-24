use Observations
Select
  AddedAt, Count(RowNum) Count, Max(RowNum) Max
from
  (
  Select
    RowNum, AddedAt
  from
    (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt, ValueDate) RowNum, AddedAt from Observation) src
  where
    (src.RowNum > 1)
  ) dups
group by
  AddedAt
order by
  Max(RowNum) Desc, AddedAt
