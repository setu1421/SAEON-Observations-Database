CREATE VIEW [dbo].[vStation]
AS
SELECT 
  Station.*,
  s.Code SiteCode,
  s.Name SiteName
FROM 
  Station
  INNER JOIN [Site] s 
    on (Station.SiteID = s.ID)
