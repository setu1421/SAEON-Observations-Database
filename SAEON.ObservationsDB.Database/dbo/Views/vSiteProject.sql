--> Added 2.0.0.5 20160512 TimPN
CREATE VIEW [dbo].[vSiteProject]
AS
SELECT 
  src.*, 
  p.Name ProjectName,
  s.Name SiteName
FROM 
  [Site_Project] src
  inner join [Project] p
    on (src.ProjectID = p.ID)
  inner join [Site] s
    on (src.SiteID = s.ID)
--< Added 2.0.0.5 20160512 TimPN
