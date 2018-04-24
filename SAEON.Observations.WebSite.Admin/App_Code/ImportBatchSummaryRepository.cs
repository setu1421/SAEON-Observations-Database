using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ext.Net;
using SubSonic;
using SAEON.Observations.Data;

/// <summary>
/// Summary description for ImportBatchRepository
/// </summary>
public class ImportBatchSummaryRepository : BaseRepository
{
    public ImportBatchSummaryRepository()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static List<VImportBatchSummary> GetPagedListByBatch(StoreRefreshDataEventArgs e, string paramPrefix, Guid Id)
    {

        SqlQuery q = new Select().From(VImportBatchSummary.Schema)
            .Where(VImportBatchSummary.Columns.ImportBatchID).IsEqualTo(Id)
            .OrderAsc(VImportBatchSummary.Columns.PhenomenonName)
            .OrderAsc(VImportBatchSummary.Columns.OfferingName)
            .OrderAsc(VImportBatchSummary.Columns.UnitOfMeasureUnit);

        GetPagedQuery(ref q, e, paramPrefix);

        VImportBatchSummaryCollection col = q.ExecuteAsCollection<VImportBatchSummaryCollection>();

        return col.ToList<VImportBatchSummary>();

    }
}