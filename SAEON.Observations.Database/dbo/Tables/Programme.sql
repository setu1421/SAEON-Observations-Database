--> Added 2.0.5 20160511 TimPN
CREATE TABLE [dbo].[Programme]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Programme_ID] DEFAULT newid(), 
    [Code] VARCHAR(50) NOT NULL, 
    [Name] VARCHAR(150) NOT NULL, 
    [Description] VARCHAR(5000) NULL,
    [Url] VARCHAR(250) NULL, 
--> Changed 2.0.22 20170111 TimPN
--    [StartDate]        DATETIME         NULL,
    [StartDate]        DATE         NULL,
--< Changed 2.0.22 20170111 TimPN
--> Changed 2.0.22 20170111 TimPN
--    [EndDate]        DATETIME         NULL,
    [EndDate]        DATE         NULL,
--< Changed 2.0.22 20170111 TimPN
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Programme_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Programme_UpdatedAt] DEFAULT GetDate(), 
    CONSTRAINT [PK_Programme] PRIMARY KEY NONCLUSTERED ([ID]),
    CONSTRAINT [UX_Programme_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Programme_Name] UNIQUE ([Name]),
    CONSTRAINT [FK_Programme_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)
GO
--> Changed 2.0.23 20170112 TimPN
--CREATE CLUSTERED INDEX [CX_Programme] ON [dbo].[Programme] ([AddedAt])
CREATE UNIQUE CLUSTERED INDEX [CX_Programme] ON [dbo].[Programme] ([AddedAt])
--< Changed 2.0.23 20170112 TimPN
GO
CREATE INDEX [IX_Programme_UserId] ON [dbo].[Programme] ([UserId])
GO
CREATE INDEX [IX_Programme_StartDate] ON [dbo].[Programme] ([StartDate])
GO
CREATE INDEX [IX_Programme_EndDate] ON [dbo].[Programme] ([EndDate])
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_Programme_Insert] ON [dbo].[Programme]
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
        Programme src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Programme_Update] ON [dbo].[Programme]
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
        Programme src
        inner join inserted ins 
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.5 20160511 TimPN
