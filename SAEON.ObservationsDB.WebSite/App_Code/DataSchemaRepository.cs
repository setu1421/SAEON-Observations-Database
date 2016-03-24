using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ext.Net;
using SubSonic;
using Observations.Data;

/// <summary>
/// Summary description for DataSchemRepository
/// </summary>
public class DataSchemRepository:BaseRepository
{
    public DataSchemRepository()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static List<object> GetPagedList(StoreRefreshDataEventArgs e, string paramPrefix)
    {

        SqlQuery q = new Select().From(VDataSchema.Schema);
        q.Where(VDataSchema.Columns.UserId).IsNotNull();

        GetPagedQuery(ref q, e, paramPrefix);

        VDataSchemaCollection col = q.ExecuteAsCollection<VDataSchemaCollection>();

        return col.ToList<object>();

    }
}