using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SAEON.Logs;
using SAEON.Observations.Auth;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    [Authorize(Policy = TenantAuthenticationDefaults.TenantPolicy)]
    //[Authorize(Policy = ODPAuthenticationDefaults.AccessTokenPolicy)]
    //[Route("OData/[controller]")]
    public abstract class BaseController<TEntity> : ODataController where TEntity : BaseEntity
    {
        public static string BaseUrl { get; set; }
        protected const int PageSize = 25;
        protected const int MaxTop = 500;
        protected const int MaxAll = 10000;

        private ObservationsDbContext _dbContext;
        protected ObservationsDbContext DbContext => _dbContext ??= HttpContext.RequestServices.GetRequiredService<ObservationsDbContext>();

        /// <summary>
        /// Overwrite to order of entities
        /// </summary>
        /// <returns>ListOf(PredicateOf(TEntity))</returns>
        protected virtual List<Expression<Func<TEntity, object>>> GetOrderBys()
        {
            return new List<Expression<Func<TEntity, object>>>();
        }

        /// <summary>
        /// query for items
        /// </summary>
        /// <returns></returns>
        protected IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> extraWhere = null)
        {
            var query = DbContext.Set<TEntity>().AsNoTracking().AsQueryable();
            if (extraWhere != null)
            {
                query = query.Where(extraWhere);
            }
            foreach (var orderBy in GetOrderBys())
            {
                query = query.OrderBy(orderBy);
            }
            return query;
        }

        protected void UpdateRequest()
        {
            BaseUrl = Request.GetUri().GetLeftPart(UriPartial.Authority) + "/OData";
            //if (isMany)
            //{
            //    var uri = Request.RequestUri.ToString();
            //    var query = Request.RequestUri.Query.ToLowerInvariant();
            //    if (!query.Contains("$count=true"))
            //    {
            //        if (string.IsNullOrWhiteSpace(query))
            //            uri += "?$count = true";
            //        else
            //            uri += "&$count=true";
            //        Request.RequestUri = new Uri(uri);
            //    }
            //}
            Request.Headers.TryAdd("Prefer", "odata.include-annotations=*");
        }

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <returns>ListOf(TEntity)</returns>
        // GET: OData/TEntity
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        public virtual IQueryable<TEntity> Get()
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    UpdateRequest();
                    SAEONLogs.Verbose("uri: {uri}", Request.GetUri());
                    return GetQuery().Take(MaxAll);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get all");
                    throw;
                }
            }
        }
    }
}

