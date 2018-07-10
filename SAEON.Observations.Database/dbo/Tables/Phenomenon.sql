CREATE TABLE [dbo].[Phenomenon] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_Phenomenon_ID] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Name]        VARCHAR (150)    NOT NULL,
    [Description] VARCHAR (5000)   NULL,
    [Url]         VARCHAR (250)    NULL,
    [UserId]      UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Phenomenon_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Phenomenon_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_Phenomenon] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_Phenomenon_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_Phenomenon_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Phenomenon_Name] UNIQUE ([Name])
);
GO
CREATE INDEX [IX_Phenomenon_UserId] ON [dbo].[Phenomenon] ([UserId])
GO
CREATE TRIGGER [dbo].[TR_Phenomenon_Insert] ON [dbo].[Phenomenon]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GetDate(),
        UpdatedAt = NULL
    from
        Phenomenon src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Phenomenon_Update] ON [dbo].[Phenomenon]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate()),
        UpdatedAt = GETDATE()
    from
        Phenomenon src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END

