--> Added 2.0.0.5 20160512 TimPN
CREATE VIEW [dbo].[vProgrammeProject]
AS
SELECT 
  src.*, 
  [Programme].Code ProgrammeCode,
  [Programme].Name ProgrammeName,
  [Project].Code ProjectCode,
  [Project].Name ProjectName
FROM 
  [Programme_Project] src
  inner join [Programme]
    on (src.ProgrammeID = [Programme].ID)
  inner join [Project]
    on (src.ProjectID = [Project].ID)
--< Added 2.0.0.5 20160512 TimPN
