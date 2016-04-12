CREATE TABLE [dbo].[Organisation] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_Table_1_Organisation] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Name]        VARCHAR (150)    NOT NULL,
    [Description] VARCHAR (5000)   NULL,
--> Added 20160406 TimPN
    [Url] VARCHAR(250) NULL, 
--< Added 20160406 TimPN
    [UserId]      UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Organisation] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_Organisation_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_Organisation_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Organisation_Code] UNIQUE ([Code]),
--< Changed 20160329 TimPN
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_Organisation_Name] UNIQUE ([Name])
    CONSTRAINT [UX_Organisation_Name] UNIQUE ([Name])
--< Changed 20160329 TimPN
);
--> Added 20160329 TimPN
GO
CREATE INDEX [IX_Organisation_UserId] ON [dbo].[Organisation] ([UserId])
--< Added 20160329 TimPN
