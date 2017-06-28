﻿--> Added 2.0.26 20170127 TimPN
Create Table [dbo].[UserQueries]
(
    [ID] UniqueIdentifier Constraint [DF_UserQueries_ID] Default NewId(), 
    [UserId] NVarChar(128) not Null,
    [Name] VarChar(150) not Null, 
    [Description] VarChar(500) Null,
--> Changed 2.0.31 20170423 TimPN
--    [QueryURI] VarChar(500) not Null,
    [QueryInput] VarChar(5000) not Null,
--< Changed 2.0.31 20170423 TimPN
    [AddedAt] DateTime null Constraint [DF_UserQueries_AddedAt] Default GetDate(),
    [AddedBy] NVarChar(128) not Null,
    [UpdatedAt] DateTime null Constraint [DF_UserQueries_UpdatedAt] Default GetDate(), 
    [UpdatedBy] NVarChar(128) not Null,
--> Added 2.0.33 20170628 TimPN
    [RowVersion] RowVersion not null,
--< Added 2.0.33 20170628 TimPN
    Constraint [PK_UserQueries] Primary Key Clustered ([ID]),
--> Removed 20170301
--    Constraint [FK_UserQueries_AspNetUsers_UserId] Foreign Key ([UserId]) References [dbo].[AspNetUsers] ([Id]),
--    Constraint [FK_UserQueries_AspNetUsers_AddedBy] Foreign Key ([UserId]) References [dbo].[AspNetUsers] ([Id]),
--    Constraint [FK_UserQueries_AspNetUsers_UpdatedBy] Foreign Key ([UserId]) References [dbo].[AspNetUsers] ([Id]),
--< Removed 20170301
    Constraint [UX_UserQueries_UserId_Name] Unique ([UserId],[Name])
)
GO
CREATE TRIGGER [dbo].[TR_UserQueries_Insert] ON [dbo].[UserQueries]
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
    UserQueries src
    inner join inserted ins
      on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_UserQueries_Update] ON [dbo].[UserQueries]
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
    UserQueries src
    inner join inserted ins
      on (ins.ID = src.ID)
    inner join deleted del
      on (del.ID = src.ID)
END
--< Added 2.0.26 20170127 TimPN
