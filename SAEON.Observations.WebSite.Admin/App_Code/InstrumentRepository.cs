using System.Collections.Generic;
using System.Linq;
using Ext.Net;
using SubSonic;
using SAEON.Observations.Data;

/// <summary>
/// Summary description for OrginasationRepository
/// </summary>
public class InstrumentRepository:BaseRepository
{
    public InstrumentRepository()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static List<object> GetPagedList(StoreRefreshDataEventArgs e, string paramPrefix)
    {

        SqlQuery q = new Select().From(Instrument.Schema);
        q.Where(Instrument.Columns.UserId).IsNotNull();

        GetPagedQuery(ref q, e, paramPrefix);

        InstrumentCollection col = q.ExecuteAsCollection<InstrumentCollection>();

        return col.ToList<object>();
    }
}