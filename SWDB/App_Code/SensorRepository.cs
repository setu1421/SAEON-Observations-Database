﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ext.Net;
using SubSonic;
using Observations.Data;

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

		SqlQuery q = new Select().From(VSensorProcedure.Schema);
        q.Where(VSensorProcedure.Columns.Id).IsNotNull();

        GetPagedQuery(ref q, e, paramPrefix);

		VSensorProcedureCollection col = q.ExecuteAsCollection<VSensorProcedureCollection>();

        return col.ToList<object>();

    }
}