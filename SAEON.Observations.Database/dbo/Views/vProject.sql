--> Added 2.0.5 20160512 TimPN
CREATE VIEW [dbo].[vProject]
AS
SELECT 
  src.*, 
  [Programme].Code ProgrammeCode,
  [Programme].Name ProgrammeName
FROM 
  [Project] src
  left join [Programme]
    on (src.ProgrammeID = [Programme].ID)
--< Added 2.0.5 20160512 TimPN
