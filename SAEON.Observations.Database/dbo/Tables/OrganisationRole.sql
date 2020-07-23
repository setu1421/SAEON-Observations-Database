CREATE TABLE [dbo].[OrganisationRole]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_OrganisationRole_ID] DEFAULT (newid()), 
    [Code] VARCHAR(50) NOT NULL, 
    [Name] VARCHAR(150) NOT NULL, 
    [Description] VARCHAR(5000) NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_OrganisationRole_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_OrganisationRole_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_OrganisationRole] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [UX_OrganisationRole_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_OrganisationRole_Name] UNIQUE ([Name]),
    CONSTRAINT [FK_OrganisationRole_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)
GO
CREATE INDEX [IX_OrganisationRole_CodeName] ON [dbo].[OrganisationRole] ([Code],[Name])
GO
CREATE INDEX [IX_OrganisationRole_UserId] ON [dbo].[OrganisationRole] ([UserId])
GO
CREATE TRIGGER [dbo].[TR_OrganisationRole] ON [dbo].[OrganisationRole]
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
        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate()),
        UpdatedAt = GETDATE()
    from
        OrganisationRole src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
