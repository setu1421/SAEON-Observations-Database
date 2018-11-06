using Ext.Net;
using SAEON.Observations.Data;
using SubSonic;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Summary description for DataLogRepository
/// </summary>
public class DataLogRepository : BaseRepository
{
    public DataLogRepository()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static List<VDataLog> GetPagedListByBatch(StoreRefreshDataEventArgs e, string paramPrefix, Guid BatchID)
    {


        SqlQuery q = new Select().From(VDataLog.Schema);
        q.Where(VDataLog.Columns.ImportBatchID).IsEqualTo(BatchID);

        GetPagedQuery(ref q, e, paramPrefix);

        VDataLogCollection col = q.ExecuteAsCollection<VDataLogCollection>();

        return col.ToList<VDataLog>();

    }
}