Select
  PhenomenonName Phenomenon, SiteName Site, StationName Station, Min(ValueDay) StartDate, Max(ValueDay) EndDate, Count(*) Observations, 
  Min(Latitude) West, Max(Latitude) East, Min(Longitude) North, Max(Longitude) South
from
  vObservationExpansion
where
  (PhenomenonName not in ('Current Direction','Current speed','Depth')) and
  (SiteName like 'SACTN%')
group by
  PhenomenonName, SiteName, StationName
order by
  PhenomenonName, SiteName, StationName
