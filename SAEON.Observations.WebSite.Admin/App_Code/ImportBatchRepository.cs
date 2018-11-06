using System.Collections.Generic;
using System.Linq;
using Ext.Net;
using SubSonic;
using SAEON.Observations.Data;

/// <summary>
/// Summary description for ImportBatchRepository
/// </summary>
public class ImportBatchRepository : BaseRepository
{
    public ImportBatchRepository()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static List<VImportBatch> GetPagedList(StoreRefreshDataEventArgs e, string paramPrefix)
    {

        SqlQuery q = new Select().From(VImportBatch.Schema)
            .Where(VImportBatch.Columns.UserId).IsNotNull();
            //.OrderDesc(VImportBatch.Columns.Code);

        GetPagedQuery(ref q, e, paramPrefix);

        VImportBatchCollection col = q.ExecuteAsCollection<VImportBatchCollection>();

        return col.ToList<VImportBatch>();

    }
}