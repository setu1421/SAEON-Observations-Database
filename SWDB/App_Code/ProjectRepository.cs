using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ext.Net;
using SubSonic;
using Observations.Data;

/// <summary>
/// Summary description for ProjectRepository
/// </summary>
public class ProjectRepository : BaseRepository
{
    public ProjectRepository()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static List<object> GetPagedList(StoreRefreshDataEventArgs e, string paramPrefix)
    {

        SqlQuery q = new Select().From(VProjectSite.Schema);
        q.Where(VProjectSite.Columns.UserId).IsNotNull();

        GetPagedQuery(ref q, e, paramPrefix);

        VProjectSiteCollection col = q.ExecuteAsCollection<VProjectSiteCollection>();

        return col.ToList<object>();
    }
}