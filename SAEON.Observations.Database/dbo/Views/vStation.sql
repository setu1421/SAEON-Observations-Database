CREATE VIEW [dbo].[vStation]
AS
SELECT 
  Station.*,
  s.Code SiteCode,
  s.Name SiteName
FROM 
  Station
  LEFT JOIN [Site] s 
    on (Station.SiteID = s.ID)
