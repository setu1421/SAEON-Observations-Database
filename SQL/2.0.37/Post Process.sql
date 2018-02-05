alter table Observation disable trigger TR_Observation_Update
Update
  Observation
set
  Latitude = vSensorLocation.Latitude
from
  Observation
  inner join vSensorLocation
    on (Observation.SensorID = vSensorLocation.SensorID)
  inner join vSensorDates
    on (Observation.SensorID = vSensorDates.SensorID)
where
  (Observation.Latitude is null) and (vSensorLocation.Latitude is not null) and
  ((vSensorDates.StartDate is null) or (Observation.ValueDate >= vSensorDates.StartDate)) and
  ((vSensorDates.EndDate is null) or (Observation.ValueDate <= vSensorDates.EndDate))
Update
  Observation
set
  Longitude = vSensorLocation.Longitude
from
  Observation
  inner join vSensorLocation
    on (Observation.SensorID = vSensorLocation.SensorID)
  inner join vSensorDates
    on (Observation.SensorID = vSensorDates.SensorID)
where
  (Observation.Longitude is null) and (vSensorLocation.Longitude is not null) and
  ((vSensorDates.StartDate is null) or (Observation.ValueDate >= vSensorDates.StartDate)) and
  ((vSensorDates.EndDate is null) or (Observation.ValueDate <= vSensorDates.EndDate))
Update
  Observation
set
  Elevation = vSensorLocation.Elevation
from
  Observation
  inner join vSensorLocation
    on (Observation.SensorID = vSensorLocation.SensorID)
  inner join vSensorDates
    on (Observation.SensorID = vSensorDates.SensorID)
where
  (Observation.Elevation is null) and (vSensorLocation.Elevation is not null) and
  ((vSensorDates.StartDate is null) or (Observation.ValueDate >= vSensorDates.StartDate)) and
  ((vSensorDates.EndDate is null) or (Observation.ValueDate <= vSensorDates.EndDate))
alter table Observation enable trigger TR_Observation_Update    
insert into SchemaColumnType
  (Name, Description, UserId)
values
  ('Latitude','A latitude column',(Select UserID from aspnet_Users where (UserName = 'TimPN'))),
  ('Longitude','A longitude column',(Select UserID from aspnet_Users where (UserName = 'TimPN'))),
  ('Elevation','An elevation column, negative for below sea level',(Select UserID from aspnet_Users where (UserName = 'TimPN')))