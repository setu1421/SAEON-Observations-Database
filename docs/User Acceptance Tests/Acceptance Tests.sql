Use ObservationsTest
RAISERROR ('Cleaning', 0, 1) WITH NOWAIT
Delete Instrument_Sensor from Instrument_Sensor inner join Sensor on (Instrument_Sensor.SensorID = Sensor.ID) where Sensor.Code like '_Test_%'
Delete Sensor where Code like '_Test_%'
Delete DataSourceRole from DataSourceRole inner join DataSource on (DataSourceRole.DataSourceID = DataSource.ID) where DataSource.Code like '_Test_%'
Delete DataSource where Code like '_Test_%'
Delete SchemaColumn from SchemaColumn inner join DataSchema on (SchemaColumn.DataSchemaID = DataSchema.ID) where DataSchema.Code  like '_Test_%'
Delete DataSchema where Code like '_Test_%'
Delete Station_Instrument from Station_Instrument inner join Instrument on (Station_Instrument.InstrumentID = Instrument.ID) where Instrument.Code like '_Test_%' 
Delete Instrument where Code like '_Test_%'
Delete Station where Code like '_Test_%'
Delete Site where Code like '_Test_%'
Declare @UserID UniqueIdentifier = (Select UserId from aspnet_Users where UserName = 'TimPN')
RAISERROR ('Adding Site', 0, 1) WITH NOWAIT
Insert into Site
(Code, Name, Description, Url, UserID)
Values
('_Test_Site','_Test_Site','_Test_Site','http://www.saeon.ac.za',@UserID)
RAISERROR ('Adding Station', 0, 1) WITH NOWAIT
Insert into Station
(Code, Name, Description, Url, SiteID, UserId)
Values
('_Test_Station','_Test_Station','_Test_Station','http://www.saeon.ac.za',(Select ID from Site where Code = '_Test_Site'),@UserID)
RAISERROR ('Adding Instrument', 0, 1) WITH NOWAIT
Insert into Instrument
(Code, Name, Description, Url, UserId)
Values
('_Test_Instrument','_Test_Instrument','_Test_Instrument','http://www.saeon.ac.za',@UserID)
RAISERROR ('Linking Instrument to Station', 0, 1) WITH NOWAIT
Insert into Station_Instrument
(StationID, InstrumentID, UserId)
Values
((Select ID from Station where Code = '_Test_Station'), (Select ID from Instrument where Code = '_Test_Instrument'),@UserID)
RAISERROR ('Adding DataSchema', 0, 1) WITH NOWAIT
Insert into DataSchema
(Code, Name, Description, DataSourceTypeID, HasColumnNames, Delimiter, UserId)
Values
('_Test_DataSchema','_Test_DataSchema','_Test_DataSchema',(Select ID from DataSourceType where Code = 'CSV'),1,',',@UserID)
RAISERROR ('Adding Data column', 0, 1) WITH NOWAIT
Insert into SchemaColumn
(DataSchemaID, Number, Name, SchemaColumnTypeID, Format, UserId)
Values
((Select ID from DataSchema where Code = '_Test_DataSchema'), 1, 'Date', (Select ID from SchemaColumnType where Name = 'Date'), 'yyyy-MM-dd',@UserID)
Insert into SchemaColumn
(DataSchemaID, Number, Name, SchemaColumnTypeID, PhenomenonID, PhenomenonOfferingID, PhenomenonUOMID, UserId)
Values
((Select ID from DataSchema where Code = '_Test_DataSchema'), 2, 'Temperature', (Select ID from SchemaColumnType where Name = 'Offering'), 
(Select ID from Phenomenon where Name = 'Temperature'),
(
Select 
  PhenomenonOffering.ID 
from 
  PhenomenonOffering 
  inner join Phenomenon 
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID) 
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
where
  (Phenomenon.Name = 'Temperature') and
  (Offering.Name = 'Actual')
),
(
Select 
  PhenomenonUOM.ID 
from 
  PhenomenonUOM
  inner join Phenomenon 
    on (PhenomenonUOM.PhenomenonID = Phenomenon.ID) 
  inner join UnitOfMeasure
    on (PhenomenonUOM.UnitOfMeasureID = UnitOfMeasure.ID)
where
  (Phenomenon.Name = 'Temperature') and
  (UnitOfMeasure.Unit = 'Degrees Celsius')
),@UserID)
RAISERROR ('Adding Humidity column', 0, 1) WITH NOWAIT
Insert into SchemaColumn
(DataSchemaID, Number, Name, SchemaColumnTypeID, PhenomenonID, PhenomenonOfferingID, PhenomenonUOMID, UserId)
Values
((Select ID from DataSchema where Code = '_Test_DataSchema'), 3, 'Humidity', (Select ID from SchemaColumnType where Name = 'Offering'), 
(Select ID from Phenomenon where Name = 'Humidity'),
(
Select 
  PhenomenonOffering.ID 
from 
  PhenomenonOffering 
  inner join Phenomenon 
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID) 
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
where
  (Phenomenon.Name = 'Humidity') and
  (Offering.Name = 'Average')
),
(
Select 
  PhenomenonUOM.ID 
from 
  PhenomenonUOM
  inner join Phenomenon 
    on (PhenomenonUOM.PhenomenonID = Phenomenon.ID) 
  inner join UnitOfMeasure
    on (PhenomenonUOM.UnitOfMeasureID = UnitOfMeasure.ID)
where
  (Phenomenon.Name = 'Humidity') and
  (UnitOfMeasure.Unit = 'Percent')
),@UserID)
RAISERROR ('Adding Wind column', 0, 1) WITH NOWAIT
Insert into SchemaColumn
(DataSchemaID, Number, Name, SchemaColumnTypeID, PhenomenonID, PhenomenonOfferingID, PhenomenonUOMID,UserId)
Values
((Select ID from DataSchema where Code = '_Test_DataSchema'), 4, 'Wind', (Select ID from SchemaColumnType where Name = 'Offering'), 
(Select ID from Phenomenon where Name = 'Wind'),
(
Select 
  PhenomenonOffering.ID 
from 
  PhenomenonOffering 
  inner join Phenomenon 
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID) 
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
where
  (Phenomenon.Name = 'Wind') and
  (Offering.Name = 'Average')
),
(
Select 
  PhenomenonUOM.ID 
from 
  PhenomenonUOM
  inner join Phenomenon 
    on (PhenomenonUOM.PhenomenonID = Phenomenon.ID) 
  inner join UnitOfMeasure
    on (PhenomenonUOM.UnitOfMeasureID = UnitOfMeasure.ID)
where
  (Phenomenon.Name = 'Wind Direction') and
  (UnitOfMeasure.Unit = 'Degrees')
),@UserID)
RAISERROR ('Adding Rainfall column', 0, 1) WITH NOWAIT
Insert into SchemaColumn
(DataSchemaID, Number, Name, SchemaColumnTypeID, PhenomenonID, PhenomenonOfferingID, PhenomenonUOMID, UserId)
Values
((Select ID from DataSchema where Code = '_Test_DataSchema'), 5, 'Rain', (Select ID from SchemaColumnType where Name = 'Offering'), 
(Select ID from Phenomenon where Name = 'Rainfall'),
(
Select 
  PhenomenonOffering.ID 
from 
  PhenomenonOffering 
  inner join Phenomenon 
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID) 
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
where
  (Phenomenon.Name = 'Rainfall') and
  (Offering.Name = 'Actual')
),
(
Select 
  PhenomenonUOM.ID 
from 
  PhenomenonUOM
  inner join Phenomenon 
    on (PhenomenonUOM.PhenomenonID = Phenomenon.ID) 
  inner join UnitOfMeasure
    on (PhenomenonUOM.UnitOfMeasureID = UnitOfMeasure.ID)
where
  (Phenomenon.Name = 'Rainfall') and
  (UnitOfMeasure.Unit = 'Milimeters per hour')
),@UserID)
RAISERROR ('Adding DataSource', 0, 1) WITH NOWAIT
Insert into DataSource
(Code, Name, Description, Url, DataSchemaID, UpdateFreq, LastUpdate, UserId)
Values
('_Test_DataSource','_Test_DataSource','_Test_DataSource','http://www.saeon.ac.za',(Select ID from DataSchema where Code = '_Test_DataSchema'),0,'1900-01-01',@UserID)
RAISERROR ('Linking DataSource Role', 0, 1) WITH NOWAIT
Insert into DataSourceRole
(DataSourceID, RoleId, RoleName, IsRoleReadOnly, UserId)
Values
((Select ID from DataSource where Code = '_Test_DataSource'),(Select RoleId from aspnet_Roles where RoleName='Administrator'),'Administrator',0,@UserID)
RAISERROR ('Adding Sensor Temperature', 0, 1) WITH NOWAIT
Insert into Sensor
(Code, Name, Description, Url, PhenomenonID, DataSourceID, UserId)
Values
('_Test_Sensor_Temperature','_Test_Sensor_Temperature','_Test_Sensor_Temperature','http://www/saeon.ac.za',
(Select ID from Phenomenon where Name = 'Temperature'), (Select ID from DataSource where Code = '_Test_DataSource'),@UserID)
RAISERROR ('Linking Sensor Temperature', 0, 1) WITH NOWAIT
Insert into Instrument_Sensor
(InstrumentID, SensorID, UserID)
Values
((Select ID from Instrument where Code = '_Test_Instrument'),(Select ID from Sensor where Code = '_Test_Sensor_Temperature'),@UserID)
RAISERROR ('Adding Sensor Humidity', 0, 1) WITH NOWAIT
Insert into Sensor
(Code, Name, Description, Url, PhenomenonID, DataSourceID, UserId)
Values
('_Test_Sensor_Humidity','_Test_Sensor_Humidity','_Test_Sensor_Humidity','http://www/saeon.ac.za',
(Select ID from Phenomenon where Name = 'Humidity'), (Select ID from DataSource where Code = '_Test_DataSource'),@UserID)
RAISERROR ('Linking Sensor Humidity', 0, 1) WITH NOWAIT
Insert into Instrument_Sensor
(InstrumentID, SensorID, UserID)
Values
((Select ID from Instrument where Code = '_Test_Instrument'),(Select ID from Sensor where Code = '_Test_Sensor_Humidity'),@UserID)
RAISERROR ('Adding Sensor Wind', 0, 1) WITH NOWAIT
Insert into Sensor
(Code, Name, Description, Url, PhenomenonID, DataSourceID, UserId)
Values
('_Test_Sensor_Wind','_Test_Sensor_Wind','_Test_Sensor_Wind','http://www/saeon.ac.za',
(Select ID from Phenomenon where Name = 'Wind'), (Select ID from DataSource where Code = '_Test_DataSource'),@UserID)
RAISERROR ('Linking Sensor Wind', 0, 1) WITH NOWAIT
Insert into Instrument_Sensor
(InstrumentID, SensorID, UserID)
Values
((Select ID from Instrument where Code = '_Test_Instrument'),(Select ID from Sensor where Code = '_Test_Sensor_Wind'),@UserID)
RAISERROR ('Adding Sensor Rain', 0, 1) WITH NOWAIT
Insert into Sensor
(Code, Name, Description, Url, PhenomenonID, DataSourceID, UserId)
Values
('_Test_Sensor_Rain','_Test_Sensor_Rain','_Test_Sensor_Rain','http://www/saeon.ac.za',
(Select ID from Phenomenon where Name = 'Rainfall'), (Select ID from DataSource where Code = '_Test_DataSource'),@UserID)
RAISERROR ('Linking Sensor Rain', 0, 1) WITH NOWAIT
Insert into Instrument_Sensor
(InstrumentID, SensorID, UserID)
Values
((Select ID from Instrument where Code = '_Test_Instrument'),(Select ID from Sensor where Code = '_Test_Sensor_Rain'),@UserID)
RAISERROR ('Adding Sensor DewPoint', 0, 1) WITH NOWAIT
Insert into Sensor
(Code, Name, Description, Url, PhenomenonID, DataSourceID, UserId)
Values
('_Test_Sensor_DewPoint','_Test_Sensor_DewPoint','_Test_Sensor_DewPoint','http://www/saeon.ac.za',
(Select ID from Phenomenon where Name = 'Dew Point Temperature'), (Select ID from DataSource where Code = '_Test_DataSource'),@UserID)
RAISERROR ('Linking Sensor DewPoint', 0, 1) WITH NOWAIT
Insert into Instrument_Sensor
(InstrumentID, SensorID, UserID)
Values
((Select ID from Instrument where Code = '_Test_Instrument'),(Select ID from Sensor where Code = '_Test_Sensor_DewPoint'),@UserID)
RAISERROR ('Done', 0, 1) WITH NOWAIT


