--> Added 2.0.1 20160406 TimPN
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
--> Added 2.0.3 20160421 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Site_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Site_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.3 20160421 TimPN
--> Changed 2.0.5 20160411 TimPN
--    CONSTRAINT [PK_Site] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [PK_Site] PRIMARY KEY NONCLUSTERED ([ID]),
--< Changed 2.0.5 20160411 TimPN
    CONSTRAINT [UX_Site_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Site_Name] UNIQUE ([Name]),
    CONSTRAINT [FK_Site_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)
--> Added 2.0.5 20160411 TimPN
GO
CREATE CLUSTERED INDEX [CX_Site] ON [dbo].[Site] ([AddedAt])
--< Added 2.0.5 20160411 TimPN
GO
CREATE INDEX [IX_Site_UserId] ON [dbo].[Site] ([UserId])
--< Added 2.0.1 20160406 TimPN

--> Added 2.0.3 20160421 TimPN
GO
CREATE INDEX [IX_Site_StartDate] ON [dbo].[Site] ([StartDate])
GO
CREATE INDEX [IX_Site_EndDate] ON [dbo].[Site] ([EndDate])
--> Changed 2.0.15 20161102 TimPN
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
        Site src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Site_Update] ON [dbo].[Site]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        Site src
        inner join inserted ins 
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.3 20160421 TimPN
