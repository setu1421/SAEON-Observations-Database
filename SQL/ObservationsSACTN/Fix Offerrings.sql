use Observations--SACTN
declare @PhenomenonID UniqueIdentifier = (Select ID
from Phenomenon
where Name = 'Water Temperature')
declare @OldPhenomenonOfferingID UniqueIdentifier = 
(
Select
  PhenomenonOffering.ID
from
  PhenomenonOffering
  inner join Offering
  on (PhenomenonOffering.OfferingID = Offering.ID)
where 
  (PhenomenonID = @PhenomenonID) and (Offering.Code = 'AVE')
)
declare @UserId UniqueIdentifier = (Select UserID
from aspnet_Users
where (UserName= 'TimPN'))
insert Offering
  (Code, Name, Description, UserID)
values
  ('AVE_H', 'Average Hourly', 'Average hourly water temperature', @UserId),
  --('AVE_D','Average Daily', 'Average daily water temperature', @UserId), --Already exists
  ('AVE_M', 'Average Monthly', 'Average montly water temperature', @UserId),
  ('AVE_Y', 'Average Yearly', 'Average yearly water temperature', @UserId)
insert PhenomenonOffering
  (PhenomenonID, OfferingID, UserId)
values
  (@PhenomenonID, (Select ID
    from Offering
    where Code = 'Ave_H'), @UserId),
  (@PhenomenonID, (Select ID
    from Offering
    where Code = 'Ave_D'), @UserId),
  (@PhenomenonID, (Select ID
    from Offering
    where Code = 'Ave_M'), @UserId),
  (@PhenomenonID, (Select ID
    from Offering
    where Code = 'Ave_Y'), @UserId)
-- Fix
Select *
from Sensor
where (Name like 'SACTN%') and (Name not like '%Temperature')
Select *
from DataSource
where (Name like 'SACTN%') and (Name not like '%Temperature')
Declare @OldName NVarChar(100) = 'tempearature'
Update
  Sensor
set
  Code = Replace(Code,@OldName,'Temperature'),
  Name = Replace(Name,@OldName,'Temperature'),
  Description = Replace(Description,@OldName,'Temperature')
where
  (Name like 'SACTN%') and (Name like '%'+@OldName)
Update
  DataSource
set
  Code = Replace(Code,@OldName,'Temperature'),
  Name = Replace(Name,@OldName,'Temperature'),
  Description = Replace(Description,@OldName,'Temperature')
where
  (Name like 'SACTN%') and (Name like '%'+@OldName)
set @OldName = 'tempeture'
Update
  Sensor
set
  Code = Replace(Code,@OldName,'Temperature'),
  Name = Replace(Name,@OldName,'Temperature'),
  Description = Replace(Description,@OldName,'Temperature')
where
  (Name like 'SACTN%') and (Name like '%'+@OldName)
Update
  DataSource
set
  Code = Replace(Code,@OldName,'Temperature'),
  Name = Replace(Name,@OldName,'Temperature'),
  Description = Replace(Description,@OldName,'Temperature')
where
  (Name like 'SACTN%') and (Name like '%'+@OldName)
set @OldName = 'temp'
Update
  Sensor
set
  Code = Replace(Code,@OldName,'Temperature'),
  Name = Replace(Name,@OldName,'Temperature'),
  Description = Replace(Description,@OldName,'Temperature')
where
  (Name like 'SACTN%') and (Name like '%'+@OldName)
Update
  DataSource
set
  --Code = Replace(Code,@OldName,'Temperature'), --Will be too long
  Name = Replace(Name,@OldName,'Temperature'),
  Description = Replace(Description,@OldName,'Temperature')
where
  (Name like 'SACTN%') and (Name like '%'+@OldName)
Select *
from Sensor
where (Name like 'SACTN%') and (Name not like '%Temperature')
Select *
from DataSource
where (Name like 'SACTN%') and (Name not like '%Temperature')
-- Hourly
declare @HourlyPhenomenonOfferingID UniqueIdentifier = 
(
Select
  PhenomenonOffering.ID
from
  PhenomenonOffering
  inner join Offering
  on (PhenomenonOffering.OfferingID = Offering.ID)
where
  (PhenomenonID = @PhenomenonID) and (Offering.Code = 'AVE_H')
)
Update
  SchemaColumn
set
  PhenomenonOfferingID = @HourlyPhenomenonOfferingID
from
  SchemaColumn
  inner join DataSchema
  on (SchemaColumn.DataSchemaID = DataSchema.ID)
where
  (DataSchema.Name like 'SACTN%') and (DataSchema.Name like '%Hourly') and (PhenomenonOfferingID = @OldPhenomenonOfferingID)
Update
  Observation
set
  PhenomenonOfferingID = @HourlyPhenomenonOfferingID
from
  Observation
  inner join Sensor
  on (Observation.SensorID = Sensor.ID)
where
  (Sensor.Name like 'SACTN%') and (Sensor.Name like '%Hourly Temperature') and (PhenomenonOfferingID = @OldPhenomenonOfferingID)
Update
  ImportBatchSummary
set
  PhenomenonOfferingID = @HourlyPhenomenonOfferingID
from
  ImportBatchSummary
  inner join Sensor
  on (ImportBatchSummary.SensorID = Sensor.ID)
where
  (Sensor.Name like 'SACTN%') and (Sensor.Name like '%Hourly Temperature') and (PhenomenonOfferingID = @OldPhenomenonOfferingID)
-- Daily
declare @DailyPhenomenonOfferingID UniqueIdentifier = 
(
Select
  PhenomenonOffering.ID
from
  PhenomenonOffering
  inner join Offering
  on (PhenomenonOffering.OfferingID = Offering.ID)
where
  (PhenomenonID = @PhenomenonID) and (Offering.Code = 'AVE_D')
)
Update
  SchemaColumn
set
  PhenomenonOfferingID = @DailyPhenomenonOfferingID
from
  SchemaColumn
  inner join DataSchema
  on (SchemaColumn.DataSchemaID = DataSchema.ID)
where
  (DataSchema.Name like 'SACTN%') and (DataSchema.Name like '%Daily') and (PhenomenonOfferingID = @OldPhenomenonOfferingID)
Update
  Observation
set
  PhenomenonOfferingID = @DailyPhenomenonOfferingID
from
  Observation
  inner join Sensor
  on (Observation.SensorID = Sensor.ID)
where
  (Sensor.Name like 'SACTN%') and (Sensor.Name like '%Daily Temperature%') and (PhenomenonOfferingID = @OldPhenomenonOfferingID)
Update
  ImportBatchSummary
set
  PhenomenonOfferingID = @DailyPhenomenonOfferingID
from
  ImportBatchSummary
  inner join Sensor
  on (ImportBatchSummary.SensorID = Sensor.ID)
where
  (Sensor.Name like 'SACTN%') and (Sensor.Name like '%Daily Temperature%') and (PhenomenonOfferingID = @OldPhenomenonOfferingID)
-- Monthly
declare @MonthlyPhenomenonOfferingID UniqueIdentifier = 
(
Select
  PhenomenonOffering.ID
from
  PhenomenonOffering
  inner join Offering
  on (PhenomenonOffering.OfferingID = Offering.ID)
where
  (PhenomenonID = @PhenomenonID) and (Offering.Code = 'AVE_M')
)
Update
  SchemaColumn
set
  PhenomenonOfferingID = @MonthlyPhenomenonOfferingID
from
  SchemaColumn
  inner join DataSchema
  on (SchemaColumn.DataSchemaID = DataSchema.ID)
where
  (DataSchema.Name like 'SACTN%') and (DataSchema.Name like '%Monthly') and (PhenomenonOfferingID = @OldPhenomenonOfferingID)
Update
  Observation
set
  PhenomenonOfferingID = @MonthlyPhenomenonOfferingID
from
  Observation
  inner join Sensor
  on (Observation.SensorID = Sensor.ID)
where
  (Sensor.Name like 'SACTN%') and (Sensor.Name like '%Monthly Temperature') and (PhenomenonOfferingID = @OldPhenomenonOfferingID)
Update
  ImportBatchSummary
set
  PhenomenonOfferingID = @MonthlyPhenomenonOfferingID
from
  ImportBatchSummary
  inner join Sensor
  on (ImportBatchSummary.SensorID = Sensor.ID)
where
  (Sensor.Name like 'SACTN%') and (Sensor.Name like '%Monthly Temperature') and (PhenomenonOfferingID = @OldPhenomenonOfferingID)
-- Yearly
declare @YearlyPhenomenonOfferingID UniqueIdentifier = 
(
Select
  PhenomenonOffering.ID
from
  PhenomenonOffering
  inner join Offering
  on (PhenomenonOffering.OfferingID = Offering.ID)
where
  (PhenomenonID = @PhenomenonID) and (Offering.Code = 'AVE_Y')
)
Update
  SchemaColumn
set
  PhenomenonOfferingID = @YearlyPhenomenonOfferingID
from
  SchemaColumn
  inner join DataSchema
  on (SchemaColumn.DataSchemaID = DataSchema.ID)
where
  (DataSchema.Name like 'SACTN%') and (DataSchema.Name like '%Annual') and (PhenomenonOfferingID = @OldPhenomenonOfferingID)
Update
  Observation
set
  PhenomenonOfferingID = @YearlyPhenomenonOfferingID
from
  Observation
  inner join Sensor
  on (Observation.SensorID = Sensor.ID)
where
  (Sensor.Name like 'SACTN%') and (Sensor.Name like '%Annual Temperature') and (PhenomenonOfferingID = @OldPhenomenonOfferingID)
Update
  ImportBatchSummary
set
  PhenomenonOfferingID = @YearlyPhenomenonOfferingID
from
  ImportBatchSummary
  inner join Sensor
  on (ImportBatchSummary.SensorID = Sensor.ID)
where
  (Sensor.Name like 'SACTN%') and (Sensor.Name like '%Annual Temperature') and (PhenomenonOfferingID = @OldPhenomenonOfferingID)
-- Check
Select distinct
  Sensor.Name
from
  Observation
  inner join PhenomenonOffering
  on (Observation.PhenomenonOfferingID = PhenomenonOffering.ID)
  inner join Offering
  on (PhenomenonOffering.OfferingID = Offering.ID)
  inner join Sensor
  on (Observation.SensorID = Sensor.ID)
where
  (Sensor.Name like 'SACTN%') and (Offering.Name = 'Average')

