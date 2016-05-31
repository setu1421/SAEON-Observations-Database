--> Added 2.0.5 20160527 TimPN
CREATE VIEW [dbo].[vProjectStation]
AS
SELECT 
  src.*, 
  [Project].Code ProjectCode,
  [Project].Name ProjectName,
  [Station].Code StationCode,
  [Station].Name StationName
FROM 
  [Project_Station] src
  inner join [Project]
    on (src.ProjectID = [Project].ID)
  inner join [Station]
    on (src.StationID = [Station].ID)
--< Added 2.0.5 20160527 TimPN
