using Microsoft.AspNet.OData;
using SAEON.Logs;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    /// <summary>
    /// Base OData controller for Entities with surrogate IDs
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    [ApiExplorerSettings(IgnoreApi = true)]
    [ODataRouteName("Internal")]
    //[ClientAuthorization("SAEON.Observations.QuerySite")] Uncomment when going live
    public abstract class BaseODataController<TEntity> : ODataController where TEntity : BaseEntity
    {
        protected readonly ObservationsDbContext db = null;

        public BaseODataController()
        {
            db = new ObservationsDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Overwrite to order of entities
        /// </summary>
        /// <returns>ListOf(PredicateOf(TEntity))</returns>
        protected virtual List<Expression<Func<TEntity, object>>> GetOrderBys()
        {
            var result = new List<Expression<Func<TEntity, object>>>();
            return result;
        }

        /// <summary>
        /// query for items
        /// </summary>
        /// <returns></returns>
        protected IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> extraWhere = null)
        {
            var query = db.Set<TEntity>().AsQueryable().AsNoTracking();
            //foreach (var include in GetIncludes())
            //{
            //    query = query.Include(include);
            //}
            //foreach (var where in GetWheres())
            //{
            //    query = query.Where(where);
            //}
            //if (extraWhere != null)
            //{
            //    query = query.Where(extraWhere);
            //}
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
        //[EnableQuery, ODataRoute] // Required in derived class
        public virtual IQueryable<TEntity> GetAll()
        {
            using (Logging.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    return GetQuery();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get all");
                    throw;
                }
            }
        }


    }
}