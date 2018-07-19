using Ext.Net;
using SAEON.Logs;
using SAEON.Observations.Data;
using SubSonic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for InventoryRepository
/// </summary>
public class InventoryRepository : BaseRepository
{
    public InventoryRepository()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static List<object> GetPagedList(StoreRefreshDataEventArgs e, string paramPrefix)
    {

        SqlQuery q = new Select().From(VInventory.Schema);
        GetPagedQuery(ref q, e, paramPrefix);
        VInventoryCollection col = q.ExecuteAsCollection<VInventoryCollection>();

        return col.ToList<object>();

    }
}