using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.AspNet.OData;
using SAEON.AspNet.WebApi;
using SAEON.Logs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;
using db = SAEON.Observations.Core.Entities;

namespace SAEON.Observations.SensorThings
{
    public abstract class SensorThingsController<TEntity, TDbEntity> : ODataController where TEntity : SensorThingsEntity, new() where TDbEntity : db.BaseIDEntity
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

        protected SensorThingsController() : base()
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

        private T ConvertDbEntity<T, TDb>(TDb dbEntity) where T : SensorThingsEntity where TDb : db.BaseIDEntity
        {
            using (Logging.MethodCall<T, TDb>(GetType()))
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

        private T ConvertDbEntity<T>(TDbEntity dbEntity) where T : SensorThingsEntity
        {
            return ConvertDbEntity<T, TDbEntity>(dbEntity);
        }

        ///// <summary>
        ///// Overwrite to filter entities
        ///// </summary>
        ///// <returns>ListOf(PredicateOf(TEntity))</returns>
        //protected virtual List<Expression<Func<TDbEntity, bool>>> GetWheres()
        //{
        //    return new List<Expression<Func<TDbEntity, bool>>>();
        //}

        ///// <summary>
        ///// Overwrite to order of entities
        ///// </summary>
        ///// <returns>ListOf(PredicateOf(TEntity))</returns>
        //protected virtual List<Expression<Func<TDbEntity, object>>> GetOrderBys()
        //{
        //    return new List<Expression<Func<TDbEntity, object>>>();
        //}

        ///// <summary>
        ///// Overwrite for entity includes
        ///// </summary>
        ///// <returns>ListOf(PredicateOf(TEntity))</returns>
        //protected virtual List<Expression<Func<TDbEntity, bool>>> GetIncludes()
        //{
        //    return new List<Expression<Func<TDbEntity, bool>>>();
        //}

        /// <summary>
        /// query for items
        /// </summary>
        /// <returns></returns>
        protected IQueryable<TDbEntity> GetDbAll(Expression<Func<TDbEntity, bool>> extraWhere = null)
        {
            var query = DbContext.Set<TDbEntity>().AsQueryable().AsNoTracking();
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
            //foreach (var orderBy in GetOrderBys())
            //{
            //    query = query.OrderBy(orderBy);
            //}
            return query;
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
        public virtual IQueryable<TEntity> GetAll()
        {
            using (Logging.MethodCall<TDbEntity>(GetType()))
            {
                try
                {
                    UpdateRequest(true);
                    Logging.Verbose("uri: {uri}", Request.RequestUri.ToString());
                    var result = new List<TEntity>();
                    foreach (var dbEntity in GetDbAll())
                    {
                        result.Add(ConvertDbEntity<TEntity>(dbEntity));
                    }
                    return result.AsQueryable();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get all");
                    throw;
                }
            }
        }

        //[EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})")] Required in derived class
        public virtual SingleResult<TEntity> GetById([FromODataUri] Guid id)
        {
            using (Logging.MethodCall<SingleResult<TDbEntity>>(GetType(), new MethodCallParameters { { "Id", id } }))
            {
                try
                {
                    UpdateRequest(false);
                    Logging.Verbose("uri: {uri}", Request.RequestUri.ToString());
                    var result = new List<TEntity>();
                    var dbEntity = GetDbAll(i => i.Id == id).FirstOrDefault();
                    if (dbEntity != null)
                    {
                        result.Add(ConvertDbEntity<TEntity>(dbEntity));
                    }
                    return SingleResult.Create(result.AsQueryable());
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        ////[EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/Related")] Required in derived class
        //protected virtual SingleResult<TRelatedSensorThingsEntity> GetRelatedSingle<TRelatedSensorThingsEntity>([FromODataUri]Guid id, Expression<Func<TEntity, TRelatedSensorThingsEntity>> select) where TRelatedSensorThingsEntity : SensorThingsEntity
        //{
        //    using (Logging.MethodCall<TEntity, TRelatedSensorThingsEntity>(GetType()))
        //    {
        //        try
        //        {
        //            UpdateRequest(false);
        //            Logging.Verbose("uri: {uri}", Request.RequestUri.ToString());
        //            var result = new List<TRelatedSensorThingsEntity>();
        //            var relatedEntity = GetSensorThingsSingle(id).Select(select).FirstOrDefault();
        //            if (relatedEntity != null)
        //            {
        //                result.Add(relatedEntity);
        //            }
        //            return SingleResult.Create(result.AsQueryable());
        //        }
        //        catch (Exception ex)
        //        {
        //            Logging.Exception(ex);
        //            throw;
        //        }
        //    }
        //}

        ////[EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/Related")] Required in derived class
        //protected virtual IQueryable<TRelatedSensorThingsEntity> GetRelatedMany<TRelatedSensorThingsEntity>([FromODataUri]Guid id, Expression<Func<TEntity, IEnumerable<TRelatedSensorThingsEntity>>> select) where TRelatedSensorThingsEntity : SensorThingsEntity
        //{
        //    using (Logging.MethodCall<TEntity, TRelatedSensorThingsEntity>(GetType()))
        //    {
        //        try
        //        {
        //            UpdateRequest(true);
        //            Logging.Verbose("uri: {uri}", Request.RequestUri.ToString());
        //            var result = new List<TRelatedSensorThingsEntity>();
        //            return GetSensorThingsSingle(id).SelectMany(select);
        //        }
        //        catch (Exception ex)
        //        {
        //            Logging.Exception(ex);
        //            throw;
        //        }
        //    }
        //}

        private IQueryable<TDbRelatedEntity> LoadRelatedMany<TDbRelatedEntity>(Guid id) where TDbRelatedEntity : db.BaseIDEntity
        {
            using (Logging.MethodCall<TDbEntity, TDbRelatedEntity>(GetType()))
            {
                try
                {
                    var dbEntity = DbContext.Set<TDbEntity>().First(i => i.Id == id);
                    switch (dbEntity)
                    {
                        // Datastream
                        case db.SensorThingsDatastream datastream when typeof(TDbRelatedEntity) == typeof(db.SensorThingsThing):
                            return (IQueryable<TDbRelatedEntity>)dbContext.SensorThingsThings.Where(i => i.Id == datastream.InstrumentId);
                        case db.SensorThingsDatastream datastream when typeof(TDbRelatedEntity) == typeof(db.SensorThingsSensor):
                            return (IQueryable<TDbRelatedEntity>)dbContext.SensorThingsSensors.Where(i => i.Id == datastream.Id);
                        case db.SensorThingsDatastream datastream when typeof(TDbRelatedEntity) == typeof(db.SensorThingsObservedProperty):
                            var dbDatastreamSensor = dbContext.SensorThingsSensors.First(i => i.Id == datastream.Id);
                            return (IQueryable<TDbRelatedEntity>)dbContext.SensorThingsObservedProperies.Where(i => i.Id == dbDatastreamSensor.PhenomenonOfferingId);
                        // Location
                        case db.SensorThingsLocation location when typeof(TDbRelatedEntity) == typeof(db.SensorThingsThing):
                            return (IQueryable<TDbRelatedEntity>)dbContext.SensorThingsThings.Where(i => i.Id == location.Id);
                        // ObservedProperty
                        case db.SensorThingsObservedProperty observedProperty when typeof(TDbRelatedEntity) == typeof(db.SensorThingsDatastream):
                            var dbObservedPropertySensor = dbContext.SensorThingsSensors.First(i => i.PhenomenonOfferingId == observedProperty.Id);
                            return (IQueryable<TDbRelatedEntity>)dbContext.SensorThingsDatastreams.Where(i => i.Id == dbObservedPropertySensor.PhenomenonOfferingId);
                        // Sensor
                        case db.SensorThingsSensor sensor when typeof(TDbRelatedEntity) == typeof(db.SensorThingsDatastream):
                            return (IQueryable<TDbRelatedEntity>)dbContext.SensorThingsDatastreams.Where(i => i.Id == sensor.Id);
                        // Thing
                        case db.SensorThingsThing thing when typeof(TDbRelatedEntity) == typeof(db.SensorThingsLocation):
                            return (IQueryable<TDbRelatedEntity>)dbContext.SensorThingsLocations.Where(i => i.Id == thing.Id);
                        case db.SensorThingsThing thing when typeof(TDbRelatedEntity) == typeof(db.SensorThingsDatastream):
                            return (IQueryable<TDbRelatedEntity>)dbContext.SensorThingsDatastreams.Where(i => i.InstrumentId == thing.Id);
                        default:
                            throw new NotImplementedException();
                    }
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        protected SingleResult<TRelatedEntity> GetDbRelatedSingle<TRelatedEntity, TDbRelatedEntity>(Guid id, Expression<Func<TDbEntity, TDbRelatedEntity>> select) where TDbRelatedEntity : db.BaseIDEntity where TRelatedEntity : SensorThingsEntity
        {
            using (Logging.MethodCall<TRelatedEntity, TDbRelatedEntity>(GetType()))
            {
                try
                {
                    UpdateRequest(true);
                    Logging.Verbose("uri: {uri}", Request.RequestUri.ToString());
                    var result = new List<TRelatedEntity>();
                    var dbRelatedEntities = LoadRelatedMany<TDbRelatedEntity>(id).Take(1);
                    foreach (var dbRelatedEntity in dbRelatedEntities)
                    {
                        result.Add(ConvertDbEntity<TRelatedEntity, TDbRelatedEntity>(dbRelatedEntity));
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

        protected IQueryable<TRelatedEntity> GetDbRelatedMany<TRelatedEntity, TDbRelatedEntity>(Guid id, Expression<Func<TDbEntity, TDbRelatedEntity>> select) where TDbRelatedEntity : db.BaseIDEntity where TRelatedEntity : SensorThingsEntity
        {
            using (Logging.MethodCall<TRelatedEntity, TDbRelatedEntity>(GetType()))
            {
                try
                {
                    UpdateRequest(true);
                    Logging.Verbose("uri: {uri}", Request.RequestUri.ToString());
                    var result = new List<TRelatedEntity>();
                    var dbRelatedEntities = LoadRelatedMany<TDbRelatedEntity>(id);
                    foreach (var dbRelatedEntity in dbRelatedEntities)
                    {
                        result.Add(ConvertDbEntity<TRelatedEntity, TDbRelatedEntity>(dbRelatedEntity));
                    }
                    return result.AsQueryable();
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
