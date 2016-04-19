CREATE VIEW [dbo].[vStation]
AS
SELECT 
  Station.*,
  p.Code + ' - ' + p.Name ProjectSiteName,
  s.Code + ' - ' + s.Name SiteName
FROM 
  Station
  INNER JOIN ProjectSite p
    on (Station.ProjectSiteID = p.ID)
  LEFT JOIN [Site] s -- Must be inner join once all stations have sites
    on (Station.SiteID = s.ID)
