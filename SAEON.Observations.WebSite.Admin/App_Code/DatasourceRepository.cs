using System.Collections.Generic;
using System.Linq;
using Ext.Net;
using SubSonic;
using SAEON.Observations.Data;

/// <summary>
/// Summary description for DatasourceRepository
/// </summary>
public class DataSourceRepository:BaseRepository
{
    public DataSourceRepository()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static List<object> GetPagedList(StoreRefreshDataEventArgs e, string paramPrefix)
    {

        SqlQuery q = new Select().From(VDataSource.Schema);
        q.Where(VDataSource.Columns.UserId).IsNotNull();

        GetPagedQuery(ref q, e, paramPrefix);

        VDataSourceCollection col = q.ExecuteAsCollection<VDataSourceCollection>();

        return col.ToList<object>();

    }
}