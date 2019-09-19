use Observations;
with SensorDates--(ID, StartDate, EndDate, FirstZero, LastZero)
as
(
Select
  Sensor.ID, 
  (Select Min(ValueDate) from Observation where Observation.SensorID = Sensor.ID) StartDate,
  (Select Max(ValueDate) from Observation where Observation.SensorID = Sensor.ID) EndDate,
  (Select Min(case when DataValue > 0 then ValueDate end) from Observation where Observation.SensorID = Sensor.ID) FirstNonZero,
  (Select Max(case when DataValue > 0 then ValueDate end) from Observation where Observation.SensorID = Sensor.ID) LastNonZero
from
  Sensor
),
SensorZeroes
as
(
Select
  SensorDates.*,
  (Select Count(*) from Observation where ((Observation.SensorID = SensorDates.ID) and (Observation.ValueDate < FirstNonZero))) FirstZeroes,
  (Select Count(*) from Observation where ((Observation.SensorID = SensorDates.ID) and (Observation.ValueDate > LastNonZero))) LastZeroes
from
  SensorDates
)
--Select
--  Sensor.Name, SensorZeroes.*
--from
--  SensorZeroes
--  inner join Sensor
--    on (SensorZeroes.ID = Sensor.ID)
--order by
--  Sensor.Name
Select 
  SensorID, SiteName, StationName, InstrumentName, SensorName, PhenomenonName, ValueDate, DataValue, FirstNonZero, FirstZeroes, LastNonZero, LastZeroes
from 
  vObservationExpansion
  inner join SensorZeroes
    on (vObservationExpansion.SensorID = SensorZeroes.ID)
where
  (PhenomenonName = 'Streamflow volume') and
  (StatusName = 'Unverified - Staging') and
--  ((ValueDate <= FirstNonZero) or (ValueDate >= LastNonZero))
  ((DateDiff(Month, ValueDate, FirstNonZero) >= 1) or (DateDiff(Month, LastNonZero, ValueDate) >= 1))
order by
  SiteName, StationName, InstrumentName, SensorName, PhenomenonName, ValueDate
