Select 
  ImportBatch.Code, Sensor.Name, OfferingPhenomenon.Name OfferingPhenomena, Offering.Name Offering, UOMPhenomenon.Name UnitOfMeasurePhenomenon, UnitOfMeasure.Unit, UnitOfMeasure.UnitSymbol, Count(*) Count, AVG(DataValue) Average, MIN(DataValue) Minimum, Max(DataValue) Maximum, STDEV(DataValue) StandardDeviation, VAR(DataValue) Variance
from
  ImportBatch
  left join Observation
      on (Observation.ImportBatchID = ImportBatch.ID)
  left join Sensor
    on (Observation.SensorID = Sensor.ID)
  left join PhenomenonOffering
    on (Observation.PhenomenonOfferingID = PhenomenonOffering.ID)
  left join Offering  
    on (PhenomenonOffering.OfferingID = Offering.ID)
  left join Phenomenon OfferingPhenomenon
    on (PhenomenonOffering.PhenomenonID = OfferingPhenomenon.ID)
  left join PhenomenonUOM
    on (Observation.PhenomenonUOMID = PhenomenonUOM.ID)
  left join UnitOfMeasure
    on (PhenomenonUOM.UnitOfMeasureID = UnitOfMeasure.ID)
  left join Phenomenon UOMPhenomenon
    on (PhenomenonUOM.PhenomenonID = UOMPhenomenon.ID)
group by
  ImportBatch.Code, Sensor.Name, OfferingPhenomenon.Name, Offering.Name, UOMPhenomenon.Name, UnitOfMeasure.Unit, UnitOfMeasure.UnitSymbol
order by
  ImportBatch.Code, Sensor.Name, OfferingPhenomenon.Name, Offering.Name, UOMPhenomenon.Name, UnitOfMeasure.Unit, UnitOfMeasure.UnitSymbol