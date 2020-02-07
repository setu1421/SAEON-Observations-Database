using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.AspNet.OData;
using Microsoft.Spatial;
using SAEON.AspNet.WebApi;
using SAEON.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
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
                        result = ConvertDatastream(dbDatastream);
                        break;
                    case db.SensorThingsLocation dbLocation when typeof(T) == typeof(Location):
                        result = ConvertLocation(dbLocation);
                        break;
                    case db.SensorThingsLocation dbLocation when typeof(T) == typeof(HistoricalLocation):
                        result = ConvertHistoricalLocation(dbLocation);
                        break;
                    case db.SensorThingsObservedProperty dbObservedProperty:
                        result = ConvertObservedProperty(dbObservedProperty);
                        break;
                    case db.SensorThingsSensor dbSensor:
                        result = ConvertSensor(dbSensor);
                        break;
                    case db.SensorThingsThing dbThing:
                        result = ConvertThing(dbThing);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                return (T)result;
            }

            Datastream ConvertDatastream(db.SensorThingsDatastream dbDatastream)
            {
                GeographyPolygon CreatePolygon(double top, double left, double bottom, double right)
                {
                    return GeographyFactory.Polygon().Ring(left, top).LineTo(left, top).LineTo(right, top).LineTo(right, bottom).LineTo(left, bottom).LineTo(left, top).Build();
                }

                using (Logging.MethodCall(GetType()))
                {
                    var result = Mapper.Map<Datastream>(dbDatastream);
                    result.UnitOfMeasurement = new UnitOfMeasurement
                    {
                        Name = $"{dbDatastream.PhenomenonName} - {dbDatastream.OfferingName} - {dbDatastream.UnitOfMeasureUnit}",
                        Symbol = dbDatastream.UnitOfMeasureSymbol,
                        Definition = string.IsNullOrWhiteSpace(dbDatastream.PhenomenonUrl) ? "http://data.saeon.ac.za" : dbDatastream.PhenomenonUrl
                    };
                    result.Name = $"{dbDatastream.Name} - {dbDatastream.PhenomenonName} - {dbDatastream.OfferingName} - {dbDatastream.UnitOfMeasureUnit}";
                    result.Description = $"{dbDatastream.Description} - {dbDatastream.PhenomenonDescription} - {dbDatastream.OfferingDescription} - {dbDatastream.UnitOfMeasureUnit}";
                    if (dbDatastream.LatitudeNorth.HasValue && dbDatastream.LongitudeWest.HasValue && dbDatastream.LatitudeSouth.HasValue && dbDatastream.LongitudeEast.HasValue)
                    {
                        result.ObservedArea = CreatePolygon(dbDatastream.LatitudeNorth.Value, dbDatastream.LongitudeWest.Value, dbDatastream.LatitudeSouth.Value, dbDatastream.LongitudeEast.Value);
                    }
                    if (dbDatastream.StartDate.HasValue && dbDatastream.EndDate.HasValue)
                    {
                        result.PhenomenonTime = new TimeInterval(dbDatastream.StartDate.Value, dbDatastream.EndDate.Value);
                        result.ResultTime = new TimeInterval(dbDatastream.StartDate.Value, dbDatastream.EndDate.Value);
                    }
                    //Logging.Verbose("Result: {@Result}", result);
                    return result;
                }
            }

            HistoricalLocation ConvertHistoricalLocation(db.SensorThingsLocation dbLocation, Location location = null, Thing thing = null)
            {
                using (Logging.MethodCall(GetType()))
                {
                    if (location == null)
                    {
                        location = ConvertLocation(dbLocation);
                    }
                    if (thing == null)
                    {
                        var dbThing = DbContext.SensorThingsThings.First(i => i.Id == dbLocation.Id);
                        thing = ConvertThing(dbThing);
                    }
                    var result = new HistoricalLocation { Id = location.Id, Time = dbLocation.StartDate ?? dbLocation.EndDate ?? DateTime.Now, Thing = thing, Locations = new List<Location> { location } };
                    //Logging.Verbose("Result: {@Result}", result);
                    return result;
                }
            }

            Location ConvertLocation(db.SensorThingsLocation dbLocation, Thing thing = null)
            {
                using (Logging.MethodCall(GetType()))
                {
                    var result = Mapper.Map<Location>(dbLocation);
                    result.location = new GeoJSONPoint(GeographyPoint.Create(dbLocation.Latitude, dbLocation.Longitude, dbLocation.Elevation));
                    if (thing == null)
                    {
                        var dbThing = DbContext.SensorThingsThings.First(i => i.Id == dbLocation.Id);
                        thing = ConvertThing(dbThing);
                    }
                    result.Things.Add(thing);
                    result.HistoricalLocations.Add(ConvertHistoricalLocation(dbLocation, result, thing));
                    //Logging.Verbose("Result: {@Result}", result);
                    return result;
                }
            }

            ObservedProperty ConvertObservedProperty(db.SensorThingsObservedProperty dbObservedProperty)
            {
                using (Logging.MethodCall(GetType()))
                {
                    var result = Mapper.Map<ObservedProperty>(dbObservedProperty);
                    result.Code = $"{dbObservedProperty.PhenomenonCode} {dbObservedProperty.OfferingCode}";
                    result.Name = $"{dbObservedProperty.PhenomenonName} {dbObservedProperty.OfferingName}";
                    result.Definition = dbObservedProperty.PhenomenonUrl;
                    result.Description = $"{dbObservedProperty.PhenomenonName}, {dbObservedProperty.OfferingName}";
                    //Logging.Verbose("Result: {@Result}", result);
                    return result;
                }
            }

            Sensor ConvertSensor(db.SensorThingsSensor dbSensor)
            {
                using (Logging.MethodCall(GetType()))
                {
                    var result = Mapper.Map<Sensor>(dbSensor);
                    result.Metdadata = dbSensor.Url;
                    //Logging.Verbose("Result: {@Result}", result);
                    return result;
                }
            }

            Thing ConvertThing(db.SensorThingsThing dbThing)
            {
                using (Logging.MethodCall(GetType()))
                {
                    var result = Mapper.Map<Thing>(dbThing);
                    result.Properties.Add("kind", dbThing.Kind);
                    if (!string.IsNullOrWhiteSpace(dbThing.Url)) result.Properties.Add("url", dbThing.Url);
                    if (dbThing.StartDate.HasValue) result.Properties.Add("startDate", dbThing.StartDate.Value.ToString("o"));
                    if (dbThing.EndDate.HasValue) result.Properties.Add("endDate", dbThing.EndDate.Value.ToString("o"));
                    //Logging.Verbose("Result: {@Result}", result);
                    return result;
                }
            }

        }

        private T ConvertDbEntity<T>(TDbEntity dbEntity) where T : SensorThingsEntity
        {
            return ConvertDbEntity<T, TDbEntity>(dbEntity);
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
                    foreach (var dbEntity in DbContext.Set<TDbEntity>().AsNoTracking())
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
                    var dbEntity = DbContext.Set<TDbEntity>().AsNoTracking().FirstOrDefault(i => i.Id == id);
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

        private TDbRelatedEntity LoadRelatedSingle<TDbRelatedEntity>(Guid id) where TDbRelatedEntity : db.BaseIDEntity
        {
            using (Logging.MethodCall<TDbEntity, TDbRelatedEntity>(GetType()))
            {
                try
                {
                    object result = default(TDbRelatedEntity);
                    var dbEntity = DbContext.Set<TDbEntity>().AsNoTracking().First(i => i.Id == id);
                    switch (dbEntity)
                    {
                        // Datastream
                        case db.SensorThingsDatastream datastream when typeof(TDbRelatedEntity) == typeof(db.SensorThingsThing):
                            result = dbContext.SensorThingsThings.AsNoTracking().FirstOrDefault(i => i.Id == datastream.InstrumentId);
                            break;
                        case db.SensorThingsDatastream datastream when typeof(TDbRelatedEntity) == typeof(db.SensorThingsSensor):
                            result = dbContext.SensorThingsSensors.AsNoTracking().FirstOrDefault(i => i.Id == datastream.Id);
                            break;
                        case db.SensorThingsDatastream datastream when typeof(TDbRelatedEntity) == typeof(db.SensorThingsObservedProperty):
                            var dbDatastreamSensor = dbContext.SensorThingsSensors.AsNoTracking().First(i => i.Id == datastream.Id);
                            result = dbContext.SensorThingsObservedProperies.AsNoTracking().FirstOrDefault(i => i.Id == dbDatastreamSensor.PhenomenonOfferingId);
                            break;
                        // Location
                        case db.SensorThingsLocation location when typeof(TDbRelatedEntity) == typeof(db.SensorThingsThing):
                            result = dbContext.SensorThingsThings.AsNoTracking().FirstOrDefault(i => i.Id == location.Id);
                            break;
                        // ObservedProperty
                        case db.SensorThingsObservedProperty observedProperty when typeof(TDbRelatedEntity) == typeof(db.SensorThingsDatastream):
                            var dbObservedPropertySensor = dbContext.SensorThingsSensors.AsNoTracking().First(i => i.PhenomenonOfferingId == observedProperty.Id);
                            result = dbContext.SensorThingsDatastreams.AsNoTracking().FirstOrDefault(i => i.Id == dbObservedPropertySensor.PhenomenonOfferingId);
                            break;
                        // Sensor
                        case db.SensorThingsSensor sensor when typeof(TDbRelatedEntity) == typeof(db.SensorThingsDatastream):
                            result = dbContext.SensorThingsDatastreams.AsNoTracking().FirstOrDefault(i => i.Id == sensor.Id);
                            break;
                        // Thing
                        case db.SensorThingsThing thing when typeof(TDbRelatedEntity) == typeof(db.SensorThingsLocation):
                            result = dbContext.SensorThingsLocations.AsNoTracking().FirstOrDefault(i => i.Id == thing.Id);
                            break;
                        case db.SensorThingsThing thing when typeof(TDbRelatedEntity) == typeof(db.SensorThingsDatastream):
                            result = dbContext.SensorThingsDatastreams.AsNoTracking().FirstOrDefault(i => i.InstrumentId == thing.Id);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    return (TDbRelatedEntity)result;
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        private IQueryable<TDbRelatedEntity> LoadRelatedMany<TDbRelatedEntity>(Guid id) where TDbRelatedEntity : db.BaseIDEntity
        {
            using (Logging.MethodCall<TDbEntity, TDbRelatedEntity>(GetType()))
            {
                try
                {
                    var dbEntity = DbContext.Set<TDbEntity>().AsNoTracking().First(i => i.Id == id);
                    switch (dbEntity)
                    {
                        // Datastream
                        case db.SensorThingsDatastream datastream when typeof(TDbRelatedEntity) == typeof(db.SensorThingsThing):
                            return (IQueryable<TDbRelatedEntity>)dbContext.SensorThingsThings.AsNoTracking().Where(i => i.Id == datastream.InstrumentId);
                        case db.SensorThingsDatastream datastream when typeof(TDbRelatedEntity) == typeof(db.SensorThingsSensor):
                            return (IQueryable<TDbRelatedEntity>)dbContext.SensorThingsSensors.AsNoTracking().Where(i => i.Id == datastream.Id);
                        case db.SensorThingsDatastream datastream when typeof(TDbRelatedEntity) == typeof(db.SensorThingsObservedProperty):
                            var dbDatastreamSensor = dbContext.SensorThingsSensors.AsNoTracking().First(i => i.Id == datastream.Id);
                            return (IQueryable<TDbRelatedEntity>)dbContext.SensorThingsObservedProperies.AsNoTracking().Where(i => i.Id == dbDatastreamSensor.PhenomenonOfferingId);
                        // Location
                        case db.SensorThingsLocation location when typeof(TDbRelatedEntity) == typeof(db.SensorThingsThing):
                            return (IQueryable<TDbRelatedEntity>)dbContext.SensorThingsThings.AsNoTracking().Where(i => i.Id == location.Id);
                        // ObservedProperty
                        case db.SensorThingsObservedProperty observedProperty when typeof(TDbRelatedEntity) == typeof(db.SensorThingsDatastream):
                            var dbObservedPropertySensor = dbContext.SensorThingsSensors.AsNoTracking().First(i => i.PhenomenonOfferingId == observedProperty.Id);
                            return (IQueryable<TDbRelatedEntity>)dbContext.SensorThingsDatastreams.AsNoTracking().Where(i => i.Id == dbObservedPropertySensor.PhenomenonOfferingId);
                        // Sensor
                        case db.SensorThingsSensor sensor when typeof(TDbRelatedEntity) == typeof(db.SensorThingsDatastream):
                            return (IQueryable<TDbRelatedEntity>)dbContext.SensorThingsDatastreams.AsNoTracking().Where(i => i.Id == sensor.Id);
                        // Thing
                        case db.SensorThingsThing thing when typeof(TDbRelatedEntity) == typeof(db.SensorThingsLocation):
                            return (IQueryable<TDbRelatedEntity>)dbContext.SensorThingsLocations.AsNoTracking().Where(i => i.Id == thing.Id);
                        case db.SensorThingsThing thing when typeof(TDbRelatedEntity) == typeof(db.SensorThingsDatastream):
                            return (IQueryable<TDbRelatedEntity>)dbContext.SensorThingsDatastreams.AsNoTracking().Where(i => i.InstrumentId == thing.Id);
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

        protected SingleResult<TRelatedEntity> GetRelatedSingle<TRelatedEntity, TDbRelatedEntity>(Guid id) where TRelatedEntity : SensorThingsEntity where TDbRelatedEntity : db.BaseIDEntity
        {
            using (Logging.MethodCall<TRelatedEntity, TDbRelatedEntity>(GetType()))
            {
                try
                {
                    UpdateRequest(true);
                    Logging.Verbose("uri: {uri}", Request.RequestUri.ToString());
                    var result = new List<TRelatedEntity>();
                    var dbRelatedEntity = LoadRelatedSingle<TDbRelatedEntity>(id);
                    if (dbRelatedEntity != null)
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

        protected IQueryable<TRelatedEntity> GetRelatedMany<TRelatedEntity, TDbRelatedEntity>(Guid id) where TDbRelatedEntity : db.BaseIDEntity where TRelatedEntity : SensorThingsEntity
        {
            using (Logging.MethodCall<TRelatedEntity, TDbRelatedEntity>(GetType()))
            {
                try
                {
                    UpdateRequest(true);
                    Logging.Verbose("uri: {uri}", Request.RequestUri.ToString());
                    var result = new List<TRelatedEntity>();
                    foreach (var dbRelatedEntity in LoadRelatedMany<TDbRelatedEntity>(id))
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
