using AutoMapper;
using AutoMapper.Configuration;
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

namespace SAEON.Observations.SensorThings
{
    public abstract class SensorThingsController<TSensorThingsEntity, TDbEntity> : ODataController where TSensorThingsEntity : SensorThingsEntity, new() where TDbEntity : BaseIDEntity
    {
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
        protected IMapper Mapper { get; private set; } = null;

        public SensorThingsController() : base()
        {
            Mapper = GetMapperConfiguration().CreateMapper();
        }

        protected virtual void CreateRelatedMappings(MapperConfigurationExpression cfg) { }

        protected MapperConfiguration GetMapperConfiguration()
        {
            var config = new MapperConfigurationExpression();
            config.CreateMap<TDbEntity, TSensorThingsEntity>();
            CreateRelatedMappings(config);
            return new MapperConfiguration(config);
        }

        protected abstract TSensorThingsEntity ConvertDbEntity(TDbEntity dbEnitity);

        /// <summary>
        /// Overwrite to filter entities
        /// </summary>
        /// <returns>ListOf(PredicateOf(TEntity))</returns>
        protected virtual List<Expression<Func<TDbEntity, bool>>> GetWheres()
        {
            return new List<Expression<Func<TDbEntity, bool>>>();
        }

        /// <summary>
        /// Overwrite to order of entities
        /// </summary>
        /// <returns>ListOf(PredicateOf(TEntity))</returns>
        protected virtual List<Expression<Func<TDbEntity, object>>> GetOrderBys()
        {
            return new List<Expression<Func<TDbEntity, object>>>();
        }

        /// <summary>
        /// Overwrite for entity includes
        /// </summary>
        /// <returns>ListOf(PredicateOf(TEntity))</returns>
        protected virtual List<Expression<Func<TDbEntity, bool>>> GetIncludes()
        {
            return new List<Expression<Func<TDbEntity, bool>>>();
        }

        /// <summary>
        /// query for items
        /// </summary>
        /// <returns></returns>
        protected IQueryable<TDbEntity> GetDbAll(Expression<Func<TDbEntity, bool>> extraWhere = null)
        {
            var query = DbContext.Set<TDbEntity>().AsQueryable().AsNoTracking();
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

        protected IQueryable<TSensorThingsEntity> GetSensorThingsMany()
        {
            var result = new List<TSensorThingsEntity>();
            foreach (var dbEntity in GetDbAll())
            {
                result.Add(ConvertDbEntity(dbEntity));
            }
            return result.AsQueryable();
        }

        protected IQueryable<TSensorThingsEntity> GetSensorThingsSingle(Guid id)
        {
            var result = new List<TSensorThingsEntity>();
            var dbEntity = GetDbAll(i => i.Id == id).FirstOrDefault();
            if (dbEntity != null)
            {
                result.Add(ConvertDbEntity(dbEntity));
            }
            return result.AsQueryable();
        }

        private void UpdateRequest(bool isMany)
        {
            Config.BaseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority) + "/SensorThings";
            if (isMany)
            {
                var uri = Request.RequestUri.ToString();
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
        //[EnableQuery(PageSize = Config.PageSize), ODataRoute] Required in derived class
        public virtual IQueryable<TSensorThingsEntity> GetAll()
        {
            using (Logging.MethodCall<TDbEntity>(GetType()))
            {
                try
                {
                    UpdateRequest(true);
                    Logging.Verbose("uri: {uri}", Request.RequestUri.ToString());
                    return GetSensorThingsMany();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get all");
                    throw;
                }
            }
        }

        //[EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})")] Required in derived class
        public virtual SingleResult<TSensorThingsEntity> GetById([FromODataUri] Guid id)
        {
            using (Logging.MethodCall<SingleResult<TDbEntity>>(GetType(), new ParameterList { { "Id", id } }))
            {
                try
                {
                    UpdateRequest(false);
                    Logging.Verbose("uri: {uri}", Request.RequestUri.ToString());
                    return SingleResult.Create(GetSensorThingsSingle(id));
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        //[EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/Related")] Required in derived class
        protected virtual SingleResult<TRelatedSensorThingsEntity> GetRelatedSingle<TRelatedSensorThingsEntity>([FromODataUri]Guid id, Expression<Func<TSensorThingsEntity, TRelatedSensorThingsEntity>> select) where TRelatedSensorThingsEntity : SensorThingsEntity
        {
            using (Logging.MethodCall<TSensorThingsEntity, TRelatedSensorThingsEntity>(GetType()))
            {
                try
                {
                    UpdateRequest(false);
                    Logging.Verbose("uri: {uri}", Request.RequestUri.ToString());
                    var result = new List<TRelatedSensorThingsEntity>();
                    var relatedEntity = GetSensorThingsSingle(id).Select(select).FirstOrDefault();
                    if (relatedEntity != null)
                    {
                        result.Add(relatedEntity);
                    }
                    return SingleResult.Create(result.AsQueryable());
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        //[EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/Related")] Required in derived class
        protected virtual IQueryable<TRelatedSensorThingsEntity> GetRelatedMany<TRelatedSensorThingsEntity>([FromODataUri]Guid id, Expression<Func<TSensorThingsEntity, IEnumerable<TRelatedSensorThingsEntity>>> select) where TRelatedSensorThingsEntity : SensorThingsEntity
        {
            using (Logging.MethodCall<TSensorThingsEntity, TRelatedSensorThingsEntity>(GetType()))
            {
                try
                {
                    UpdateRequest(false);
                    Logging.Verbose("uri: {uri}", Request.RequestUri.ToString());
                    var result = new List<TRelatedSensorThingsEntity>();
                    return GetSensorThingsSingle(id).SelectMany(select);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }
    }
}
