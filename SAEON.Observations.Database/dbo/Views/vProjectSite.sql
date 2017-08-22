--> Removed 2.0.34 TimPN 20170821
/*
CREATE VIEW [dbo].[vProjectSite]
AS

SELECT p.*,o.Code + ' - ' + o.Name as OrganisationName FROM ProjectSite p
 INNER JOIN Organisation o
	on p.OrganisationID = o.ID
*/
--< Removed 2.0.34 TimPN 20170821

