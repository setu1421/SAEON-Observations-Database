Select
  (Select count(*) from Phenomenon) Phenomena,
  (Select count(*) from Offering) Offerings,
  (Select count(*) from UnitOfMeasure) UnitsOfMeasure,
  (Select count(*) from PhenomenonOffering) PhenomenaOfferings,
  (Select count(*) from PhenomenonUOM) PhenomenaUnitsOfMeasure,
  (Select count(*) from DataSchema) DataSchemas,
  (Select count(*) from DataSource) DataSources,
  (Select count(*) from Sensor) Sensors,
  (Select count(PhenomenonID) from Sensor) SensorPhenomena,
  (Select count(*) from ImportBatch) ImportBatches,
  (Select count(*) from Observation) Observations,
  (Select count(PhenomenonOfferingID) from Observation) ObservationsOfferings,
  (Select count(PhenomenonUOMID) from Observation) ObservationsUnitsOfMeasure
Select
  Offering.Name, UnitOfMeasure.Unit, count(*) Count
from
  Observation
  inner join PhenomenonOffering
    on (Observation.PhenomenonOfferingID = PhenomenonOffering.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
  inner join PhenomenonUOM
    on (Observation.PhenomenonUOMID = PhenomenonUOM.ID)
  inner join UnitOfMeasure
    on (PhenomenonUOM.UnitOfMeasureID = UnitOfMeasure.ID)
group by
  Offering.Name, UnitOfMeasure.Unit
order by
  Offering.Name, UnitOfMeasure.Unit


