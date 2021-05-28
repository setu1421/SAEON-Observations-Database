Drop view if exists VUserDownloads
-- Smcri Programme/Projects cleanup
Select * from Programme where Description like 'SMCRI%'
Select * from Project inner join Programme on (Project.ProgrammeID = Programme.ID) where Programme.Description like 'SMCRI%'
--Delete Project from Project inner join Programme on (Project.ProgrammeID = Programme.ID) where Programme.Description like 'SMCRI%'
--Delete Programme from Programme where Description like 'SMCRI%'
