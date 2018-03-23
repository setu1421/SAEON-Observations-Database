Select 
  Sensor.Name, OfferingPhenomenon.Name OfferingPhenomena, Offering.Name Offering, UOMPhenomenon.Name UnitOfMeasurePhenomenon, UnitOfMeasure.Unit, UnitOfMeasure.UnitSymbol, Count(*) Count
from
  Observation
  inner join Sensor
    on (Observation.SensorID = Sensor.ID)
  inner join PhenomenonOffering
    on (Observation.PhenomenonOfferingID = PhenomenonOffering.ID)
  inner join Offering  
    on (PhenomenonOffering.OfferingID = Offering.ID)
  inner join Phenomenon OfferingPhenomenon
    on (PhenomenonOffering.PhenomenonID = OfferingPhenomenon.ID)
  inner join PhenomenonUOM
    on (Observation.PhenomenonUOMID = PhenomenonUOM.ID)
  inner join UnitOfMeasure
    on (PhenomenonUOM.UnitOfMeasureID = UnitOfMeasure.ID)
  inner join Phenomenon UOMPhenomenon
    on (PhenomenonUOM.PhenomenonID = UOMPhenomenon.ID)
group by
  Sensor.Name, OfferingPhenomenon.Name, Offering.Name, UOMPhenomenon.Name, UnitOfMeasure.Unit, UnitOfMeasure.UnitSymbol
order by
  Sensor.Name, OfferingPhenomenon.Name, Offering.Name, UOMPhenomenon.Name, UnitOfMeasure.Unit, UnitOfMeasure.UnitSymbol