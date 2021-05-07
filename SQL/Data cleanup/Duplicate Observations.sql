use Observations;
with Duplicates
as
(
Select
  SensorID, ValueDate, PhenomenonOfferingID, PhenomenonUOMID, Elevation, Count(*) Count
from 
  Observation
group by
  SensorID, ValueDate, PhenomenonOfferingID, PhenomenonUOMID, Elevation
having
  (Count(*) > 1)
)
-- Only 2+ duplicates, 2430 rows
--select
--  *
--from
--  Duplicates
--order by
--  SensorID, ValueDate, PhenomenonOfferingID, PhenomenonUOMID, Elevation

-- 2+ duplicates with observations, 4860 rows
--select
--  o.*
--from
--  Duplicates d
--  inner join Observation o
--    on (d.SensorID = o.SensorID) and
--	   (d.ValueDate = o.ValueDate) and
--	   (d.PhenomenonOfferingID = o.PhenomenonOfferingID) and
--	   (d.PhenomenonUOMID = o.PhenomenonUOMID) and
--	   (((d.Elevation is null) and (o.Elevation is null)) or (d.Elevation = o.Elevation))
--order by
--  o.SensorID, o.ValueDate, o.PhenomenonOfferingID, o.PhenomenonUOMID, o.Elevation

-- 2+ duplicates with observations, import batch, 4860 rows
--select
--  d.*, o.DataValue, ImportBatch.Code ImportBatchCode
--from
--  Duplicates d
--  inner join Observation o
--    on (d.SensorID = o.SensorID) and
--	   (d.ValueDate = o.ValueDate) and
--	   (d.PhenomenonOfferingID = o.PhenomenonOfferingID) and
--	   (d.PhenomenonUOMID = o.PhenomenonUOMID) and
--	   (((d.Elevation is null) and (o.Elevation is null)) or (d.Elevation = o.Elevation))
--  inner join ImportBatch
--    on (o.ImportBatchID = ImportBatch.ID)
--order by
--  o.SensorID, o.ValueDate, o.PhenomenonOfferingID, o.PhenomenonUOMID, o.Elevation, ImportBatch.Code

-- Count of distinct ImportBatch.Code
--select
--  Count(Distinct ImportBatchCode)
--from
--(
--select
--  d.*, o.DataValue, ImportBatch.Code ImportBatchCode
--from
--  Duplicates d
--  inner join Observation o
--    on (d.SensorID = o.SensorID) and
--	   (d.ValueDate = o.ValueDate) and
--	   (d.PhenomenonOfferingID = o.PhenomenonOfferingID) and
--	   (d.PhenomenonUOMID = o.PhenomenonUOMID) and
--	   (((d.Elevation is null) and (o.Elevation is null)) or (d.Elevation = o.Elevation))
--  inner join ImportBatch
--    on (o.ImportBatchID = ImportBatch.ID)
--) a

-- Distinct ImportBatch.Code
--select Distinct
--  ImportBatch.Code ImportBatchCode
--from
--  Duplicates d
--  inner join Observation o
--    on (d.SensorID = o.SensorID) and
--	   (d.ValueDate = o.ValueDate) and
--	   (d.PhenomenonOfferingID = o.PhenomenonOfferingID) and
--	   (d.PhenomenonUOMID = o.PhenomenonUOMID) and
--	   (((d.Elevation is null) and (o.Elevation is null)) or (d.Elevation = o.Elevation))
--  inner join ImportBatch
--    on (o.ImportBatchID = ImportBatch.ID)

-- 2+ duplicates with observations, expansion, 4860 rows
select
  Sensor.Code SensorCode, Sensor.Name SensorName, o.ValueDate Date, Phenomenon.Name Phenomenon, Offering.Name Offering, UnitOfMeasure.Unit Unit, o.Elevation, ImportBatch.Code ImportBatch, ImportBatch.ImportDate, o.DataValue Value, Instrument.Code InstrumentCode, Instrument.Name InstrumentName, '' Resolution
from
  Duplicates d
  inner join Observation o
    on (d.SensorID = o.SensorID) and
	   (d.ValueDate = o.ValueDate) and
	   (d.PhenomenonOfferingID = o.PhenomenonOfferingID) and
	   (d.PhenomenonUOMID = o.PhenomenonUOMID) and
	   (d.PhenomenonUOMID = o.PhenomenonUOMID) --and
	   --(((d.Elevation is null) and (o.Elevation is null)) or
	   -- ((d.Elevation is null) and (o.Elevation is not null)) or
	   -- ((d.Elevation is not null) and (o.Elevation is null)) or
	   -- ((d.Elevation is not null) and (o.Elevation is not null)))
	    --(d.Elevation = o.Elevation))
	   --(((d.Elevation is null) and (o.Elevation is null)) or (d.Elevation = o.Elevation))
  inner join ImportBatch
    on (o.ImportBatchID = ImportBatch.ID)
  inner join PhenomenonOffering
    on (o.PhenomenonOfferingID = PhenomenonOffering.ID)
  inner join Phenomenon
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
  inner join PhenomenonUOM
    on (o.PhenomenonUOMID = PhenomenonUOM.ID)
  inner join UnitOfMeasure
    on (PhenomenonUOM.UnitOfMeasureID = UnitOfMeasure.ID)
  inner join Sensor
    on (o.SensorID = Sensor.ID)
  inner join Instrument_Sensor
    on (Instrument_Sensor.SensorID = Sensor.ID)
  inner join Instrument
    on (Instrument_Sensor.InstrumentID = Instrument.ID)
order by
  Sensor.Code, Sensor.Name, o.ValueDate, Phenomenon.Name, Offering.Name, UnitOfMeasure.Unit, o.Elevation, ImportBatch.Code
