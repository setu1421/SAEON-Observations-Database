CREATE TABLE [dbo].[Site]
(
    [ID] UNIQUEIDENTIFIER NOT NULL DEFAULT newid(), 
    [Code] VARCHAR(50) NOT NULL, 
    [Name] VARCHAR(150) NOT NULL, 
    [Description] VARCHAR(5000) NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Site] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_Site_Name] UNIQUE ([Name] ASC),
    CONSTRAINT [IX_Site_Code] UNIQUE ([Code] ASC),
    CONSTRAINT [FK_Site_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)

