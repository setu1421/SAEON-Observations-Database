using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using SAEON.Observations.Core.SensorThings;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.OData;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
{
    [ODataRouteName("SensorThings")]
    public class BaseSensorThingsControllers<TEntity> : ODataController where TEntity : BaseSensorThingEntity
    {
        protected ObservationsDbContext db = new ObservationsDbContext();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        protected virtual List<TEntity> GetList()
        {
            return new List<TEntity>();
        }

        protected IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> extraWhere = null)
        {
            var list = GetList();
            list.ForEach(i => i.GenerateSensorThingsProperties());
            var query = list.AsQueryable();
            if (extraWhere != null)
            {
                query = query.Where(extraWhere);
            }
            return query;
        }

        //[EnableQuery, ODataRoute] Required in derived class
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

        /// <summary>
        /// Get TEntity by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>TEntity</returns>
        //[EnableQuery, ODataRoute("({id})")] Required in derived class
        public virtual SingleResult<TEntity> GetById([FromODataUri] Guid id)
        {
            using (Logging.MethodCall<SingleResult<TEntity>>(GetType(), new ParameterList { { "Id", id } }))
            {
                try
                {
                    return SingleResult.Create(GetQuery(i => (i.id == id)));
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
        /// <param name="include">Lamda to include TRelated.ListOf(TEntrity)</param>
        /// <returns>SingleResultOf(TRelated)</returns>
        // GET: odata/TEntity(5)/TRelated
        //[EnableQuery, ODataRoute("({id})/TRelated")] Required in derived class
        protected SingleResult<TRelated> GetSingle<TRelated>(Guid id, Expression<Func<TEntity, TRelated>> select, Expression<Func<TRelated, IEnumerable<TEntity>>> include) where TRelated : BaseSensorThingEntity
        {
            using (Logging.MethodCall<SingleResult<TRelated>>(GetType()))
            {
                try
                {
                    return SingleResult.Create(GetQuery(i => (i.id == id)).Select(select).Include(include));
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
        protected IQueryable<TRelated> GetMany<TRelated>(Guid id, Expression<Func<TEntity, IEnumerable<TRelated>>> select, Expression<Func<TRelated, TEntity>> include) where TRelated : BaseSensorThingEntity
        {
            using (Logging.MethodCall<TRelated>(GetType()))
            {
                try
                {
                    return GetQuery(i => i.id == id).SelectMany(select).Include(include);
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
        protected IQueryable<TRelated> GetMany<TRelated>(Guid id, Expression<Func<TEntity, IEnumerable<TRelated>>> select, Expression<Func<TRelated, IEnumerable<TEntity>>> include) where TRelated : BaseSensorThingEntity
        {
            using (Logging.MethodCall<TRelated>(GetType()))
            {
                try
                {
                    return GetQuery(i => i.id == id).SelectMany(select).Include(include);
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