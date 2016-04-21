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
    [AddedAt] DATETIME NULL DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL DEFAULT GetDate(), 
    CONSTRAINT [PK_Site] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [UX_Site_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Site_Name] UNIQUE ([Name]),
    CONSTRAINT [FK_Site_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)

GO
CREATE INDEX [IX_Site_UserId] ON [dbo].[Site] ([UserId])
--< Added 2.0.0.1 20160406 TimPN

GO
--> Added 2.0.0.3 20160421 TimPN

CREATE TRIGGER [dbo].[TR_Site_Insert] ON [dbo].[Site]
FOR INSERT
AS
BEGIN
    SET NoCount ON
	Update 
		Site 
	set 
		AddedAt = GETDATE()
	from
		inserted i 
		inner join Site s
			on (i.ID = s.ID)
END