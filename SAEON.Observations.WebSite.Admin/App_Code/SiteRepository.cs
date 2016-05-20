using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ext.Net;
using SubSonic;
using SAEON.Observations.Data;

/// <summary>
/// Summary description for OrginasationRepository
/// </summary>
public class SiteRepository:BaseRepository
{
    public SiteRepository()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static List<object> GetPagedList(StoreRefreshDataEventArgs e, string paramPrefix)
    {

        SqlQuery q = new Select().From(Site.Schema);
        q.Where(Site.Columns.UserId).IsNotNull();

        GetPagedQuery(ref q, e, paramPrefix);

        SiteCollection col = q.ExecuteAsCollection<SiteCollection>();

        return col.ToList<object>();
    }
}