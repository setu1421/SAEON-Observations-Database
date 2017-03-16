using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ext.Net;
using SubSonic;
using SAEON.Observations.Data;
using System.Web.Security;
using Serilog;
using System.Transactions;

/// <summary>
/// Summary description for OfferingRepository
/// </summary>
public class DataQueryRepository : BaseRepository
{
    public DataQueryRepository()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static List<object> GetPagedList(StoreRefreshDataEventArgs e, string paramPrefix, DateTime FromDate, DateTime ToDate)
    {
        //string[] roles = Roles.GetRolesForUser();
        //SqlQuery q = new Select().From(VObservation.Schema)
        //    .InnerJoin(DataSourceRole.DataSourceIDColumn, VObservation.Schema.GetColumn("DataSourceID"))
        //    .Where(VObservation.Columns.ValueDate).IsGreaterThanOrEqualTo(FromDate)
        //    .And(VObservation.Columns.ValueDate).IsLessThanOrEqualTo(ToDate)
        //    .And(DataSourceRole.Columns.RoleName).In(roles)
        //    .And(VObservation.Columns.ValueDate).IsGreaterThanOrEqualTo(DataSourceRole.Columns.DateStart)
        //    .And(VObservation.Columns.ValueDate).IsLessThanOrEqualTo(DataSourceRole.Columns.DateEnd);
        SqlQuery q = new Select().From(VObservationRole.Schema)
            .Where(VObservationRole.Columns.UserId).IsEqualTo(AuthHelper.GetLoggedInUserId);

        GetPagedQuery(ref q, e, paramPrefix);
        Log.Verbose("GetPagedList SQL: {sql}", q.BuildSqlStatement());
        VObservationCollection col = q.ExecuteAsCollection<VObservationCollection>();
        return col.ToList<object>();
    }

    public static List<object> GetPagedFilteredList(StoreRefreshDataEventArgs e,
                                           string paramPrefix,
                                           ref SqlQuery q)
    {

        //SqlQuery q = new Select().From(VObservation.Schema)
        //             .Where(VObservation.Columns.ValueDate).IsGreaterThanOrEqualTo(FromDate)
        //             .And(VObservation.Columns.ValueDate).IsLessThanOrEqualTo(ToDate);

        //if (stations.Count > 0)
        //    q.Or(VObservation.Columns.StationID).In(stations);

        //if (sensors.Count > 0)
        //    q.Or(VObservation.Columns.SensorProcedureID).In(sensors);

        //if (phenomenon.Count > 0)
        //    q.Or(VObservation.Columns.PhenomenonID).In(phenomenon);

        //if (offerings.Count > 0)
        //    q.Or(VObservation.Columns.PhenomenonOfferingID).In(offerings);

        GetPagedQuery(ref q, e, paramPrefix);
        //Log.Verbose("GetPagedFilteredList SQL: {sql}", q.BuildSqlStatement());
        VObservationCollection col = q.ExecuteAsCollection<VObservationCollection>();
        return col.ToList<object>();
    }

    //public static void qFilterNSort(ref SqlQuery q, ref StoreRefreshDataEventArgs storeRefreshDataEventArgs, string json)
    public static void qFilterNSort(ref SqlQuery q, ref StoreRefreshDataEventArgs storeRefreshDataEventArgs)
    {
        //string filters;
        //if (json != null)
        //{
        //    filters = json;
        //}
        //else
        //{
        //    filters = storeRefreshDataEventArgs.Parameters["gridfilters"];
        //}

        if (storeRefreshDataEventArgs.Parameters["gridfilters"] != null)
        {
            if (!string.IsNullOrEmpty(storeRefreshDataEventArgs.Parameters["gridfilters"]))
            {
                FilterConditions fc = new FilterConditions(storeRefreshDataEventArgs.Parameters["gridfilters"]);

                foreach (FilterCondition condition in fc.Conditions)
                {
                    switch (condition.FilterType)
                    {
                        case FilterType.Date:
                            switch (condition.Comparison.ToString())
                            {
                                case "Eq":
                                    q.And(condition.Name).IsEqualTo(condition.Value);

                                    break;
                                case "Gt":
                                    q.And(condition.Name).IsGreaterThanOrEqualTo(condition.Value);

                                    break;
                                case "Lt":
                                    q.And(condition.Name).IsLessThanOrEqualTo(condition.Value);

                                    break;
                                default:
                                    break;
                            }

                            break;

                        case FilterType.Numeric:
                            switch (condition.Comparison.ToString())
                            {
                                case "Eq":
                                    q.And(condition.Name).IsEqualTo(condition.Value);

                                    break;
                                case "Gt":
                                    q.And(condition.Name).IsGreaterThanOrEqualTo(condition.Value);

                                    break;
                                case "Lt":
                                    q.And(condition.Name).IsLessThanOrEqualTo(condition.Value);

                                    break;
                                default:
                                    break;
                            }

                            break;

                        case FilterType.String:
                            q.And(condition.Name).Like("%" + condition.Value + "%");


                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                }

            }
        }




    }

    public static void qPage(ref SqlQuery q, ref StoreRefreshDataEventArgs storeRefreshDataEventArgs)
    {
        if (!string.IsNullOrEmpty(storeRefreshDataEventArgs.Sort))
        {
            if (storeRefreshDataEventArgs.Dir == Ext.Net.SortDirection.DESC)
            {
                q.OrderDesc(storeRefreshDataEventArgs.Sort);
            }
            else
            {
                q.OrderAsc(storeRefreshDataEventArgs.Sort);
            }
        }

        int total = q.GetRecordCount();

        storeRefreshDataEventArgs.Total = total;

        int currenPage = storeRefreshDataEventArgs.Start / storeRefreshDataEventArgs.Limit + 1;
        Log.Verbose("CurrentPage: {currentPage} Start: {Start} Limit: {Limit} Total: {total}", currenPage, storeRefreshDataEventArgs.Start, storeRefreshDataEventArgs.Limit, total);
        if (storeRefreshDataEventArgs.Limit > total)
            q.Paged(currenPage, total);
        else
            q.Paged(currenPage, storeRefreshDataEventArgs.Limit);
    }
}