Select distinct
  PhenomenonOffering.ID, Sensor.ID SensorID, Sensor.Name Sensor, Phenomenon.Name Phenomenon, Offering.Name Offering, UnitOfMeasure.Unit, UnitOfMeasure.UnitSymbol Symbol, Phenomenon.Url
from
  Observation
  inner join Sensor
    on (Observation.SensorID = Sensor.ID)
  inner join PhenomenonOffering
    on (Observation.PhenomenonOfferingID = PhenomenonOffering.ID)
  inner join Phenomenon
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
  inner join PhenomenonUOM
    on (Observation.PhenomenonUOMID = PhenomenonUOM.ID)
  inner join UnitOfMeasure
    on (PhenomenonUOM.UnitOfMeasureID = UnitOfMeasure.ID)
order by
  Sensor.Name, Phenomenon.Name, Offering.Name, UnitOfMeasure.Unit