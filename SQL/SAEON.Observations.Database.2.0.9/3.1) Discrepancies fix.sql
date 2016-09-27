-- DataSchema CS_FYN2_D_AIRTEMP PhenomenonOffering Max wrong, manually change
-- Sensor JNHK_DWS_CSAWS_BP_D in staging linked to wrong phenomenon and dataschema (Fog, should be pressure), manually change 
--Add following users to ObserveDB {1, Julia, Merinda, Shaun, Tommy}
--Add following users to ObserveDB_Imp {Shaun, Tommy}

-- Live fixes
use [ObservationDB]
insert into [Status]
  (Code, Name, Description, UserId)
values
  ('QA-98','Unverified - Staging','Unverified staging data, not loaded into log',(Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'victoriag'))
Update DataSchema
Set UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'victoriag')
where (Code = 'ACSYS_R')
Update DataSchema
Set UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'abri')
where (Code = 'DAV_PR2')
Update Phenomenon
set Url = 'http://data.saeon.ac.za'
where Code = 'WRUN'
Update Station
Set UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'abri')
where (Code = 'JNHK_BIES_19B')
Update Sensor
set UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'abri')
where (Code = 'CONSTB_CWS_CSAWS_SM1')
-- Staging fixes
use [ObservationDB_IMP]
Update ProjectSite
set UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'tim')
from
  ProjectSite
  inner join aspnet_Users
    on (ProjectSite.UserId = aspnet_Users.UserId)
where (aspnet_Users.LoweredUserName = '1')
Update Station
set UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'tim')
from
  Station
  inner join aspnet_Users
    on (Station.UserId = aspnet_Users.UserId)
where (aspnet_Users.LoweredUserName = '1')
Update DataSchema
Set UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'jp')
where (Code = 'CS_FYN7_FOG_D')
Update DataSchema
set UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'tim')
from
  DataSchema
  inner join aspnet_Users
    on (DataSchema.UserId = aspnet_Users.UserId)
where (aspnet_Users.LoweredUserName = '1')
Update DataSource
set UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'tim')
from
  DataSource
  inner join aspnet_Users
    on (DataSource.UserId = aspnet_Users.UserId)
where (aspnet_Users.LoweredUserName = '1')
Update Phenomenon
set UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'tim')
from
  Phenomenon
  inner join aspnet_Users
    on (Phenomenon.UserId = aspnet_Users.UserId)
where (aspnet_Users.LoweredUserName = '1')
Update Offering
set UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'tim')
from
  Offering
  inner join aspnet_Users
    on (Offering.UserId = aspnet_Users.UserId)
where (aspnet_Users.LoweredUserName = '1')
Update UnitOfMeasure
set UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'tim')
from
  UnitOfMeasure
  inner join aspnet_Users
    on (UnitOfMeasure.UserId = aspnet_Users.UserId)
where (aspnet_Users.LoweredUserName = '1')
Update Sensor
set UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'tim')
from
  Sensor
  inner join aspnet_Users
    on (Sensor.UserId = aspnet_Users.UserId)
where (aspnet_Users.LoweredUserName = '1')
Update ImportBatch
set UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'tim')
from
  ImportBatch
  inner join aspnet_Users
    on (ImportBatch.UserId = aspnet_Users.UserId)
where (aspnet_Users.LoweredUserName = '1')
Update Sensor
set StationID = (Select ID from Station where Code = 'CONSTB_CWS_W')
from
  Sensor
  inner join Station
    on (Sensor.StationID = Station.ID)
where (Station.Code = 'Const_test')
Update Sensor
set DataSourceID = (Select ID from DataSource where Code = 'CONSTB_CWS_CSAWS')
from
  Sensor
  inner join DataSource
    on (Sensor.DataSourceID = DataSource.ID)
where (DataSource.Code = 'Const_test')
Update DataSchema
Set Code = 'DAV_PR2'
where (Code = 'DAV_FYN6')
Update Phenomenon
set
  Description = 'The "amount" of wind passing the station during a given period of time, expressed in "kilometers of wind". Davis WeatherLink calculates wind run by multiplying the average wind speed for each archive record by the archive interval.',
  UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'tonys')
where Code = 'WRUN'
Update Sensor
set UserId = (Select UserId from aspnet_Users where aspnet_Users.LoweredUserName = 'abri')
where (Code = 'JNHK_DWS_CSAWS_BP')
Update Sensor
set Code = 'JNHK_DWS_CSAWS_BP_D-S'
where (Code = 'JNHK_DWS_CSAWS_BP_D')
Update Sensor
set DataSourceID = (Select ID from DataSource where Code = 'CONSTB_CWS_CSAWS_D')
where (Code = 'CONSTB_CWS_CSAWS_SM6_D')   
Update Sensor
set DataSourceID = (Select ID from DataSource where Code = 'CONSTB_CWS_CSAWS_D')
where (Code = 'CONSTB_CWS_CSAWS_T_D')   
Update Sensor
set DataSourceID = (Select ID from DataSource where Code = 'CONSTB_CWS_CSAWS_D')
where (Code = 'CONSTB_CWS_CSAWS_WD_D')
Update Sensor
set DataSourceID = (Select ID from DataSource where Code = 'CONSTB_CWS_CSAWS_D')
where (Code = 'CONSTB_CWS_CSAWS_WS_D')
Update Sensor
set DataSourceID = (Select ID from DataSource where Code = 'CONSTB_CWS_CSAWS_D')
where (Code = 'CONSTB_DWS_CSAWS_SM2_D')
Update Sensor
Set DataSchemaID = (Select ID from DataSchema where Code = 'CS_FYN2_FOG')
where Code = 'CONSTB_CWS_CSAWS_FOG'
Update Sensor
Set DataSchemaID = (Select ID from DataSchema where Code = 'CS_FYN2_RAIN')
where Code = 'CONSTB_CWS_CSAWS_RAIN'
Update Sensor
Set DataSchemaID = (Select ID from DataSchema where Code = 'CS_FYN2_RH')
where Code = 'CONSTB_CWS_CSAWS_RH'
Update Sensor
Set DataSchemaID = (Select ID from DataSchema where Code = 'CS_FYN2_SM1')
where Code = 'CONSTB_CWS_CSAWS_SM1'
Update Sensor
Set DataSchemaID = (Select ID from DataSchema where Code = 'CS_FYN2_SM2')
where Code = 'CONSTB_CWS_CSAWS_SM2'
Update Sensor
Set DataSchemaID = (Select ID from DataSchema where Code = 'CS_FYN2_SM3')
where Code = 'CONSTB_CWS_CSAWS_SM3'
Update Sensor
Set DataSchemaID = (Select ID from DataSchema where Code = 'CS_FYN2_SM4')
where Code = 'CONSTB_CWS_CSAWS_SM4'
Update Sensor
Set DataSchemaID = (Select ID from DataSchema where Code = 'CS_FYN2_SM5')
where Code = 'CONSTB_CWS_CSAWS_SM5'
Update Sensor
Set DataSchemaID = (Select ID from DataSchema where Code = 'CS_FYN2_SM6')
where Code = 'CONSTB_CWS_CSAWS_SM6'
Update Sensor
Set DataSchemaID = (Select ID from DataSchema where Code = 'CS_FYN2_D_SM6')
where Code = 'CONSTB_CWS_CSAWS_SM6_D'
Update Sensor
Set DataSchemaID = (Select ID from DataSchema where Code = 'CS_FYN2_AIRTEMP')
where Code = 'CONSTB_CWS_CSAWS_T'
Update Sensor
Set DataSchemaID = (Select ID from DataSchema where Code = 'CS_FYN2_D_AIRTEMP')
where Code = 'CONSTB_CWS_CSAWS_T_D'
Update Sensor
Set DataSchemaID = (Select ID from DataSchema where Code = 'CS_FYN2_WD')
where Code = 'CONSTB_CWS_CSAWS_WD'
Update Sensor
Set DataSchemaID = (Select ID from DataSchema where Code = 'CS_FYN2_D_WD')
where Code = 'CONSTB_CWS_CSAWS_WD_D'
Update Sensor
Set DataSchemaID = (Select ID from DataSchema where Code = 'CS_FYN2_WS')
where Code = 'CONSTB_CWS_CSAWS_WS'
Update Sensor
Set DataSchemaID = (Select ID from DataSchema where Code = 'CS_FYN2_D_WS')
where Code = 'CONSTB_CWS_CSAWS_WS_D'
Update Sensor
Set DataSchemaID = (Select ID from DataSchema where Code = 'CS_FYN2_D_SM2')
where Code = 'CONSTB_DWS_CSAWS_SM2_D'





