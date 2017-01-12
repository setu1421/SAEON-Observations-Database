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
--> Changed 2.0.23 20170112 TimPN
--CREATE CLUSTERED INDEX [CX_OrganisationRole] ON [dbo].[OrganisationRole] ([AddedAt])
CREATE UNIQUE CLUSTERED INDEX [CX_OrganisationRole] ON [dbo].[OrganisationRole] ([AddedAt])
--< Changed 2.0.23 20170112 TimPN
--< Added 2.0.8 20160718 TimPN

GO
CREATE INDEX [IX_OrganisationRole_UserId] ON [dbo].[OrganisationRole] ([UserId])
--< Added 2.0.1 20160406 TimPN
--> Added 2.0.8 20160718 TimPN
--> Changed 2.0.15 20161102 TimPN
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
        OrganisationRole src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_OrganisationRole_Update] ON [dbo].[OrganisationRole]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
--> Changed 2.0.19 20161205 TimPN
--		AddedAt = del.AddedAt,
        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate ()),
--< Changed 2.0.19 20161205 TimPN
        UpdatedAt = GETDATE()
    from
        OrganisationRole src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160718 TimPN

