--> Added 2.0.0.5 20160511 TimPN
CREATE TABLE [dbo].[Programme]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Programme_ID] DEFAULT newid(), 
    [Code] VARCHAR(50) NOT NULL, 
    [Name] VARCHAR(150) NOT NULL, 
    [Description] VARCHAR(5000) NULL,
    [Url] VARCHAR(250) NULL, 
    [StartDate] DATETIME NULL, 
    [EndDate] DATETIME NULL, 
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Programme_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Programme_UpdatedAt] DEFAULT GetDate(), 
    CONSTRAINT [PK_Programme] PRIMARY KEY NONCLUSTERED ([ID]),
    CONSTRAINT [UX_Programme_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Programme_Name] UNIQUE ([Name]),
    CONSTRAINT [FK_Programme_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)
GO
CREATE CLUSTERED INDEX [CX_Programme] ON [dbo].[Programme] ([AddedAt])
GO
CREATE INDEX [IX_Programme_UserId] ON [dbo].[Programme] ([UserId])
GO
CREATE INDEX [IX_Programme_StartDate] ON [dbo].[Programme] ([StartDate])
GO
CREATE INDEX [IX_Programme_EndDate] ON [dbo].[Programme] ([EndDate])
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
        inserted ins 
        inner join Programme src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Programme_Update] ON [dbo].[Programme]
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
        inner join Programme src
            on (ins.ID = src.ID)
END
--< Added 2.0.0.5 20160511 TimPN
