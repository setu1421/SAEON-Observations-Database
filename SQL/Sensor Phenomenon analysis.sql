Select 
  SiteCode, SiteName, StationCode, StationName, InstrumentCode, InstrumentName, SensorCode, SensorName, PhenomenonName, Min(StartDate) StartDate, Max(EndDate) EndDate, Sum(Count) Observations
from
  vImportBatchSummary
group by
  SiteCode, SiteName, StationCode, StationName, InstrumentCode, InstrumentName, SensorCode, SensorName, PhenomenonName
order by
  SiteCode, SiteName, StationCode, StationName, InstrumentCode, InstrumentName, SensorCode, SensorName, PhenomenonName