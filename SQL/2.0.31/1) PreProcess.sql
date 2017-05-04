Declare @Msg VarChar(100)
Set @Msg = Convert(varchar(100),GetDate(),113)+' Drop PK_Observation'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Alter table Observation drop constraint PK_Observation
Set @Msg = Convert(varchar(100),GetDate(),113)+' Drop CX_Observation'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Drop index CX_Observation on Observation
Set @Msg = Convert(varchar(100),GetDate(),113)+' Drop IX_Observation'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Drop index IX_Observation on Observation
Set @Msg = Convert(varchar(100),GetDate(),113)+' Drop IX_Observation_ImportBatchID'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Drop index IX_Observation_ImportBatchID on Observation
Set @Msg = Convert(varchar(100),GetDate(),113)+' Drop IX_Observation_ValueDate'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Drop index IX_Observation_ValueDate on Observation
Set @Msg = Convert(varchar(100),GetDate(),113)+' Drop IX_Observation_AddedDate'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Drop index IX_Observation_AddedDate on Observation
Set @Msg = Convert(varchar(100),GetDate(),113)+' Drop IX_Observation_SensorIDPhenomenonOfferingID'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Drop index IX_Observation_SensorIDPhenomenonOfferingID on Observation
Set @Msg = Convert(varchar(100),GetDate(),113)+' Drop DF_Observation_ID'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Alter table Observation drop constraint DF_Observation_ID
Set @Msg = Convert(varchar(100),GetDate(),113)+' Drop ID'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Alter table Observation drop column ID
--Set @Msg = Convert(varchar(100),GetDate(),113)+' Add ID'
--RAISERROR(@msg, 0, 1) WITH NOWAIT
--Alter table Observation add ID Int IDENTITY(1,1) not null;
--Set @Msg = Convert(varchar(100),GetDate(),113)+' Add PK_Observation'
--RAISERROR(@msg, 0, 1) WITH NOWAIT
--Alter table Observation add Constraint PK_Observation Primary Key Clustered (ID) on Observations
Set @Msg = Convert(varchar(100),GetDate(),113)+' Add ID and PK_Observation'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Alter table Observation add ID Int IDENTITY(1,1) not null Constraint PK_Observation Primary Key Clustered (ID) on Observations
Set @Msg = Convert(varchar(100),GetDate(),113)+' Add RowVersion'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Alter table Observation add RowVersion RowVersion not null
Set @Msg = Convert(varchar(100),GetDate(),113)+' Add ValueDay'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Alter table Observation Add ValueDay as Cast(ValueDate as Date)
Set @Msg = Convert(varchar(100),GetDate(),113)+' Done'
RAISERROR(@msg, 0, 1) WITH NOWAIT
Checkpoint
