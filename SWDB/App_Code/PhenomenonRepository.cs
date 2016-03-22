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
public class PhenomenonRepository:BaseRepository
{
    public PhenomenonRepository()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static List<object> GetPagedList(StoreRefreshDataEventArgs e, string paramPrefix)
    {

        SqlQuery q = new Select().From(Phenomenon.Schema);
        q.Where(Phenomenon.Columns.UserId).IsNotNull();

        GetPagedQuery(ref q, e, paramPrefix);

        PhenomenonCollection col = q.ExecuteAsCollection<PhenomenonCollection>();

        return col.ToList<object>();
    }
}