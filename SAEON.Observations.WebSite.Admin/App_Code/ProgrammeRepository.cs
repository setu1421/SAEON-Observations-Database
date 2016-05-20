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
public class ProgrammeRepository:BaseRepository
{
    public ProgrammeRepository()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static List<object> GetPagedList(StoreRefreshDataEventArgs e, string paramPrefix)
    {

        SqlQuery q = new Select().From(Programme.Schema);
        q.Where(Programme.Columns.UserId).IsNotNull();

        GetPagedQuery(ref q, e, paramPrefix);

        ProgrammeCollection col = q.ExecuteAsCollection<ProgrammeCollection>();

        return col.ToList<object>();
    }
}