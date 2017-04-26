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
  Phenomenon.Name Phenomenon, count(*) Count
from
  Phenomenon
  inner join PhenomenonOffering
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
group by
  Phenomenon.Name
order by
  Count(*) desc, Phenomenon.Name
Select
  Phenomenon.Name Phenomenon, Offering.Name Offering
from
  Phenomenon
  inner join PhenomenonOffering
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
order by
  Phenomenon.Name, Offering.Name
Select
  Phenomenon.Name Phenomenon, UnitOfMeasure.Unit unitOfMeasure
from
  Phenomenon
  inner join PhenomenonUOM
    on (PhenomenonUOM.PhenomenonID = Phenomenon.ID)
  inner join UnitOfMeasure
    on (PhenomenonUOM.UnitOfMeasureID = UnitOfMeasure.ID)
order by
  Phenomenon.Name, UnitOfMeasure.Unit
Select
  Phenomenon.Name Phenomenon, Offering.Name Offering, UnitOfMeasure.Unit unitOfMeasure
from
  Phenomenon
  inner join PhenomenonOffering
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
  inner join PhenomenonUOM
    on (PhenomenonUOM.PhenomenonID = Phenomenon.ID)
  inner join UnitOfMeasure
    on (PhenomenonUOM.UnitOfMeasureID = UnitOfMeasure.ID)
order by
  Phenomenon.Name, Offering.Name, UnitOfMeasure.Unit
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
  Count(*) desc, Offering.Name, UnitOfMeasure.Unit


