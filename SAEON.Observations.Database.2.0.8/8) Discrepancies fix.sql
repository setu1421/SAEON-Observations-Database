-- DataSchema CS_FYN2_D_AIRTEMP PhenomenonOffering Max wrong, manually change 

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
