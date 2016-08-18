-- DataSchema CS_FYN2_D_AIRTEMP PhenomenonOffering Max wrong, manually change
-- Sensor JNHK_DWS_CSAWS_BP_D in staging linked to wrong phenomenon and dataschema (Fog, should be pressure), manually change 

-- Live fixes
use [ObservationDB]
Update
  DataSchema
Set
  UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'victoriag')
where
  (Code = 'ACSYS_R')
Update
  DataSchema
Set
  UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'abri')
where
  (Code = 'DAV_PR2')
Update
  Phenomenon
set
  Url = 'http://data.saeon.ac.za'
where
  Code = 'WRUN'
Update
  Sensor
set
  UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'abri')
where
  (Code = 'CONSTB_CWS_CSAWS_SM1')
-- Staging fixes
use [ObservationDB_IMP]
Update 
  ProjectSite
set
  UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'tim')
from
  ProjectSite
  inner join aspnet_Users
    on (ProjectSite.UserId = aspnet_Users.UserId)
where
  (aspnet_Users.LoweredUserName = '1')
Update 
  Station
set
  UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'tim')
from
  Station
  inner join aspnet_Users
    on (Station.UserId = aspnet_Users.UserId)
where
  (aspnet_Users.LoweredUserName = '1')
Update 
  DataSchema
set
  UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'tim')
from
  DataSchema
  inner join aspnet_Users
    on (DataSchema.UserId = aspnet_Users.UserId)
where
  (aspnet_Users.LoweredUserName = '1')
Update 
  DataSource
set
  UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'tim')
from
  DataSource
  inner join aspnet_Users
    on (DataSource.UserId = aspnet_Users.UserId)
where
  (aspnet_Users.LoweredUserName = '1')
Update 
  Phenomenon
set
  UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'tim')
from
  Phenomenon
  inner join aspnet_Users
    on (Phenomenon.UserId = aspnet_Users.UserId)
where
  (aspnet_Users.LoweredUserName = '1')
Update 
  Offering
set
  UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'tim')
from
  Offering
  inner join aspnet_Users
    on (Offering.UserId = aspnet_Users.UserId)
where
  (aspnet_Users.LoweredUserName = '1')
Update 
  UnitOfMeasure
set
  UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'tim')
from
  UnitOfMeasure
  inner join aspnet_Users
    on (UnitOfMeasure.UserId = aspnet_Users.UserId)
where
  (aspnet_Users.LoweredUserName = '1')
Update 
  Sensor
set
  UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'tim')
from
  Sensor
  inner join aspnet_Users
    on (Sensor.UserId = aspnet_Users.UserId)
where
  (aspnet_Users.LoweredUserName = '1')
Update 
  ImportBatch
set
  UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'tim')
from
  ImportBatch
  inner join aspnet_Users
    on (ImportBatch.UserId = aspnet_Users.UserId)
where
  (aspnet_Users.LoweredUserName = '1')
Update
  DataSchema
Set
  Code = 'DAV_PR2'
where
  (Code = 'DAV_FYN6')
Update
  Phenomenon
set
  Description = 'The "amount" of wind passing the station during a given period of time, expressed in "kilometers of wind". Davis WeatherLink calculates wind run by multiplying the average wind speed for each archive record by the archive interval.',
  UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'tonys')
where
  Code = 'WRUN'
Update
  Sensor
set
  UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'abri')
where
  (Code = 'JNHK_DWS_CSAWS_BP')
Update
  Sensor
set
  Code = 'JNHK_DWS_CSAWS_BP_D-S'
where
  (Code = 'JNHK_DWS_CSAWS_BP_D')
  