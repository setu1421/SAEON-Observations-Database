CREATE TABLE [dbo].[TransformationType] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_TransformationType_ID] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Name]        VARCHAR (150)    NOT NULL,
    [Description] VARCHAR (500)    NOT NULL,
    [iorder]      INT              NULL,
    [UserId] UNIQUEIDENTIFIER NULL, 
    [AddedAt] DATETIME NULL CONSTRAINT [DF_TransformationType_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_TransformationType_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_TransformationType] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [UX_TransformationType_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_TransformationType_Name] UNIQUE ([Name]),
    CONSTRAINT [FK_TransformationType_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
);
GO
CREATE INDEX [IX_TransformationType_CodeName] ON [dbo].[TransformationType] ([Code],[Name])
GO
CREATE INDEX [IX_TransformationType_UserId] ON [dbo].[TransformationType] ([UserId])
GO
CREATE TRIGGER [dbo].[TR_TransformationType_Insert] ON [dbo].[TransformationType]
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
        TransformationType src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_TransformationType_Update] ON [dbo].[TransformationType]
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
        TransformationType src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
