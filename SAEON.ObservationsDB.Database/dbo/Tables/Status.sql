CREATE TABLE [dbo].[Status] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_Status_ID] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Name]        VARCHAR (150)    NOT NULL,
    [Description] VARCHAR (500)    NOT NULL,
--> Added 20160329 TimPN
    [UserId] UNIQUEIDENTIFIER NOT NULL, 
--< Added 20160329 TimPN
    CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED ([ID]),
--> Changed 20160329 TimPN
--	CONSTRAINT [IX_Status_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Status_Code] UNIQUE ([Code]),
--< Changed 20160329 TimPN
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_Status_Name] UNIQUE ([Name])
    CONSTRAINT [UX_Status_Name] UNIQUE ([Name]),
--< Changed 20160329 TimPN
--> Added 20160329 TimPN
    CONSTRAINT [FK_Status_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
--< Added 20160329 TimPN
);
--> Added 20160329 TimPN
GO
CREATE INDEX [IX_Status_UserId] ON [dbo].[Status] ([UserId])
--< Added 20160329 TimPN
