//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using Ext.Net;
//using SubSonic;
//using SAEON.Observations.Data;

///// <summary>
///// Summary description for OfferingRepository
///// </summary>
//public class DataQueyRepository:BaseRepository
//{
//    public DataQueyRepository()
//    {
//        //
//        // TODO: Add constructor logic here
//        //
//    }

//    public static List<object> GetPagedList(StoreRefreshDataEventArgs e, string paramPrefix)
//    {

//        SqlQuery q = new Select().From(VObservation.Schema);
//        q.Where(VObservation.Columns.Id).IsNotNull();

//        GetPagedQuery(ref q, e, paramPrefix);

//        VObservationCollection col = q.ExecuteAsCollection<VObservationCollection>();

//        return col.ToList<object>();

//    }
//}