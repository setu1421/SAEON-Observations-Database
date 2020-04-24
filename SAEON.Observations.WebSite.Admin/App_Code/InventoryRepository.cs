using Ext.Net;
using SAEON.Observations.Data;
using SubSonic;
using System.Collections.Generic;
using System.Linq;

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

        SqlQuery q = new Select().From(VInventorySensor.Schema);
        GetPagedQuery(ref q, e, paramPrefix);
        VInventorySensorCollection col = q.ExecuteAsCollection<VInventorySensorCollection>();

        return col.ToList<object>();

    }
}