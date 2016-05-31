CREATE VIEW [dbo].[vStation]
AS
SELECT 
  Station.*,
  p.Code + ' - ' + p.Name ProjectSiteName,
  s.Code + ' - ' + s.Name SiteName
FROM 
  Station
  LEFT JOIN ProjectSite p -- Must be removed once all stations have sites
    on (Station.ProjectSiteID = p.ID)
  LEFT JOIN [Site] s 
    on (Station.SiteID = s.ID)
