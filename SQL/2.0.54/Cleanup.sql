declare @PhenomenonCode nvarchar(200) = ''
declare @OldOfferingCode nvarchar(200) = ''
declare @NewOfferingCode nvarchar(200) = ''
declare @PhenomenonID int = (select ID from Phenomenon where Code = @PhenomenonCode)
declare @OldOfferingID int = (select ID from Offering where Code = @OldOfferingCode)
declare @NewOfferingID int = (select ID from Offering where Code = @NewOfferingCode)