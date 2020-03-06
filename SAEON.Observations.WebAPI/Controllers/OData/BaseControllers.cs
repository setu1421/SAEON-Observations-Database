using Microsoft.AspNet.OData;
using SAEON.AspNet.WebApi;
using SAEON.Logs;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ODataRouteName("OData")]
    [TenantAuthorization]
    public abstract class BaseController<TEntity> : ODataController where TEntity : BaseEntity
    {
        public static string BaseUrl { get; set; }
        protected const int PageSize = 25;
        protected const int MaxTop = 500;
        protected const int MaxAll = 10000;

        private ObservationsDbContext dbContext = null;
        protected ObservationsDbContext DbContext => dbContext ?? (dbContext = new ObservationsDbContext(TenantAuthorizationAttribute.GetTenantFromHeaders(Request)));

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

        protected void UpdateRequest(bool isMany)
        {
            BaseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority) + "/OData";
            var uri = Request.RequestUri.ToString();
            if (isMany && !uri.ToLowerInvariant().Contains("$expand="))
            {
                if (!uri.ToLowerInvariant().Contains("$count=true"))
                {
                    if (!uri.Contains("?")) uri += "?";
                    uri += "$count=true";
                    Request.RequestUri = new Uri(uri);
                }
            }
            Request.Headers.TryAddWithoutValidation("Prefer", "odata.include-annotations=*");
        }


        /// <summary>
        /// Get all entities
        /// </summary>
        /// <returns>ListOf(TEntity)</returns>
        // GET: odata/TEntity
        //[ODataRoute] required on derived class
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        public virtual IQueryable<TEntity> GetAll()
        {
            using (Logging.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    UpdateRequest(true);
                    Logging.Verbose("uri: {uri}", Request.RequestUri.ToString());
                    return GetQuery().Take(MaxAll);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get all");
                    throw;
                }
            }
        }

    }

    public abstract class NamedController<TEntity> : BaseController<TEntity> where TEntity : NamedEntity
    {
        protected override List<Expression<Func<TEntity, object>>> GetOrderBys()
        {
            var result = base.GetOrderBys();
            result.Add(i => i.Name);
            return result;
        }

        /// <summary>
        /// Get TEntity by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>TEntity</returns>
        // GET: odata/TEntity(5)
        //[ODataRoute("({id})")] required on derived class
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        public virtual SingleResult<TEntity> GetById([FromODataUri] Guid id)
        {
            using (Logging.MethodCall<SingleResult<TEntity>>(GetType(), new MethodCallParameters { { "Id", id } }))
            {
                try
                {
                    UpdateRequest(false);
                    Logging.Verbose("uri: {uri}", Request.RequestUri.ToString());
                    return SingleResult.Create(GetQuery(i => (i.Id == id)));
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        /// <summary>
        /// Related Entity TEntity.TRelated
        /// </summary>
        /// <typeparam name="TRelated"></typeparam>
        /// <param name="id">Id of TEntity</param>
        /// <param name="select">Lambda to select TRelated</param>
        /// <returns>TRelated</returns>
        // GET: odata/TEntity(5)/TRelated
        //[ODataRoute("({id})/TRelated")] required on calling class
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        protected TRelated GetSingle<TRelated>(Guid id, Expression<Func<TEntity, TRelated>> select) where TRelated : GuidIdEntity
        {
            using (Logging.MethodCall<SingleResult<TRelated>>(GetType()))
            {
                try
                {
                    UpdateRequest(false);
                    Logging.Verbose("uri: {uri}", Request.RequestUri.ToString());
                    return GetQuery(i => i.Id == id).Select(select).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        /// <summary>
        /// Get IQueryableOf(TRelated)
        /// </summary>
        /// <typeparam name="TRelated"></typeparam>
        /// <param name="id">Id of TEntity</param>
        /// <param name="select">Lambda to select ListOf(TRelated)</param>
        /// <returns>IQueryableOf(TRelated)</returns>
        // GET: odata/TEntity(5)/TRelated
        //[ODataRoute("({id})/TRelated")] required on calling class
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        protected IQueryable<TRelated> GetMany<TRelated>(Guid id, Expression<Func<TEntity, IEnumerable<TRelated>>> select) where TRelated : GuidIdEntity
        {
            using (Logging.MethodCall<TRelated>(GetType()))
            {
                try
                {
                    UpdateRequest(true);
                    Logging.Verbose("uri: {uri}", Request.RequestUri.ToString());
                    return GetQuery(i => i.Id == id).SelectMany(select).Take(MaxAll);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }
    }
}

