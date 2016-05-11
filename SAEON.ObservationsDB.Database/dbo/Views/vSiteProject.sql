CREATE VIEW [dbo].[vSiteProject]
AS
SELECT 
  src.*, p.Name ProjectName
FROM 
  [Site_Project] src
  inner join [Project] p
    on (src.ProjectID = p.ID)
