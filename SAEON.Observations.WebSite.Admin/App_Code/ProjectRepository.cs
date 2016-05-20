﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ext.Net;
using SubSonic;
using SAEON.Observations.Data;

/// <summary>
/// Summary description for OrginasationRepository
/// </summary>
public class ProjectRepository:BaseRepository
{
    public ProjectRepository()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static List<object> GetPagedList(StoreRefreshDataEventArgs e, string paramPrefix)
    {

        SqlQuery q = new Select().From(Project.Schema);
        q.Where(Project.Columns.UserId).IsNotNull();

        GetPagedQuery(ref q, e, paramPrefix);

        ProjectCollection col = q.ExecuteAsCollection<ProjectCollection>();

        return col.ToList<object>();
    }
}