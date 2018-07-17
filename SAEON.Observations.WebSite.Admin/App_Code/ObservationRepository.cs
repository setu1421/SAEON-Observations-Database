using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ext.Net;
using SubSonic;
using SAEON.Observations.Data;
using System.Web.Security;

/// <summary>
/// Summary description for ObservationRepository
/// </summary>
public class ObservationRepository : BaseRepository
{
    public ObservationRepository()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    //public static List<object> GetPagedList(StoreRefreshDataEventArgs e, string paramPrefix)
    //{
    //    SqlQuery q = new Select().From(VObservation.Schema);

    //    GetPagedQuery(ref q, e, paramPrefix);

    //    VObservationCollection col = q.ExecuteAsCollection<VObservationCollection>();

    //    return col.ToList();
    //}

    public static List<VObservationExpansion> GetPagedListByBatch(StoreRefreshDataEventArgs e, string paramPrefix, Guid BatchID)
    {
        SqlQuery q = new Select().From(VObservationExpansion.Schema)
            .Where(VObservationExpansion.Columns.ImportBatchID).IsEqualTo(BatchID)
            .OrderDesc(VObservationExpansion.Columns.ValueDate)
            .OrderAsc(VObservationExpansion.Columns.SensorName)
            .OrderAsc(VObservationExpansion.Columns.OfferingName);
        
        GetPagedQuery(ref q, e, paramPrefix);
        return q.ExecuteAsCollection<VObservationExpansionCollection>().ToList();
    }
}