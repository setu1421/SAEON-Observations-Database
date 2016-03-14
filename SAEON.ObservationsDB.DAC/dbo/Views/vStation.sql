CREATE VIEW [dbo].[vStation]
AS

SELECT s.*,p.Code + ' - ' + p.Name as ProjectSiteName FROM Station s
 INNER JOIN ProjectSite p
	on s.ProjectSiteID = p.ID
