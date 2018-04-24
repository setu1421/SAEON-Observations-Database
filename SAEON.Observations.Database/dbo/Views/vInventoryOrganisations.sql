--> Added 20170523 2.0.32 TimPN
CREATE VIEW [dbo].[vInventoryOrganisations]
AS 
Select
  Coalesce(InstrumentOrganisation.Name, StationOrganisation.Name, SiteOrganisation.Name)+'~'+IsNull(StatusName,'') SurrogateKey,
  --Station.ID StationID, PhenomenonOffering.ID PhenomenonOfferingID, 
  Coalesce(InstrumentOrganisation.Name, StationOrganisation.Name, SiteOrganisation.Name) Name, IsNull(StatusName,'No status') Status, 
  Count(*) Count, Min(DataValue) Minimum, Max(DataValue) Maximum, Avg(DataValue) Average, StDev(DataValue) StandardDeviation, Var(DataValue) Variance
from
  vObservationExpansion
  left join Organisation_Site
    on (Organisation_Site.SiteID = vObservationExpansion.SiteID) and
       ((Organisation_Site.StartDate is null) or (ValueDay >= Organisation_Site.StartDate )) and
       ((Organisation_Site.EndDate is null) or (ValueDay <= Organisation_Site.EndDate))
  left join Organisation SiteOrganisation
    on (Organisation_Site.OrganisationID = SiteOrganisation.ID)  		
  left join Organisation_Station
    on (Organisation_Station.StationID = vObservationExpansion.StationID) and
       ((Organisation_Station.StartDate is null) or (ValueDay >= Organisation_Station.StartDate )) and
       ((Organisation_Station.EndDate is null) or (ValueDay <= Organisation_Station.EndDate))
  left join Organisation StationOrganisation
    on (Organisation_Station.OrganisationID = vObservationExpansion.StationID)  		
  left join Organisation_Instrument
    on (Organisation_Instrument.InstrumentID = vObservationExpansion.InstrumentID) and
       ((Organisation_Instrument.StartDate is null) or (ValueDay >= Organisation_Instrument.StartDate )) and
       ((Organisation_Instrument.EndDate is null) or (ValueDay <= Organisation_Instrument.EndDate))
  left join Organisation InstrumentOrganisation
    on (Organisation_Instrument.OrganisationID = vObservationExpansion.InstrumentID)  		
where 
  (Coalesce(InstrumentOrganisation.Name, StationOrganisation.Name, SiteOrganisation.Name) is not null)
group by
  Coalesce(InstrumentOrganisation.Name, StationOrganisation.Name, SiteOrganisation.Name),
  StatusName
--< Added 20170523 2.0.32 TimPN

