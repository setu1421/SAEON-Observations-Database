use ObservationsSACTN
Update Instrument set Latitude = null, Longitude = null, Elevation = null where Code like 'SACTN%'
Update Observation set Latitude = null where Latitude is not null
Update Observation set Longitude = null where Longitude is not null
Update Observation set Elevation = null where Elevation is not null
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
Update Station set Code = 'SACTN Winkelspruit KZNSB', Name = Code, Description = 'Winkelspruit KZNSB' where Code = 'SACTN Winkelspriut KZNSB'
