use Observations;
Select
  Site.Code SiteCode, Site.Name SiteName, Station.Code, Station.Name, Station.Description, Station.StartDate, Station.EndDate, Latitude, Longitude
from 
  Station
  inner join Site
    on (Station.SiteID = Site.ID)
where
  (Site.Code not in ('ALGB','ALGBPP','ISAR','STFR','MOSB')) and
  (Station.Code not like 'SACTN%')
order by
  Site.Name, Station.Name


