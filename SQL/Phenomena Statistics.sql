use Observations
Select
  PhenomenonName Phenomenon, Sum(Count) Count
from
  vImportBatchSummary
group by
  PhenomenonName
order by
  Sum(Count) desc, PhenomenonName

Select Sum(Count) Observations from vImportBatchSummary
Select Count(Distinct PhenomenonName) Phenomena from vImportBatchSummary
Select Count(Distinct SiteID) Sites from vImportBatchSummary
Select Count(Distinct StationID) Stations from vImportBatchSummary
Select Count(Distinct InstrumentID) Instruments from vImportBatchSummary
Select Count(Distinct SensorID) Sensors from vImportBatchSummary