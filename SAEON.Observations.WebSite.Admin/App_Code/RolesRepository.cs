using System;
using System.Collections.Generic;
using System.Linq;
using SAEON.Observations.Data;
using SubSonic;
using Ext.Net;

/// <summary>
/// Summary description for RoleRepository
/// </summary>
public class RoleRepository
{
	public RoleRepository()
    {
    }

    public static List<object>	GetPagedList(StoreRefreshDataEventArgs e, string paramPrefix)
    {

        SqlQuery q = new Select().From(AspnetRole.Schema);

		q.Where(AspnetRole.Columns.RoleId).IsNotNull();

        //string s = e.Parameters[paramPrefix];
		string s = paramPrefix;

        if (!string.IsNullOrEmpty(s))
        {
            FilterConditions fc = new FilterConditions(s);

            foreach (FilterCondition condition in fc.Conditions)
            {
                switch (condition.FilterType)
                {
					//case FilterType.Date:
					//    switch (condition.Comparison.ToString())
					//    {
					//        case "Eq":
					//            q.And(condition.Name).IsEqualTo(condition.Value);

					//            break;
					//        case "Gt":
					//            q.And(condition.Name).IsGreaterThanOrEqualTo(condition.Value);

					//            break;
					//        case "Lt":
					//            q.And(condition.Name).IsLessThanOrEqualTo(condition.Value);

					//            break;
					//        default:
					//            break;
					//    }

					//    break;

					//case FilterType.Numeric:
					//    switch (condition.Comparison.ToString())
					//    {
					//        case "Eq":
					//            q.And(condition.Name).IsEqualTo(condition.Value);

					//            break;
					//        case "Gt":
					//            q.And(condition.Name).IsGreaterThanOrEqualTo(condition.Value);

					//            break;
					//        case "Lt":
					//            q.And(condition.Name).IsLessThanOrEqualTo(condition.Value);

					//            break;
					//        default:
					//            break;
					//    }

					//    break;

                    case FilterType.String:
                        q.And(condition.Name).Like("%" + condition.Value + "%");


                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }

        }

        if (!string.IsNullOrEmpty(e.Sort))
        {
            if (e.Dir == Ext.Net.SortDirection.DESC)
            {
                q.OrderDesc(e.Sort);
            }
            else
            {
                q.OrderAsc(e.Sort);
            }
        }

        int total = q.GetRecordCount();

        e.Total = total;


        int currenPage = e.Start / e.Limit + 1;

        if (e.Limit > total)
            q.Paged(currenPage, total);
        else
            q.Paged(currenPage, e.Limit);


		AspnetRoleCollection userCol = q.ExecuteAsCollection<AspnetRoleCollection>();

        return userCol.ToList<object>();

    }
}