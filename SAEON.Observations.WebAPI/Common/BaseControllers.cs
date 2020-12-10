//#define ODPAuth
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SAEON.Logs;
using SAEON.Observations.Auth;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;

namespace SAEON.Observations.WebAPI
{
    #region ApiControllers
    [Authorize(Policy = TenantAuthenticationDefaults.TenantPolicy)]
#if ODPAuth
    [Authorize(Policy = ODPAuthenticationDefaults.AccessTokenPolicy)]
#endif
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        private IConfiguration config;
        protected IConfiguration Config => config ??= HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        private ObservationsDbContext dbContext;
        protected ObservationsDbContext DbContext => dbContext ??= HttpContext.RequestServices.GetRequiredService<ObservationsDbContext>();
    }

    public abstract class BaseApiListController<TObject> : BaseApiController where TObject : class
    {
        protected virtual List<TObject> GetList()
        {
            var result = new List<TObject>();
            return result;
        }

        [HttpGet]
        public virtual List<TObject> GetAll()
        {
            using (SAEONLogs.MethodCall<TObject>(GetType()))
            {
                try
                {
                    return GetList();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get all");
                    throw;
                }
            }
        }
    }

    public abstract class BaseApiEntityController<TEntity> : BaseApiController where TEntity : BaseEntity
    {
        [NotMapped]
        public string EntitySetName { get; protected set; }
        [NotMapped]
        public List<string> NavigationLinks { get; } = new List<string>();

        protected abstract void UpdateRequest();

        protected virtual List<Expression<Func<TEntity, object>>> GetOrderBys()
        {
            return new List<Expression<Func<TEntity, object>>>();
        }

        protected virtual IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> extraWhere = null)
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

        [HttpGet]
        public virtual IQueryable<TEntity> GetAll()
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    UpdateRequest();
                    SAEONLogs.Verbose("Uri: {Uri}", Request.GetUri());
                    return GetQuery();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }
    }
    #endregion

    #region MvcControllers
    [Authorize(Policy = TenantAuthenticationDefaults.TenantPolicy)]
    public abstract class BaseMvcController : Controller
    {
        private IConfiguration config;
        protected IConfiguration Config => config ??= HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        private ObservationsDbContext dbContext;
        protected ObservationsDbContext DbContext => dbContext ??= HttpContext.RequestServices.GetRequiredService<ObservationsDbContext>();
    }
    #endregion

    #region ODataControllers
    [Authorize(Policy = TenantAuthenticationDefaults.TenantPolicy)]
#if ODPAuth
    [Authorize(Policy = ODPAuthenticationDefaults.AccessTokenPolicy)]
#endif
    [ApiExplorerSettings(IgnoreApi = true)]
    public abstract class BaseODataController<TEntity> : ODataController where TEntity : BaseEntity
    {
        public static string BaseUrl { get; set; }

        private ObservationsDbContext _dbContext;
        protected ObservationsDbContext DbContext => _dbContext ??= HttpContext.RequestServices.GetRequiredService<ObservationsDbContext>();

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

        [EnableQuery(PageSize = ODataDefaults.PageSize, MaxTop = ODataDefaults.MaxTop)]
        public virtual IQueryable<TEntity> Get()
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    UpdateRequest();
                    SAEONLogs.Verbose("uri: {uri}", Request.GetUri());
                    return GetQuery().Take(ODataDefaults.MaxAll);
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

