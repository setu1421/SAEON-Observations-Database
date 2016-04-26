--> Added 2.0.0.1 20160406 TimPN
CREATE TABLE [dbo].[Site]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Site_ID] DEFAULT newid(), 
    [Code] VARCHAR(50) NOT NULL, 
    [Name] VARCHAR(150) NOT NULL, 
    [Description] VARCHAR(5000) NULL,
    [Url] VARCHAR(250) NULL, 
    [StartDate] DATETIME NULL, 
    [EndDate] DATETIME NULL, 
    [UserId] UNIQUEIDENTIFIER NOT NULL,
--> Added 2.0.0.3 20160421 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Site_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Site_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.0.3 20160421 TimPN
    CONSTRAINT [PK_Site] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [UX_Site_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Site_Name] UNIQUE ([Name]),
    CONSTRAINT [FK_Site_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)

GO
CREATE INDEX [IX_Site_UserId] ON [dbo].[Site] ([UserId])
--< Added 2.0.0.1 20160406 TimPN

--> Added 2.0.0.3 20160421 TimPN
GO
CREATE INDEX [IX_Site_StartDate] ON [dbo].[Site] ([StartDate])
GO
CREATE INDEX [IX_Site_EndDate] ON [dbo].[Site] ([EndDate])
GO
CREATE TRIGGER [dbo].[TR_Site_Insert] ON [dbo].[Site]
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
        inner join Site src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Site_Update] ON [dbo].[Site]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    if UPDATE(AddedAt) RAISERROR ('Cannot update AddedAt.', 16, 1)
    if UPDATE(UpdatedAt) RAISERROR ('Cannot update UpdatedAt.', 16, 1)
    if not UPDATE(AddedAt) and not UPDATE(UpdatedAt)
        Update 
            src 
        set 
            UpdatedAt = GETDATE()
        from
            inserted ins 
            inner join Site src
                on (ins.ID = src.ID)
END
--< Added 2.0.0.3 20160421 TimPN
