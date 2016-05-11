CREATE VIEW [dbo].[vStation]
AS
SELECT 
  Station.*,
  p.Code + ' - ' + p.Name ProjectSiteName
--> Removed 2.0.0.5 20160411 TimPN
--  s.Code + ' - ' + s.Name SiteName
--< Removed 2.0.0.5 20160411 TimPN
FROM 
  Station
  LEFT JOIN ProjectSite p -- Must be removed once all stations have sites
    on (Station.ProjectSiteID = p.ID)
--> Removed 2.0.0.5 20160411 TimPN
--  LEFT JOIN [Site] s -- Must be inner join once all stations have sites
--    on (Station.SiteID = s.ID)
--< Removed 2.0.0.5 20160411 TimPN
