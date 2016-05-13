--> Added 2.0.0.5 20160512 TimPN
CREATE VIEW [dbo].[vSiteProject]
AS
SELECT 
  src.*, 
  [Site].Code SiteCode,
  [Site].Name SiteName,
  [Project].Code ProjectCode,
  [Project].Name ProjectName
FROM 
  [Site_Project] src
  inner join [Site]
    on (src.SiteID = [Site].ID)
  inner join [Project]
    on (src.ProjectID = [Project].ID)
--< Added 2.0.0.5 20160512 TimPN
