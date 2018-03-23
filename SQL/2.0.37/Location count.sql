use Observations
Select
  Count(*)
from
  Observation
  inner join vSensorLocation
    on (Observation.SensorID = vSensorLocation.SensorID)
  inner join vSensorDates
    on (Observation.SensorID = vSensorDates.SensorID)
  where
    (Observation.Latitude is null) and (vSensorLocation.Latitude is not null) and
    (Observation.Longitude is null) and (vSensorLocation.Longitude is not null) and
    ((vSensorDates.StartDate is null) or (Observation.ValueDate >= vSensorDates.StartDate)) and
    ((vSensorDates.EndDate is null) or (Observation.ValueDate <= vSensorDates.EndDate))
