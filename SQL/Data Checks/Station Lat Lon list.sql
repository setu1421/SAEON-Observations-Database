use Observations;
Select Distinct
  Site.Code SiteCode, Site.Name SiteName, Station.Code StationCode, Station.Name StationName, 
  Station.Latitude, Station.Longitude, Station_Instrument.Latitude, Station_Instrument.Longitude
from 
  Station
  inner join Site
    on (Station.SiteID = Site.ID)
  inner join Station_Instrument
    on (Station_Instrument.StationID = Station.ID)
  inner join Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID)
where
  (Station.Code like 'ELW_%') and (Station.Code like '%_UTR013_PAO')
order by
  Site.Name, Station.Name