CREATE VIEW [dbo].[vDatasetsExpansion]
AS
Select Distinct
  Datasets.*,
  Organisation.ID OrganisationID, Organisation.Code OrganisationCode, Organisation.Name OrganisationName, Organisation.Description OrganisationDescription, Organisation.Url OrganisationUrl,
  Programme.ID ProgrammeID, Programme.Code ProgrammeCode, Programme.Name ProgrammeName, Programme.Description ProgrammeDescription, Programme.Url ProgrammeUrl,
  Project.ID ProjectID, Project.Code ProjectCode, Project.Name ProjectName, Project.Description ProjectDescription, Project.Url ProjectUrl,
  Site.ID SiteID, Site.Code SiteCode, Site.Name SiteName, Site.Description SiteDescription, Site.Url SiteUrl,
  Station.Code StationCode, Station.Name StationName, Station.Description StationDescription, Station.Url StationUrl,
  Phenomenon.ID PhenomenonID, Phenomenon.Code PhenomenonCode, Phenomenon.Name PhenomenonName, Phenomenon.Description PhenomenonDescription, Phenomenon.Url PhenomenonUrl,
  Offering.ID OfferingID, Offering.Code OfferingCode, Offering.Name OfferingName, Offering.Description OfferingDescription,
  UnitOfMeasure.ID UnitOfMeasureID, UnitOfMeasure.Code UnitOfMeasureCode, UnitOfMeasure.Unit UnitOfMeasureUnit, UnitOfMeasure.UnitSymbol UnitOfMeasureSymbol
from
  Datasets
  inner join Station
    on (Datasets.StationID  = Station.ID)
  inner join Site
    on (Station.SiteID = Site.ID)
  inner join PhenomenonOffering
    on (Datasets.PhenomenonOfferingID = PhenomenonOffering.ID)
  inner join Phenomenon
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
  inner join PhenomenonUOM
    on (Datasets.PhenomenonUOMID = PhenomenonUOM.ID)
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
where
  (VerifiedCount > 0) and (LatitudeNorth is not null) and (LatitudeSouth is not null) and (LongitudeWest is not null) and (LongitudeEast is not null)
