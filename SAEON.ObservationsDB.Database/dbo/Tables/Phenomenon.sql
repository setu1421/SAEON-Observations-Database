CREATE TABLE [dbo].[Phenomenon] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_Phenomenon_ID] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Name]        VARCHAR (150)    NOT NULL,
    [Description] VARCHAR (5000)   NULL,
    [Url]         VARCHAR (250)    NULL,
    [UserId]      UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Phenomenon] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_Phenomenon_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_Phenomenon_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Phenomenon_Code] UNIQUE ([Code]),
--< Changed 20160329 TimPN
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_Phenomenon_Name] UNIQUE ([Name])
    CONSTRAINT [UX_Phenomenon_Name] UNIQUE ([Name])
--< Changed 20160329 TimPN
);
--> Added 2.0.0.0 20160406 TimPN
GO
CREATE INDEX [IX_Phenomenon_UserId] ON [dbo].[Phenomenon] ([UserId])
--< Added 2.0.0.0 20160406 TimPN
