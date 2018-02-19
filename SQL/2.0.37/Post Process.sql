use Observations
insert into SchemaColumnType
  (Name, Description, UserId)
values
  ('Latitude','A latitude column',(Select UserID from aspnet_Users where (UserName = 'TimPN'))),
  ('Longitude','A longitude column',(Select UserID from aspnet_Users where (UserName = 'TimPN'))),
  ('Elevation','An elevation column, negative for below sea level',(Select UserID from aspnet_Users where (UserName = 'TimPN')))
Declare @Msg VarChar(100)
Declare @BatchSize int = 10000
Declare @BatchNum int
Declare @Done bit
Declare @Updated int = 0
-- Latitude, Longitude
Set @BatchNum = 1
Set @Done = 0
WHILE (@Done = 0)
  BEGIN 
	Set @Msg = Convert(VarChar(20), GetDate()) + ' Latitude, Longitude ' + Convert(VarChar(20), @BatchNum) + ' ' + Convert(VarChar(20), @BatchNum * @BatchSize)
	RAISERROR(@msg, 0, 1) WITH NOWAIT
    BEGIN TRANSACTION 
	alter table Observation disable trigger TR_Observation_Update
	Update Top (@BatchSize)
	  Observation
	set
	  Latitude = vSensorLocation.Latitude,
	  Longitude = vSensorLocation.Longitude
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
    Set @Updated = @@RowCount 
	Set @Msg = Convert(VarChar(20), GetDate()) + ' Latitude, Longitude Updated ' + Convert(VarChar(20), @Updated) + ' ' + Convert(VarChar(20), @BatchNum) + ' ' + Convert(VarChar(20), @BatchNum * @BatchSize)
	RAISERROR(@msg, 0, 1) WITH NOWAIT
    IF @Updated = 0 Set @Done = 1
	alter table Observation enable trigger TR_Observation_Update    
    COMMIT TRANSACTION 
	CheckPoint
	set @BatchNum = @BatchNum + 1
  END 
-- Latitude
Set @BatchNum = 1
Set @Done = 0
WHILE (@Done = 0)
  BEGIN 
	Set @Msg = Convert(VarChar(20), GetDate()) + ' Latitude ' + Convert(VarChar(20), @BatchNum) + ' ' + Convert(VarChar(20), @BatchNum * @BatchSize)
	RAISERROR(@msg, 0, 1) WITH NOWAIT
    BEGIN TRANSACTION 
	alter table Observation disable trigger TR_Observation_Update
	Update Top (@BatchSize)
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
	  (Observation.Longitude is null) and (vSensorLocation.Longitude is not null) and
	  ((vSensorDates.StartDate is null) or (Observation.ValueDate >= vSensorDates.StartDate)) and
	  ((vSensorDates.EndDate is null) or (Observation.ValueDate <= vSensorDates.EndDate))
    IF @@ROWCOUNT = 0 Set @Done = 1
	alter table Observation enable trigger TR_Observation_Update    
    COMMIT TRANSACTION 
	CheckPoint
	set @BatchNum = @BatchNum + 1
  END 
-- Latitude, Longitude
Set @BatchNum = 1
Set @Done = 0
WHILE (@Done = 0)
  BEGIN 
	Set @Msg = Convert(VarChar(20), GetDate()) + ' Latitude ' + Convert(VarChar(20), @BatchNum) + ' ' + Convert(VarChar(20), @BatchNum * @BatchSize)
	RAISERROR(@msg, 0, 1) WITH NOWAIT
    BEGIN TRANSACTION 
	alter table Observation disable trigger TR_Observation_Update
	Update Top (@BatchSize)
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
	  (Observation.Latitude is null) and (vSensorLocation.Latitude is not null) and
	  (Observation.Longitude is null) and (vSensorLocation.Longitude is not null) and
	  ((vSensorDates.StartDate is null) or (Observation.ValueDate >= vSensorDates.StartDate)) and
	  ((vSensorDates.EndDate is null) or (Observation.ValueDate <= vSensorDates.EndDate))
    IF @@ROWCOUNT = 0 Set @Done = 1
	alter table Observation enable trigger TR_Observation_Update    
    COMMIT TRANSACTION 
	CheckPoint
	set @BatchNum = @BatchNum + 1
  END 
-- Elevation
Set @BatchNum = 1
Set @Done = 0
WHILE (@Done = 0)
  BEGIN 
	Set @Msg = Convert(VarChar(20), GetDate()) + ' Elevation ' + Convert(VarChar(20), @BatchNum) + ' ' + Convert(VarChar(20), @BatchNum * @BatchSize)
	RAISERROR(@msg, 0, 1) WITH NOWAIT
    BEGIN TRANSACTION 
	alter table Observation disable trigger TR_Observation_Update
	Update Top (@BatchSize)
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
    IF @@ROWCOUNT = 0 Set @Done = 1
	alter table Observation enable trigger TR_Observation_Update    
    COMMIT TRANSACTION 
	CheckPoint
	set @BatchNum = @BatchNum + 1
  END 
Print 'Done'
