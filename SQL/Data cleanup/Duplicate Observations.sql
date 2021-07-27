--use Observations;
with Duplicates
as
(
Select
  SensorID, ValueDate, PhenomenonOfferingID, PhenomenonUOMID, Elevation,
  Row_Number() over 
	(partition by SensorID, ValueDate, PhenomenonOfferingID, PhenomenonUOMID, Elevation 
	 order by SensorID, ValueDate, PhenomenonOfferingID, PhenomenonUOMID, Elevation) RowNum 
from 
  Observation
)
select
  Sensor.Code SensorCode, Sensor.Name SensorName, o.ValueDate Date, Phenomenon.Name Phenomenon, Offering.Name Offering, UnitOfMeasure.Unit Unit, o.Elevation, ImportBatch.Code ImportBatch, ImportBatch.ImportDate, o.DataValue Value, Instrument.Code InstrumentCode, Instrument.Name InstrumentName, '' Resolution
from
  Duplicates d
  inner join Observation o
    on (d.SensorID = o.SensorID) and
	   (d.ValueDate = o.ValueDate) and
	   (d.PhenomenonOfferingID = o.PhenomenonOfferingID) and
	   (d.PhenomenonUOMID = o.PhenomenonUOMID) and
	   (d.PhenomenonUOMID = o.PhenomenonUOMID)
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
where
  (d.RowNum > 1)
order by
  Sensor.Code, Sensor.Name, o.ValueDate, Phenomenon.Name, Offering.Name, UnitOfMeasure.Unit, o.Elevation, ImportBatch.Code
