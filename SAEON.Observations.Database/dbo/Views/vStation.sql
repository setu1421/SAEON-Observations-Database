CREATE VIEW [dbo].[vStation]
AS
SELECT 
  Station.*,
--> Removed 20170123 TimPN
--  p.Code + ' - ' + p.Name ProjectSiteName,
--< Removed 20170123 TimPN
  s.Code SiteCode,
  s.Name SiteName
FROM 
  Station
--> Removed 20170123 TimPN
--  LEFT JOIN ProjectSite p -- Must be removed once all stations have sites
--    on (Station.ProjectSiteID = p.ID)
--< Removed 20170123 TimPN
--> Changed 20170123 TimPN
  INNER JOIN [Site] s 
--< Changed 20170123 TimPN
    on (Station.SiteID = s.ID)
