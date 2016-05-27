--> Added 2.0.0.5 20160527 TimPN
CREATE VIEW [dbo].[vStationProject]
AS
SELECT 
  src.*, 
  [Station].Code StationCode,
  [Station].Name StationName,
  [Project].Code ProjectCode,
  [Project].Name ProjectName
FROM 
  [Station_Project] src
  inner join [Station]
    on (src.StationID = [Station].ID)
  inner join [Project]
    on (src.ProjectID = [Project].ID)
--< Added 2.0.0.5 20160527 TimPN
