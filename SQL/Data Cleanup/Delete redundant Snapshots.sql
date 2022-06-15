--use Observations;
with RankedSnapshots
as
(
Select
  Year([When]) Year, Month([When]) Month, Day([When]) Day,
  ROW_NUMBER() over (PARTITION BY Year([When]), Month([when]), Day([when]) ORDER BY [When] Desc) RowNum,
  InventorySnapshots.*
from
  InventorySnapshots
)
--select
--  *
--from
--  RankedSnapshots
--where
--  RowNum > 1
--order by
--  [When] Desc
delete from RankedSnapshots where RowNum > 1