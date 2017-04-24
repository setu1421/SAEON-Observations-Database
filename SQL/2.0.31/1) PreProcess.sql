Declare @Msg VarChar(100)
Set @Msg = Convert(varchar(100),GetDate(),113)+' Drop PK_Observation'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Alter table Observation drop constraint PK_Observation
Set @Msg = Convert(varchar(100),GetDate(),113)+' Drop CX_Observation'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Drop index CX_Observation on Observation
Set @Msg = Convert(varchar(100),GetDate(),113)+' Drop DF_Observation_ID'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Alter table Observation drop constraint DF_Observation_ID
Set @Msg = Convert(varchar(100),GetDate(),113)+' Drop ID'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Alter table Observation drop column ID
Set @Msg = Convert(varchar(100),GetDate(),113)+' Create ObservationsSequence'
RAISERROR(@msg, 0, 1) WITH NOWAIT
CREATE SEQUENCE [dbo].[ObservationsSequence]
    AS INT
    START WITH 1
    INCREMENT BY 1;
Set @Msg = Convert(varchar(100),GetDate(),113)+' Add ID'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Alter table Observation add ID Int Constraint DF_Observation_ID Default (Next Value for ObservationsSequence) not null
Set @Msg = Convert(varchar(100),GetDate(),113)+' Add PK_Observation'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Alter table Observation add Constraint PK_Observation Primary Key Clustered (ID)
Set @Msg = Convert(varchar(100),GetDate(),113)+' Add RowVersion'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Alter table Observation add RowVersion RowVersion not null
Set @Msg = Convert(varchar(100),GetDate(),113)+' Add ValueDay'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Alter table Observation Add ValueDay as Cast(ValueDate as Date)
Set @Msg = Convert(varchar(100),GetDate(),113)+' Done'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Checkpoint
