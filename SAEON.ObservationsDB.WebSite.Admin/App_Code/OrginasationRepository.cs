using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ext.Net;
using SubSonic;
using SAEON.ObservationsDB.Data;

/// <summary>
/// Summary description for OrginasationRepository
/// </summary>
public class OrginasationRepository:BaseRepository
{
    public OrginasationRepository()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static List<object> GetPagedList(StoreRefreshDataEventArgs e, string paramPrefix)
    {

        SqlQuery q = new Select().From(Organisation.Schema);
        q.Where(Organisation.Columns.UserId).IsNotNull();

        GetPagedQuery(ref q, e, paramPrefix);

        OrganisationCollection col = q.ExecuteAsCollection<OrganisationCollection>();

        return col.ToList<object>();

    }
}