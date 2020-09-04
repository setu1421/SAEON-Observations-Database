CREATE VIEW [dbo].[vImportBatchSummary]
AS 
Select
  ImportBatchSummary.*, 
 -- case when Exists(
	--Select 
	--  * 
	--from 
	--  Observation
	--  inner join Status
	--    on (Observation.StatusID = Status.ID)
 --   where 
 --     ((Observation.ImportBatchID = ImportBatchSummary.ImportBatchID) and 
	--   (Observation.SensorID = ImportBatchSummary.SensorID) and
 --      (Observation.PhenomenonOfferingID = ImportBatchSummary.PhenomenonOfferingID) and
 --      (Observation.PhenomenonUOMID = ImportBatchSummary.PhenomenonUOMID) and
	--   (Status.Name = 'Verified'))
 -- ) then 1
 -- else 0
 -- end HasVerified, 
 -- (
 -- Select 
 --   Count(*) 
 -- from 
 --   Observation
	--inner join Status
	--  on (Observation.StatusID = Status.ID)
 -- where 
 --   ((Observation.ImportBatchID = ImportBatchSummary.ImportBatchID) and 
	-- (Observation.SensorID = ImportBatchSummary.SensorID) and
	-- (Observation.PhenomenonOfferingID = ImportBatchSummary.PhenomenonOfferingID) and
	-- (Observation.PhenomenonUOMID = ImportBatchSummary.PhenomenonUOMID) and
	-- (Status.Name = 'Verified'))
 -- ) Verifed,
  Phenomenon.ID PhenomenonID, Phenomenon.Code PhenomenonCode, Phenomenon.Name PhenomenonName, Phenomenon.Description PhenomenonDescription, Phenomenon.Url PhenomenonUrl,
  OfferingID OfferingID, Offering.Code OfferingCode, Offering.Name OfferingName, Offering.Description OfferingDescription, 
  UnitOfMeasureID, UnitOfMeasure.Code UnitOfMeasureCode, UnitOfMeasure.Unit UnitOfMeasureUnit, UnitOfMeasure.UnitSymbol UnitOfMeasureSymbol,
  Sensor.Code SensorCode, Sensor.Name SensorName, Sensor.Description SensorDescription,  Sensor.Url SensorUrl,
  Instrument.Code InstrumentCode, Instrument.Name InstrumentName, Instrument.Description InstrumentDescription, Instrument.Url InstrumentUrl,
  Station.Code StationCode, Station.Name StationName, Station.Description StationDescription, Station.Url StationUrl,
  Site.Code SiteCode, Site.Name SiteName, Site.Description SiteDescription, Site.Url SiteUrl,
  Project.ID ProjectID, Project.Code ProjectCode, Project.Name ProjectName, Project.Description ProjectDescription, Project.Url ProjectUrl,
  Programme.ID ProgrammeID, Programme.Code ProgrammeCode, Programme.Name ProgrammeName, Programme.Description ProgrammeDescription, Programme.Url ProgrammeUrl,
  Organisation.ID OrganisationID, Organisation.Code OrganisationCode, Organisation.Name OrganisationName, Organisation.Description OrganisationDescription, Organisation.Url OrganisationUrl
From
  ImportBatchSummary
  inner join Sensor
    on (ImportBatchSummary.SensorID = Sensor.ID)
  inner join Instrument
    on (ImportBatchSummary.InstrumentID = Instrument.ID)
  inner join Station
    on (ImportBatchSummary.StationID = Station.ID)
  inner join Site
    on (ImportBatchSummary.SiteID = Site.ID)
  inner join PhenomenonOffering
    on (ImportBatchSummary.PhenomenonOfferingID = PhenomenonOffering.ID)
  inner join Phenomenon
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
  inner join PhenomenonUOM
    on (ImportBatchSummary.PhenomenonUOMID = PhenomenonUOM.ID)
  inner join UnitOfMeasure
    on (PhenomenonUOM.UnitOfMeasureID = UnitOfMeasure.ID)
  left join Project_Station
    on (Project_Station.StationID = Station.ID)
  left join Project
    on (Project_Station.ProjectID = Project.ID)
  left join Programme
    on (Project.ProgrammeID = Programme.ID)
  left join vStationOrganisation
    on (vStationOrganisation.StationID = Station.ID)
  left join Organisation
    on (vStationOrganisation.OrganisationID = Organisation.ID)
