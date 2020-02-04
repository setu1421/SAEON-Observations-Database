using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.AspNet.OData;
using SAEON.AspNet.WebApi;
using SAEON.Logs;
using db = SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;

namespace SAEON.Observations.SensorThings
{
    public abstract class SensorThingsController<TSensorThingsEntity, TDbEntity> : ODataController where TSensorThingsEntity : SensorThingsEntity, new() where TDbEntity : db.BaseIDEntity
    {
        private db.ObservationsDbContext dbContext = null;
        protected db.ObservationsDbContext DbContext
        {
            get
            {
                if (dbContext == null) dbContext = new db.ObservationsDbContext(TenantAuthorizationAttribute.GetTenantFromHeaders(Request));
                return dbContext;
            }
        }
        protected IMapper Mapper { get; private set; } = null;
        private Converter converter = null;
        protected Converter Converter
        {
            get
            {
                if (converter == null) converter = new Converter(DbContext, Mapper);
                return converter;
            }
        }

        public SensorThingsController() : base()
        {
            var config = new MapperConfigurationExpression();
            config.CreateMap<db.SensorThingsDatastream, Datastream>();
            config.CreateMap<db.SensorThingsLocation, HistoricalLocation>();
            config.CreateMap<db.SensorThingsLocation, Location>();
            config.CreateMap<db.SensorThingsObservedProperty, ObservedProperty>();
            config.CreateMap<db.SensorThingsSensor, Sensor>();
            config.CreateMap<db.SensorThingsThing, Thing>();
            Mapper = new MapperConfiguration(config).CreateMapper();
        }

        protected T ConvertDbEntity<T>(TDbEntity dbEntity) where T : SensorThingsEntity
        {
            using (Logging.MethodCall<T, TDbEntity>(GetType()))
            {
                object result = default(T);
                switch (dbEntity)
                {
                    case db.SensorThingsDatastream dbDatastream:
                        result = Converter.ConvertDatastream(dbDatastream);
                        break;
                    case db.SensorThingsLocation dbLocation when typeof(T) == typeof(Location):
                        result = Converter.ConvertLocation(dbLocation);
                        break;
                    case db.SensorThingsLocation dbLocation when typeof(T) == typeof(HistoricalLocation):
                        result = Converter.ConvertHistoricalLocation(dbLocation);
                        break;
                    case db.SensorThingsObservedProperty dbObservedProperty:
                        result = Converter.ConvertObservedProperty(dbObservedProperty);
                        break;
                    case db.SensorThingsSensor dbSensor:
                        result = Converter.ConvertSensor(dbSensor);
                        break;
                    case db.SensorThingsThing dbThing:
                        result = Converter.ConvertThing(dbThing);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                return (T)result;
            }
        }

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
                result.Add(ConvertDbEntity<TSensorThingsEntity>(dbEntity));
            }
            return result.AsQueryable();
        }

        protected IQueryable<TSensorThingsEntity> GetSensorThingsSingle(Guid id)
        {
            var result = new List<TSensorThingsEntity>();
            var dbEntity = GetDbAll(i => i.Id == id).FirstOrDefault();
            if (dbEntity != null)
            {
                result.Add(ConvertDbEntity<TSensorThingsEntity>(dbEntity));
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
            using (Logging.MethodCall<SingleResult<TDbEntity>>(GetType(), new MethodCallParameters { { "Id", id } }))
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
