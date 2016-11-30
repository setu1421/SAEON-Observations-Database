--> Changed 2.0.3 20160503 TimPN
--Renamed SensorProcedure to Sensor
--< Changed 2.0.3 20160503 TimPN
CREATE VIEW [dbo].[vDataLog]
AS

SELECT 
d.ID, 
d.ImportDate,

--> Added 2.0.17 20161128 TimPN
Site.Name SiteName,
Station.Name StationName,
Instrument.Name InstrumentName,
--< Added 2.0.17 20161128 TimPN
--> Removed 2.0.17 20161128 TimPN
--org.Name Organisation,
--ps.[Description] ProjectSite,
--st.Name StationName,
--< Removed 2.0.17 20161128 TimPN
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

--> Changed 2.0.3 20160421 TimPN
--d.PhenonmenonUOMID, 
d.PhenomenonUOMID, 
--< Changed 2.0.3 20160421 TimPN
CASE
--> Changed 2.0.3 20160421 TimPN
--    WHEN d.PhenonmenonUOMID is null then 1
    WHEN d.PhenomenonUOMID is null then 1
--< Changed 2.0.3 20160421 TimPN
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
--> Removed 2.0.17 20161128 TimPN
--LEFT JOIN Sensor sp
--    on d.SensorID = sp.ID
--LEFT JOIN Station st
--    on sp.StationID = st.ID
--LEFT JOIN ProjectSite ps
--    on st.ProjectSiteID = ps.ID
--LEFT JOIN Organisation org
--    on ps.OrganisationID = org.ID
--< Removed 2.0.17 20161128 TimPN
--> Added 2.0.17 20161128 TimPN
  inner join Sensor 
    on (d.SensorID = Sensor.ID) 
  inner join Instrument_Sensor
    on (Instrument_Sensor.SensorID = Sensor.ID) and
	   ((Instrument_Sensor.StartDate is null) or (d.ValueDate >= Instrument_Sensor.StartDate)) and
	   ((Instrument_Sensor.EndDate is null) or (d.ValueDate <= Instrument_Sensor.EndDate))
  inner join Instrument
    on (Instrument_Sensor.InstrumentID = Instrument.ID) and
	   ((Instrument.StartDate is null) or (d.ValueDate >= Instrument.StartDate )) and
	   ((Instrument.EndDate is null) or (d.ValueDate <= Instrument.EndDate))
  inner join Station_Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID) and
	   ((Station_Instrument.StartDate is null) or (d.ValueDate >= Station_Instrument.StartDate)) and
	   ((Station_Instrument.EndDate is null) or (d.ValueDate >= Station_Instrument.EndDate))
  inner join Station 
    on (Station_Instrument.StationID = Station.ID) and
	   ((Station.StartDate is null) or (Cast(d.ValueDate as Date) >= Cast(Station.StartDate as Date))) and
	   ((Station.EndDate is null) or (Cast(d.ValueDate as Date) >= Cast(Station.EndDate as Date)))
  inner join Site
    on (Station.SiteID = Site.ID) and
	   ((Site.StartDate is null) or  (Cast(d.ValueDate as Date) >= Cast(Site.StartDate as Date))) and
	   ((Site.EndDate is null) or  (Cast(d.ValueDate as Date) >= Cast(Site.EndDate as Date)))
--< Added 2.0.17 20161128 TimPN
LEFT JOIN PhenomenonOffering po
 ON d.PhenomenonOfferingID = po.ID
LEFT JOIN Phenomenon p
    on po.PhenomenonID = p.ID
LEFT JOIN Offering o
    on po.OfferingID = o.ID
LEFT JOIN PhenomenonUOM pu
--> Changed 2.0.3 20160421 TimPN
--    on d.PhenonmenonUOMID = pu.ID
    on d.PhenomenonUOMID = pu.ID
--< Changed 2.0.3 20160421 TimPN
LEFT JOIN UnitOfMeasure uom
    on pu.UnitOfMeasureID = uom.ID
LEFT JOIN DataSourceTransformation ds
    on d.DataSourceTransformationID = ds.ID
LEFT JOIN TransformationType tt
    on ds.TransformationTypeID = tt.ID
INNER JOIN [Status] s
    on d.StatusID = s.ID

    




