Select
  PhenomenonName Phenomenon, Min(ValueDay) StartDate, Max(ValueDay) EndDate, Count(*) Observations, 
  Min(Longitude) West, Max(Longitude) East, Max(Latitude) North, Min(Latitude) South
from
  vObservationExpansion
where
  (PhenomenonName not in ('Current Direction','Current speed','Depth')) and
  (StationName like 'SACTN%') and (SensorName like '% Daily %')
group by
  PhenomenonName
order by
  PhenomenonName
