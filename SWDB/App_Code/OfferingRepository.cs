using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ext.Net;
using SubSonic;
using SAEON.ObservationsDB.Data;

/// <summary>
/// Summary description for OfferingRepository
/// </summary>
public class OfferingRepository:BaseRepository
{
    public OfferingRepository()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static List<object> GetPagedList(StoreRefreshDataEventArgs e, string paramPrefix)
    {

        SqlQuery q = new Select().From(Offering.Schema);
        q.Where(Offering.Columns.UserId).IsNotNull();

        GetPagedQuery(ref q, e, paramPrefix);

        OfferingCollection col = q.ExecuteAsCollection<OfferingCollection>();

        return col.ToList<object>();

    }
}