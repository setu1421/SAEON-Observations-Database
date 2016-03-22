using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ext.Net;
using SubSonic;
using SAEON.ObservationsDB.Data;

/// <summary>
/// Summary description for ImportBatchRepository
/// </summary>
public class ImportBatchRepository:BaseRepository
{
    public ImportBatchRepository()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static List<VImportBatch> GetPagedList(StoreRefreshDataEventArgs e, string paramPrefix)
    {

        SqlQuery q = new Select().From(VImportBatch.Schema);
        q.Where(VImportBatch.Columns.UserId).IsNotNull();

        GetPagedQuery(ref q, e, paramPrefix);

        VImportBatchCollection col = q.ExecuteAsCollection<VImportBatchCollection>();

        return col.ToList<VImportBatch>();

    }
}