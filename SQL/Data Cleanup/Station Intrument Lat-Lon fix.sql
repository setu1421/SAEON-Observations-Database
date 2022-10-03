use Observations;
Declare @StationCode VarChar(200) = 'ELW_ABSS_CMP_SFB_UTR019_SFE80'
Declare @OldLatitude float = null -- -33.1240
Declare @OldLongitude float = null
Declare @NewLatitude float = -34.1240
Declare @NewLongitude float = 25.28967
Select
  Station_Instrument.Latitude, Station_Instrument.Longitude
from
  Station_Instrument 
  inner join Station
    on (Station_Instrument.StationID = Station.ID)
where
  (Station.Code = @StationCode) and
  ((@OldLatitude is null) or (Station_Instrument.Latitude = @OldLatitude)) and
  ((@OldLongitude is null) or (Station_Instrument.Longitude = @OldLongitude))
Update
  Station_Instrument
Set
  Latitude = Coalesce(@NewLatitude, Station_Instrument.Latitude),
  Longitude = Coalesce(@NewLongitude, Station_Instrument.Longitude)
from
  Station_Instrument 
  inner join Station
    on (Station_Instrument.StationID = Station.ID)
where
  (Station.Code = @StationCode) and
  ((@OldLatitude is null) or (Station_Instrument.Latitude = @OldLatitude)) and
  ((@OldLongitude is null) or (Station_Instrument.Longitude = @OldLongitude))
