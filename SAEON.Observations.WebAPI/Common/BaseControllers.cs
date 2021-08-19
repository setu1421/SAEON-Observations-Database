using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SAEON.AspNet.Auth;
using SAEON.Logs;
using SAEON.Observations.Auth;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI
{
    #region ApiControllers
    [Authorize(Policy = TenantAuthenticationDefaults.TenantPolicy)]
    [ApiController]
#if ResponseCaching
    [ResponseCache(Duration = Defaults.CacheDuration)]
    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
#endif
    [Authorize(Policy = ODPAuthenticationDefaults.AccessTokenPolicy)]
    public abstract class BaseApiController : ControllerBase
    {
        private IConfiguration config;
        protected IConfiguration Config => config ??= HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        protected bool TrackChanges { get; set; } = false;
        private ObservationsDbContext dbContext;
        protected ObservationsDbContext DbContext
        {
            get
            {
                if (dbContext is null)
                {
                    dbContext = HttpContext.RequestServices.GetRequiredService<ObservationsDbContext>();
                    if (TrackChanges)
                    {
                        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                    }
                }
                return dbContext;
            }
        }
    }

    public abstract class BaseListController<TObject> : BaseApiController where TObject : class
    {
        protected virtual List<TObject> GetList()
        {
            var result = new List<TObject>();
            return result;
        }

        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public virtual ActionResult<IEnumerable<TObject>> GetAll()
        {
            using (SAEONLogs.MethodCall<TObject>(GetType()))
            {
                try
                {
                    return Ok(GetList().AsEnumerable());
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get all");
                    throw;
                }
            }
        }
    }

    public class OrderBy<TEntity>
    {
        public Expression<Func<TEntity, object>> Expression { get; set; }
        public bool Ascending { get; set; } = true;

        public OrderBy() { }
        public OrderBy(Expression<Func<TEntity, object>> expression)
        {
            Expression = expression;
        }

        public OrderBy(Expression<Func<TEntity, object>> expression, bool ascending)
        {
            Expression = expression;
            Ascending = ascending;
        }
    }

    public abstract class BaseReadController<TEntity> : BaseApiController where TEntity : BaseEntity
    {
        [NotMapped]
        public string EntitySetName { get; protected set; }
        [NotMapped]
        public List<string> NavigationLinks { get; } = new List<string>();

        protected virtual List<Expression<Func<TEntity, bool>>> GetWheres()
        {
            return new List<Expression<Func<TEntity, bool>>>();
        }

        protected virtual List<OrderBy<TEntity>> GetOrderBys()
        {
            return new List<OrderBy<TEntity>>();
        }

        protected virtual List<Expression<Func<TEntity, object>>> GetIncludes()
        {
            return new List<Expression<Func<TEntity, object>>>();
        }

        protected abstract void UpdateRequest();

        protected virtual IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> extraWhere = null)
        {
            var query = DbContext.Set<TEntity>()/*.AsNoTracking()*/.AsQueryable();
            foreach (var include in GetIncludes())
            {
                query = query.Include(include);
            }
            foreach (var where in GetWheres())
            {
                query = query.Where(where);
            }
            if (extraWhere is not null)
            {
                query = query.Where(extraWhere);
            }
            var orderBys = GetOrderBys();
            //SAEONLogs.Verbose("OrderBys: {orderBys}", orderBys?.Count);
            var orderBy = orderBys.FirstOrDefault();
            if (orderBy is not null)
            {
                IOrderedQueryable<TEntity> orderedQuery;
                if (orderBy.Ascending)
                    orderedQuery = query.OrderBy(orderBy.Expression);
                else
                    orderedQuery = query.OrderByDescending(orderBy.Expression);
                foreach (var thenBy in orderBys.Skip(1))
                {
                    if (thenBy.Ascending)
                        orderedQuery = orderedQuery.ThenBy(orderBy.Expression);
                    else
                        orderedQuery = orderedQuery.ThenByDescending(orderBy.Expression);
                }
                query = orderedQuery;
            }
            return query;
        }

        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async virtual Task<IEnumerable<TEntity>> GetAll()
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    UpdateRequest();
                    SAEONLogs.Verbose("Uri: {Uri}", Request.GetUri());
                    return await GetQuery().ToListAsync();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }
    }

    public abstract class BaseIdedReadController<TEntity> : BaseReadController<TEntity> where TEntity : IdedEntity
    {
        [HttpGet("{id:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public virtual async Task<ActionResult<TEntity>> GetById(Guid id)
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "Id", id } }))
            {
                try
                {
                    TEntity item = await GetQuery(i => (i.Id == id)).FirstOrDefaultAsync();
                    if (item is null)
                    {
                        SAEONLogs.Error("{id} not found", id);
                        return NotFound();
                    }
                    return Ok(item);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }
    }
    #endregion

    #region ODataControllers
    [Authorize(Policy = TenantAuthenticationDefaults.TenantPolicy)]
    [ApiExplorerSettings(IgnoreApi = true)]
#if ResponseCaching
    [ResponseCache(Duration = Defaults.CacheDuration)]
    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
#endif
    [Authorize(Policy = ODPAuthenticationDefaults.AccessTokenPolicy)]
    public abstract class BaseODataController<TEntity> : ODataController where TEntity : BaseEntity
    {
        public static string BaseUrl { get; set; }

        private ObservationsDbContext _dbContext;
        protected ObservationsDbContext DbContext => _dbContext ??= HttpContext.RequestServices.GetRequiredService<ObservationsDbContext>();

        protected virtual List<Expression<Func<TEntity, bool>>> GetWheres()
        {
            return new List<Expression<Func<TEntity, bool>>>();
        }

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
            var query = DbContext.Set<TEntity>()/*.AsNoTracking()*/.AsQueryable();
            foreach (var where in GetWheres())
            {
                query = query.Where(where);
            }
            if (extraWhere is not null)
            {
                query = query.Where(extraWhere);
            }
            foreach (var orderBy in GetOrderBys())
            {
                query = query.OrderBy(orderBy);
            }
            return query;
        }

        /*
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
        */

        protected abstract void UpdateRequest();

        [HttpGet]
        [EnableQuery(PageSize = ODataDefaults.PageSize, MaxTop = ODataDefaults.MaxTop)]
        public virtual ActionResult<IQueryable<TEntity>> Get()
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    UpdateRequest();
                    SAEONLogs.Verbose("uri: {uri}", Request.GetUri());
                    return Ok(GetQuery().Take(ODataDefaults.MaxAll));
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get all");
                    throw;
                }
            }
        }
    }
    #endregion
}

