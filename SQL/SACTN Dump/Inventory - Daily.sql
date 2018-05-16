use ObservationsSACTN
Select
  PhenomenonName Phenomenon, OfferingName Offering, UnitOfMeasureUnit Unit, SiteName Site, StationName Station, Min(ValueDay) StartDate, Max(ValueDay) EndDate, Count(*) Observations
from
  vObservationExpansion
where
  (PhenomenonName not in ('Current Direction','Current speed','Depth')) and
  (StationName like 'SACTN%') and (SensorName like '% Daily %')
group by
  PhenomenonName, OfferingName, UnitOfMeasureUnit, SiteName, StationName
order by
  PhenomenonName, OfferingName, UnitOfMeasureUnit, SiteName, StationName
