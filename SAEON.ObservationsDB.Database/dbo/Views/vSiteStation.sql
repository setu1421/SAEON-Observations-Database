CREATE VIEW [dbo].[vSiteStation]
AS
SELECT 
  src.*, 
  [Site].Name SiteName,
  [Station].Name StationName
FROM 
  [Site_Station] src
  inner join [Site] 
    on (src.SiteID = [Site].ID)
  inner join [Station]
    on (src.StationID = [Station].ID)
