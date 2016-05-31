CREATE TABLE [dbo].[Offering] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_Offering_ID] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Name]        VARCHAR (150)    NOT NULL,
    [Description] VARCHAR (5000)   NULL,
    [UserId]      UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Offering] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_Offering_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_Offering] UNIQUE ([Name]),
    CONSTRAINT [UX_Offering_Name] UNIQUE ([Name]),
--< Changed 20160329 TimPN
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_Offering_Code] UNIQUE ([Code])
    CONSTRAINT [UX_Offering_Code] UNIQUE ([Code])
--< Changed 20160329 TimPN
);
--> Added 2.0.0 20160406 TimPN
GO
CREATE INDEX [IX_Offering_UserId] ON [dbo].[Offering] ([UserId])
--< Added 2.0.0 20160406 TimPN
