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
        protected const int MaxAll = 1000;

        private ObservationsDbContext dbContext = null;
        protected ObservationsDbContext DbContext
        {
            get
            {
                if (dbContext == null) dbContext = new ObservationsDbContext(TenantAuthorizationAttribute.GetTenantFromHeaders(Request));
                return dbContext;
            }
            private set => dbContext = value;
        }

        /// <summary>
        /// Overwrite to filter entities
        /// </summary>
        /// <returns>ListOf(PredicateOf(TEntity))</returns>
        protected virtual List<Expression<Func<TEntity, bool>>> GetWheres()
        {
            return new List<Expression<Func<TEntity, bool>>>();
        }

        /// <summary>
        /// Overwrite to order of entities
        /// </summary>
        /// <returns>ListOf(PredicateOf(TEntity))</returns>
        protected virtual List<Expression<Func<TEntity, object>>> GetOrderBys()
        {
            return new List<Expression<Func<TEntity, object>>>();
        }

        /// <summary>
        /// Overwrite for entity includes
        /// </summary>
        /// <returns>ListOf(PredicateOf(TEntity))</returns>
        protected virtual List<Expression<Func<TEntity, bool>>> GetIncludes()
        {
            return new List<Expression<Func<TEntity, bool>>>();
        }

        /// <summary>
        /// query for items
        /// </summary>
        /// <returns></returns>
        protected IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> extraWhere = null)
        {
            var query = DbContext.Set<TEntity>().AsQueryable().AsNoTracking();
            foreach (var include in GetIncludes())
            {
                query = query.Include(include);
            }
            foreach (var where in GetWheres())
            {
                query = query.Where(where);
            }
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

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <returns>ListOf(TEntity)</returns>
        // GET: odata/TEntity
        //[EnableQuery, ODataRoute] Required in derived class
        public virtual IQueryable<TEntity> GetAll()
        {
            using (Logging.MethodCall<TEntity>(GetType()))
            {
                try
                {
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
        //[EnableQuery, ODataRoute("({id})")] Required in derived class
        public virtual SingleResult<TEntity> GetById([FromODataUri] Guid id)
        {
            using (Logging.MethodCall<SingleResult<TEntity>>(GetType(), new MethodCallParameters { { "Id", id } }))
            {
                try
                {
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
        /// <param name="include">Lamda to include TRelated.TEntity</param>
        /// <returns>SingleResultOf(TRelated)</returns>
        // GET: odata/TEntity(5)/TRelated
        //[EnableQuery, ODataRoute("({id})/TRelated")] Required in derived class
        protected SingleResult<TRelated> GetSingle<TRelated>(Guid id, Expression<Func<TEntity, TRelated>> select, Expression<Func<TRelated, TEntity>> include) where TRelated : GuidIdEntity
        {
            using (Logging.MethodCall<SingleResult<TRelated>>(GetType()))
            {
                try
                {
                    return SingleResult.Create(GetQuery(i => i.Id == id).Select(select).Include(include));
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
        /// <param name="include">Lamda to include TRelated.ListOf(TEntity)</param>
        /// <returns>SingleResultOf(TRelated)</returns>
        // GET: odata/TEntity(5)/TRelated
        //[EnableQuery, ODataRoute("({id})/TRelated")] Required in derived class
        protected SingleResult<TRelated> GetSingle<TRelated>(Guid id, Expression<Func<TEntity, TRelated>> select, Expression<Func<TRelated, IEnumerable<TEntity>>> include) where TRelated : GuidIdEntity
        {
            using (Logging.MethodCall<SingleResult<TRelated>>(GetType()))
            {
                try
                {
                    return SingleResult.Create(GetQuery(i => (i.Id == id)).Select(select).Include(include));
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
        /// <param name="include">Lambda to include TRelated.TEntity</param>
        /// <returns>IQueryableOf(TRelated)</returns>
        // GET: odata/TEntity(5)/TRelated
        //[EnableQuery, ODataRoute("({id})/TRelated")] Required in derived class
        protected IQueryable<TRelated> GetMany<TRelated>(Guid id, Expression<Func<TEntity, IEnumerable<TRelated>>> select, Expression<Func<TRelated, TEntity>> include) where TRelated : GuidIdEntity
        {
            using (Logging.MethodCall<TRelated>(GetType()))
            {
                try
                {
                    return GetQuery(i => i.Id == id).SelectMany(select).Include(include).Take(MaxAll);
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
        /// <param name="include">Lambda to include TRelated.ListOf(TEntity)</param>
        /// <returns>IQueryableOf(TRelated)</returns>
        // GET: odata/TEntity(5)/TRelated
        //[EnableQuery, ODataRoute("({id})/TRelated")] Required in derived class
        protected IQueryable<TRelated> GetMany<TRelated>(Guid id, Expression<Func<TEntity, IEnumerable<TRelated>>> select, Expression<Func<TRelated, IEnumerable<TEntity>>> include) where TRelated : GuidIdEntity
        {
            using (Logging.MethodCall<TRelated>(GetType()))
            {
                try
                {
                    return GetQuery(i => i.Id == id).SelectMany(select).Include(include).Take(MaxAll);
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

