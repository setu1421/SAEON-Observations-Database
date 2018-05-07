Select
  PhenomenonName Phenomenon, SiteName Site, StationName Station, Min(ValueDay) StartDate, Max(ValueDay) EndDate, Count(*) Observations
from
  vObservationExpansion
where
  (PhenomenonName not in ('Current Direction','Current speed','Depth')) and
  (StationName like 'SACTN%')
group by
  PhenomenonName, SiteName, StationName
order by
  PhenomenonName, SiteName, StationName
