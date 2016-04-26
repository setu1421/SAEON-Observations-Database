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
CREATE TRIGGER [dbo].[TR_Site_InsertUpdate] ON [dbo].[Site]
FOR INSERT, UPDATE
AS
BEGIN
    SET NoCount ON
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
