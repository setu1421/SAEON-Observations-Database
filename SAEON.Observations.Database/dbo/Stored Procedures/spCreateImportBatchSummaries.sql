CREATE PROCEDURE [dbo].[spCreateImportBatchSummaries]
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        Delete ImportBatchSummary
        Insert Into ImportBatchSummary
          (ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID, Count, ValueCount, Minimum, Maximum, Average, StandardDeviation, Variance, 
           LatitudeNorth, LatitudeSouth, LongitudeWest, LongitudeEast, ElevationMinimum, ElevationMaximum, StartDate, EndDate, VerifiedCount, UnverifiedCount)
        Select
          ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID, COUNT(ID) Count, COUNT(DataValue) ValueCount, 
          MIN(DataValue) Minimum, MAX(DataValue) Maximum, 
          AVG(DataValue) Average, STDEV(DataValue) StandardDeviation, VAR(DataValue) Variance, 
          Max(Latitude) LatitudeNorth, Min(Latitude) LatitudeSouth, Min(Longitude) LongitudeWest, Max(Longitude) LongitudeEast,
          Min(Elevation) ElevationMinimum, Max(Elevation) ElevationMaximum, Min(ValueDate) StartDate, Max(ValueDate) EndDate,
          (
          Select 
            Count(*) 
          from 
            Observation
	        left join Status
	          on (Observation.StatusID = Status.ID)
          where 
            ((Observation.ImportBatchID = vObservationExpansion.ImportBatchID) and 
	         (Observation.SensorID = vObservationExpansion.SensorID) and
	         (Observation.PhenomenonOfferingID = vObservationExpansion.PhenomenonOfferingID) and
	         (Observation.PhenomenonUOMID = vObservationExpansion.PhenomenonUOMID) and
	         ((Observation.StatusID is null) or (Status.Name = 'Verified')))
          ) VerifiedCount,
          (
          Select 
            Count(*) 
          from 
            Observation
	        inner join Status
	          on (Observation.StatusID = Status.ID)
          where 
            ((Observation.ImportBatchID = vObservationExpansion.ImportBatchID) and 
	         (Observation.SensorID = vObservationExpansion.SensorID) and
	         (Observation.PhenomenonOfferingID = vObservationExpansion.PhenomenonOfferingID) and
	         (Observation.PhenomenonUOMID = vObservationExpansion.PhenomenonUOMID) and
	         (Status.Name <> 'Verified'))
          ) UnverifiedCount
        from
          vObservationExpansion
        group by
          ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID
	    COMMIT TRANSACTION 
 		Print 'Committed transaction'
    END TRY
    BEGIN CATCH
        --SELECT
        --    ERROR_NUMBER() AS ErrorNumber,
        --    ERROR_STATE() AS ErrorState,
        --    ERROR_SEVERITY() AS ErrorSeverity,
        --    ERROR_PROCEDURE() AS ErrorProcedure,
        --    ERROR_LINE() AS ErrorLine,
        --    ERROR_MESSAGE() AS ErrorMessage;
        Print 'Error Number: '+Cast(Error_Number() as VarChar(100))+' State: '+Cast(Error_State()as VarChar(100))+
            ' Severity: '+Cast(Error_Severity() as VarChar(100))+' Procedure: '+Coalesce(Error_Procedure(),'None')+
            ' Line: '+Cast(Error_Line() as VarChar(100))+' Message: '+Error_Message()
        ROLLBACK TRANSACTION;
	    Print 'Rolled back transaction';
        --Throw;
    END CATCH
    Select top(1) * from ImportBatchSummary;
END
