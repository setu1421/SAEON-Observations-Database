--> Added 2.0.1 20160406 TimPN
CREATE TABLE [dbo].[OrganisationRole]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_OrganisationRole_ID] DEFAULT newid(), 
    [Code] VARCHAR(50) NOT NULL, 
    [Name] VARCHAR(150) NOT NULL, 
    [Description] VARCHAR(5000) NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
--> Added 2.0.8 20160718 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_OrganisationRole_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_OrganisationRole_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.8 20160718 TimPN
--> Changed 2.0.8 20160718 TimPN
--    CONSTRAINT [PK_OrganisationRole] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [PK_OrganisationRole] PRIMARY KEY NONCLUSTERED ([ID]),
--< Changed 2.0.8 20160718 TimPN
    CONSTRAINT [UX_OrganisationRole_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_OrganisationRole_Name] UNIQUE ([Name]),
    CONSTRAINT [FK_OrganisationRole_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)
--> Added 2.0.8 20160718 TimPN
GO
CREATE CLUSTERED INDEX [CX_OrganisationRole] ON [dbo].[OrganisationRole] ([AddedAt])
--< Added 2.0.8 20160718 TimPN

GO
CREATE INDEX [IX_OrganisationRole_UserId] ON [dbo].[OrganisationRole] ([UserId])
--< Added 2.0.1 20160406 TimPN
--> Added 2.0.8 20160718 TimPN
GO
CREATE TRIGGER [dbo].[TR_OrganisationRole] ON [dbo].[OrganisationRole]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        inserted ins
        inner join OrganisationRole src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_OrganisationRole_Update] ON [dbo].[OrganisationRole]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    --if UPDATE(AddedAt) RAISERROR ('Cannot update AddedAt.', 16, 1)
    Update
        src
    set
        UpdatedAt = GETDATE()
    from
        inserted ins
        inner join OrganisationRole src
            on (ins.ID = src.ID)
END
--< Added 2.0.8 20160718 TimPN

