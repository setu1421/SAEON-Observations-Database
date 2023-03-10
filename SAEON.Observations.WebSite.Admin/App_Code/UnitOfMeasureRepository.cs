using System.Collections.Generic;
using System.Linq;
using Ext.Net;
using SubSonic;
using SAEON.Observations.Data;

/// <summary>
/// Summary description for OrginasationRepository
/// </summary>
public class UnitOfMeasureRepository:BaseRepository
{
    public UnitOfMeasureRepository()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static List<object> GetPagedList(StoreRefreshDataEventArgs e, string paramPrefix)
    {

        SqlQuery q = new Select().From(UnitOfMeasure.Schema);
        q.Where(UnitOfMeasure.Columns.UserId).IsNotNull();

        GetPagedQuery(ref q, e, paramPrefix);

        UnitOfMeasureCollection col = q.ExecuteAsCollection<UnitOfMeasureCollection>();

        return col.ToList<object>();
    }
}