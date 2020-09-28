CREATE TABLE [dbo].[UnitOfMeasure] (
    [ID]         UNIQUEIDENTIFIER CONSTRAINT [DF_UnitOfMeasure_ID] DEFAULT (newid()) NOT NULL,
    [Code]       VARCHAR (50)     NOT NULL,
    [Unit]       VARCHAR (100)    NOT NULL,
    [UnitSymbol] VARCHAR (20)     NOT NULL,
    [UserId]     UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_UnitOfMeasure_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_UnitOfMeasure_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_UnitOfMeasure] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_UnitOfMeasure_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_UnitOfMeasure_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_UnitOfMeasure_Unit] UNIQUE ([Unit])
);
GO
CREATE INDEX [IX_UnitOfMeasure_CodeName] ON [dbo].[UnitOfMeasure] ([Code],[Unit])
GO
CREATE INDEX [IX_UnitOfMeasure_UserId] ON [dbo].[UnitOfMeasure] ([UserId])
GO
CREATE TRIGGER [dbo].[TR_UnitOfMeasure_Insert] ON [dbo].[UnitOfMeasure]
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
        UnitOfMeasure src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_UnitOfMeasure_Update] ON [dbo].[UnitOfMeasure]
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
        UnitOfMeasure src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
