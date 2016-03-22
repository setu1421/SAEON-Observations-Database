using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ext.Net;
using SubSonic;
using Observations.Data;

/// <summary>
/// Summary description for OrginasationRepository
/// </summary>
public class StationRepository:BaseRepository
{
    public StationRepository()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static List<object> GetPagedList(StoreRefreshDataEventArgs e, string paramPrefix)
    {

        SqlQuery q = new Select().From(VStation.Schema);
        q.Where(VStation.Columns.UserId).IsNotNull();

        GetPagedQuery(ref q, e, paramPrefix);

        VStationCollection col = q.ExecuteAsCollection<VStationCollection>();

        return col.ToList<object>();
    }
}