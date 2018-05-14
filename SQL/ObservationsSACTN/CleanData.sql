use ObservationsSACTN
Update Instrument set Latitude = null, Longitude = null, Elevation = null where Code like 'SACTN%'
--Update Observation set Latitude = null where Latitude is not null
--Update Observation set Longitude = null where Longitude is not null
--Update Observation set Elevation = null where Elevation is not null
Update Station
Set Latitude = -34.024482, Longitude = 23.899327
where (Code = 'SACTN Storms River Mouth SAWS')
Update Station
Set Latitude = -Abs(Latitude)
where (Code = 'SACTN Antsey''s Beach KZNSB')
Update Station
Set Latitude = -27.424722
where (Code = 'SACTN Sodwana DEA')
Update Station
Set Latitude = -34.03601
where (Code = 'SACTN Noordhoek SAEON')
Update Station
Set Longitude = 25.34978
where (Code = 'SACTN Seaview SAEON')
Update Station
Set Latitude = -30.91621653
where (Code = 'SACTN Southbroom SAWS')
Update Station
Set Longitude = 26.901741
where (Code = 'SACTN Port Alfred SAWS')
Update Station
Set Latitude = -34.13525212
where (Code = 'SACTN Fish Hoek SAWS')
Update Station
Set Latitude = -34.319517
where (Code = 'SACTN Bordjies DAFF')
Update Station
Set Latitude = -33.984972
where (Code = 'SACTN Oudekraal DAFF')
Update
  Station
Set
  SiteID = (Select ID from Site where Name = 'SACTN Dyer Island')
from
  Station 
  inner join Site
    on (Station.SiteID = Site.ID)
where
  (Station.Name = 'SACTN Dyer Island DEA')
Update
  Station
Set
  SiteID = (Select ID from Site where Name = 'SACTN Seaview')
from
  Station 
  inner join Site
    on (Station.SiteID = Site.ID)
where
  (Station.Name = 'SACTN Seaview SAEON')

Declare @Code VarChar(500)
--Stations
set @Code = 'SACTN Winkelspruit KZNSB'
Update Station set Code = @Code, Name = @Code, Description = SUBSTRING(@Code,7, 500) where Name = 'SACTN Winkelspriut KZNSB'
set @Code = 'SACTN Kenton on Sea SAEON'
Update Station set Code = @Code, Name = @Code, Description = SUBSTRING(@Code,7, 500) where Code = 'SACTN Kenton on Sea UTR'

-- Instruments
set @Code = 'SACTN Aliwal Shoal DEA UTR'
Update Instrument set Code = @Code, Name = @Code, Description = SUBSTRING(@Code,7, 500) where Name = 'SACTN Aliwal Shaol DEA UTR'
set @Code = 'SACTN Dassen Island SAWS Thermo'
Update Instrument set Code = @Code, Name = @Code, Description = SUBSTRING(@Code,7, 500) where Code = 'SACTN Dassen IsLand SAWS Thermo'
set @Code = 'SACTN Ystervarkpunt DEA UTR'
Update Instrument set Code = @Code, Name = @Code, Description = SUBSTRING(@Code,7, 500) where Code = 'SACTN Yservarpunt DEA UTR'
set @Code = 'SACTN Park Rynie KZNSB Thermo'
Update Instrument set Code = @Code, Name = @Code, Description = SUBSTRING(@Code,7, 500) where Code = 'SACTN Park Rynien KZNSB Thermo'
set @Code = 'SACTN St Michaels KZNSB Thermo'
Update Instrument set Code = @Code, Name = @Code, Description = SUBSTRING(@Code,7, 500) where Code = 'SACTN St Machaels KZNSB Thermo'
set @Code = 'SACTN Winkelspruit KZNSB Thermo'
Update Instrument set Code = @Code, Name = @Code, Description = SUBSTRING(@Code,7, 500) where Code = 'SACTN Winkelspriut KZNSB Thermo'
set @Code = 'SACTN Gordons Bay SAWS Thermo'
Update Instrument set Code = @Code, Name = @Code, Description = SUBSTRING(@Code,7, 500) where Code = 'SACTN Gordons Bay DAFF Thermo'
set @Code = 'SACTN Kenton on Sea SAEON UTR'
Update Instrument set Code = @Code, Name = @Code, Description = SUBSTRING(@Code,7, 500) where Code = 'SACTN Kenton on Sea UTR'

-- StationInstruments
Delete 
  Station_Instrument 
from
  Station_Instrument
  inner join Station
    on (Station_Instrument.StationID = Station.ID)
  inner join Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID)
where
  (Station.Code = 'SACTN Sea Point SAWS') and (Instrument.Code = 'SACTN Seaview SAEON UTR')
Insert Station_Instrument
  (StationID, InstrumentID, UserId)
values
  ((Select ID from Station where Code = 'SACTN Seaview SAEON'), (Select ID from Instrument where Code = 'SACTN Seaview SAEON UTR'), (Select UserId from aspnet_Users where LoweredUserName='timpn'))
-- Sensors
set @Code = 'SACTN Blythedale KZNSB Thermo Annual Temperature'
Update Sensor set Code = @Code, Name = @Code, Description = SUBSTRING(@Code,7, 500) where Code = 'SACTN Bylthedale KZNSB Thermo Annual Temperature'
set @Code = 'SACTN Blythedale KZNSB Thermo Daily Temperature'
Update Sensor set Code = @Code, Name = @Code, Description = SUBSTRING(@Code,7, 500) where Code = 'SACTN Bylthedale KZNSB Thermo Daily Temperature'
set @Code = 'SACTN Blythedale KZNSB Thermo Monthly Temperature'
Update Sensor set Code = @Code, Name = @Code, Description = SUBSTRING(@Code,7, 500) where Code = 'SACTN Bylthedale KZNSB Thermo Monthly Temperature'
set @Code = 'SACTN Gordons Bay SAWS Thermo Daily Temperature'
Update Sensor set Code = @Code, Name = @Code, Description = SUBSTRING(@Code,7, 500) where Code = 'SACTN Gordon''s Bay SAWS Thermo Daily Temperature'
set @Code = 'SACTN Humewood SAWS Thermo Monthly Temperature'
Update Sensor set Code = @Code, Name = @Code, Description = SUBSTRING(@Code,7, 500) where Code = 'SACTN Humehood SAWS Thermo Monthly Temperature'



