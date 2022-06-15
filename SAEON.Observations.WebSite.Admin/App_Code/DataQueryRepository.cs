using Ext.Net;
using SAEON.Observations.Data;
using SubSonic;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Summary description for OfferingRepository
/// </summary>
public class DataQueryRepository : BaseRepository
{
    public DataQueryRepository()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    //public static List<object> GetPagedList(StoreRefreshDataEventArgs e, string paramPrefix, DateTime FromDate, DateTime ToDate)
    //{
    //    SqlQuery q = new Select().From(VObservation.Schema);

    //    GetPagedQuery(ref q, e, paramPrefix);
    //    //SAEONLogs.Verbose("GetPagedList SQL: {sql}", q.BuildSqlStatement());
    //    VObservationCollection col = q.ExecuteAsCollection<VObservationCollection>();
    //    return col.ToList<object>();
    //}

    public static List<object> GetPagedFilteredList(StoreRefreshDataEventArgs e,
                                           string paramPrefix,
                                           ref SqlQuery q)
    {

        //SqlQuery q = new Select().From(VObservation.Schema)
        //             .Where(VObservation.Columns.ValueDate).IsGreaterThanOrEqualTo(FromDate)
        //             .And(VObservation.Columns.ValueDate).IsLessThanOrEqualTo(ToDate);

        //if (stations.Count > 0)
        //    q.Or(VObservation.Columns.StationID).In(stations);

        //if (sensors.Count > 0)
        //    q.Or(VObservation.Columns.SensorProcedureID).In(sensors);

        //if (phenomenon.Count > 0)
        //    q.Or(VObservation.Columns.PhenomenonID).In(phenomenon);

        //if (offerings.Count > 0)
        //    q.Or(VObservation.Columns.PhenomenonOfferingID).In(offerings);

        GetPagedQuery(ref q, e, paramPrefix);
        //GetUnpagedQuery(ref q, e, paramPrefix);
        //SAEONLogs.Verbose("GetPagedFilteredList SQL: {sql}", q.BuildSqlStatement());
        VObservationCollection col = q.ExecuteAsCollection<VObservationCollection>();
        return col.ToList<object>();
    }
}