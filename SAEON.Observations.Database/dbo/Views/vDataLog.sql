CREATE VIEW [dbo].[vDataLog]
AS

SELECT 
d.ID, 
d.ImportDate,
Site.Name SiteName,
Station.Name StationName,
Instrument.Name InstrumentName,
d.SensorID,
Sensor.Name SensorName,
CASE 
    WHEN d.SensorID is null then 1
    ELSE 0
END SensorInvalid,

d.ValueDate,
d.InvalidDateValue, 
CASE 
    WHEN ValueDate is null then 1
    ELSE 0
END DateValueInvalid,

d.InvalidTimeValue, 
CASE 
    WHEN InvalidTimeValue is not null then 1
    ELSE 0
END TimeValueInvalid,

CASE 
    WHEN InvalidDateValue is null AND InvalidTimeValue IS NULL Then ValueDate
    WHEN ValueTime is not null then ValueTime 
END ValueTime,


d.RawValue,
d.ValueText,
CASE
    WHEN d.RawValue is null then 1
    ELSE 0
END RawValueInvalid,	

d.DataValue,
d.TransformValueText, 
CASE
    WHEN d.DataValue is null then 1
    ELSE 0
END DataValueInvalid,

d.PhenomenonOfferingID, 
CASE
    WHEN d.PhenomenonOfferingID is null then 1
    ELSE 0
END OfferingInvalid,

d.PhenomenonUOMID, 
CASE
    WHEN d.PhenomenonUOMID is null then 1
    ELSE 0
END UOMInvalid,

p.Name PhenomenonName,
o.Name OfferingName,
uom.Unit,

d.DataSourceTransformationID,
tt.Name Transformation,
d.StatusID,
s.Name [Status],
d.ImportBatchID,
d.RawFieldValue,
d.Comment

FROM DataLog d
  left join Sensor 
    on (d.SensorID = Sensor.ID) 
  left join Instrument_Sensor
    on (Instrument_Sensor.SensorID = Sensor.ID) 
  left join Instrument
    on (Instrument_Sensor.InstrumentID = Instrument.ID) 
  left join Station_Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID) 
  left join Station 
    on (Station_Instrument.StationID = Station.ID) 
  left join Site
    on (Station.SiteID = Site.ID) 
  LEFT JOIN PhenomenonOffering po
    ON d.PhenomenonOfferingID = po.ID
  LEFT JOIN Phenomenon p
    on po.PhenomenonID = p.ID
  LEFT JOIN Offering o
    on po.OfferingID = o.ID
  LEFT JOIN PhenomenonUOM pu
    on d.PhenomenonUOMID = pu.ID
  LEFT JOIN UnitOfMeasure uom
    on pu.UnitOfMeasureID = uom.ID
  LEFT JOIN DataSourceTransformation ds
    on d.DataSourceTransformationID = ds.ID
  LEFT JOIN TransformationType tt
    on ds.TransformationTypeID = tt.ID
  INNER JOIN [Status] s
    on d.StatusID = s.ID
WHERE
  ((Instrument_Sensor.StartDate is null) or (d.ValueDate >= Instrument_Sensor.StartDate)) and
  ((Instrument_Sensor.EndDate is null) or (d.ValueDate <= Instrument_Sensor.EndDate)) and
  ((Instrument.StartDate is null) or (d.ValueDay >= Instrument.StartDate )) and
  ((Instrument.EndDate is null) or (d.ValueDay <= Instrument.EndDate)) and
  ((Station_Instrument.StartDate is null) or (d.ValueDay >= Station_Instrument.StartDate)) and
  ((Station_Instrument.EndDate is null) or (d.ValueDay <= Station_Instrument.EndDate)) and
  ((Station.StartDate is null) or (d.ValueDay >= Station.StartDate)) and
  ((Station.EndDate is null) or (d.ValueDay <= Station.EndDate)) and
  ((Site.StartDate is null) or  (d.ValueDay >= Site.StartDate)) and
  ((Site.EndDate is null) or  (d.ValueDay <= Site.EndDate))

    




