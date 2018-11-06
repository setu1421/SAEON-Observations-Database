using System.Collections.Generic;
using System.Linq;
using Ext.Net;
using SubSonic;
using SAEON.Observations.Data;

/// <summary>
/// Summary description for OfferingRepository
/// </summary>
public class SensorRepository:BaseRepository
{
	public SensorRepository()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static List<object> GetPagedList(StoreRefreshDataEventArgs e, string paramPrefix)
    {

		SqlQuery q = new Select().From(VSensor.Schema);
        q.Where(VSensor.Columns.Id).IsNotNull();

        GetPagedQuery(ref q, e, paramPrefix);

		VSensorCollection col = q.ExecuteAsCollection<VSensorCollection>();

        return col.ToList<object>();

    }
}