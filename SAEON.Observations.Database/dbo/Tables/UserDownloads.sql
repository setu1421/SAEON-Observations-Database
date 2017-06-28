﻿--> Added 2.0.26 20170127 TimPN
Create Table [dbo].[UserDownloads]
(
    [ID] UniqueIdentifier Constraint [DF_UserDownloads_ID] Default NewId(), 
    [UserId] NVarChar(128) not Null,
    [Name] VarChar(150) not Null, 
    [Description] VarChar(5000) Null,
--> Changed 2.0.31 20170423 TimPN
--    [QueryURI] VarChar(500) not Null,
    [QueryInput] VarChar(5000) not Null,
--< Changed 2.0.31 20170423 TimPN
    [DownloadURI] VarChar(500) not Null,
    [AddedAt] DateTime null Constraint [DF_UserDownloads_AddedAt] Default GetDate(),
    [AddedBy] NVarChar(128) not Null,
    [UpdatedAt] DateTime null Constraint [DF_UserDownloads_UpdatedAt] Default GetDate(), 
    [UpdatedBy] NVarChar(128) not Null,
--> Added 2.0.33 20170628 TimPN
    [RowVersion] RowVersion not null,
--< Added 2.0.33 20170628 TimPN
    Constraint [PK_UserDownloads] Primary Key Clustered ([ID]),
--> Removed 20170301
--    Constraint [FK_UserDownloads_AspNetUsers_UserId] Foreign Key ([UserId]) References [dbo].[AspNetUsers] ([Id]),
--    Constraint [FK_UserDownloads_AspNetUsers_AddedBy] Foreign Key ([UserId]) References [dbo].[AspNetUsers] ([Id]),
--    Constraint [FK_UserDownloads_AspNetUsers_UpdatedBy] Foreign Key ([UserId]) References [dbo].[AspNetUsers] ([Id]),
--< Removed 20170301
    Constraint [UX_UserDownloads_UserId_Name] Unique ([UserId],[Name])
)
GO
CREATE TRIGGER [dbo].[TR_UserDownloads_Insert] ON [dbo].[UserDownloads]
FOR INSERT
AS
BEGIN
  SET NoCount ON
  Update
      src
  set
      AddedAt = GetDate(),
      UpdatedAt = NULL
  from
    UserDownloads src
    inner join inserted ins
      on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_UserDownloads_Update] ON [dbo].[UserDownloads]
FOR UPDATE
AS
BEGIN
  SET NoCount ON
  Update
      src
  set
    AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate ()),
    UpdatedAt = GetDate ()
  from
    UserDownloads src
    inner join inserted ins
      on (ins.ID = src.ID)
    inner join deleted del
      on (del.ID = src.ID)
END
--< Added 2.0.26 20170127 TimPN
