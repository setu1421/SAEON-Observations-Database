Update Project set ProgrammeID = (select ID from Programme where Name = 'SAEON') where ProgrammeID is null
alter table Project drop constraint UX_Project_ProgramID_Name
alter table Project drop constraint UX_Project_ProgramID_Code
drop index IX_Project_ProgrammeID on Project
alter table Project alter column ProgrammeID UniqueIdentifier not null
delete from UserDownloads