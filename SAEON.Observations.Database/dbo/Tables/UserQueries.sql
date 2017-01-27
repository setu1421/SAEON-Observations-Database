--> Added 2.0.26 20170127 TimPN
Create Table [dbo].[UserQueries]
(
    [ID] UniqueIdentifier Constraint [DF_UserQueries_ID] Default NewId(), 
    [UserId] NVarChar(128) not Null,
    [Name] VarChar(150) not Null, 
    [Description] VarChar(500) Null,
    [QueryURI] VarChar(500) not Null,
    [AddedAt] DateTime null Constraint [DF_UserQueries_AddedAt] Default GetDate(),
    [AddedBy] NVarChar(128) not Null,
    [UpdatedAt] DateTime null Constraint [DF_UserQueries_UpdatedAt] Default GetDate(), 
    [UpdatedBy] NVarChar(128) not Null,
    Constraint [PK_UserQueries] Primary Key Clustered ([ID]),
    Constraint [FK_UserQueries_AspNetUsers_UserId] Foreign Key ([UserId]) References [dbo].[AspNetUsers] ([Id]),
    Constraint [FK_UserQueries_AspNetUsers_AddedBy] Foreign Key ([UserId]) References [dbo].[AspNetUsers] ([Id]),
    Constraint [FK_UserQueries_AspNetUsers_UpdatedBy] Foreign Key ([UserId]) References [dbo].[AspNetUsers] ([Id]),
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
