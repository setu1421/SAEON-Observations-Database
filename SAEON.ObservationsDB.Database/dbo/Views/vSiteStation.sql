--> Added 2.0.0.5 20160512 TimPN
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
--< Added 2.0.0.5 20160512 TimPN
