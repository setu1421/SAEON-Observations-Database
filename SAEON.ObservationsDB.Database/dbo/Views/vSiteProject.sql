--> Added 2.0.0.5 20160512 TimPN
CREATE VIEW [dbo].[vSiteProject]
AS
SELECT 
  src.*, 
  [Project].Name ProjectName,
  [Site].Name SiteName
FROM 
  [Site_Project] src
  inner join [Project]
    on (src.ProjectID = [Project].ID)
  inner join [Site]
    on (src.SiteID = [Site].ID)
--< Added 2.0.0.5 20160512 TimPN
