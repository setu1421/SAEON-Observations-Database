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
    public abstract class SensorThingsGuidIdController<TEntity, TDbEntity> : ODataController where TEntity : SensorThingsGuidIdEntity, new() where TDbEntity : db.GuidIdEntity
    {
        protected static string BaseUrl { get; set; }
        public const int PageSize = 25;
        public const int MaxTop = 500;
        public const int MaxAll = 10000;
        private db.ObservationsDbContext dbContext = null;
        protected db.ObservationsDbContext DbContext => dbContext ?? (dbContext = new db.ObservationsDbContext(TenantAuthorizationAttribute.GetTenantFromHeaders(Request)));
        protected IMapper Mapper { get; private set; } = null;

        protected SensorThingsGuidIdController() : base()
        {
            var config = new MapperConfigurationExpression();
            config.CreateMap<db.SensorThingsDatastream, Datastream>();
            config.CreateMap<db.SensorThingsFeatureOfInterest, FeatureOfInterest>();
            config.CreateMap<db.SensorThingsHistoricalLocation, HistoricalLocation>();
            config.CreateMap<db.SensorThingsLocation, Location>();
            config.CreateMap<db.SensorThingsObservation, Observation>();
            config.CreateMap<db.SensorThingsObservedProperty, ObservedProperty>();
            config.CreateMap<db.SensorThingsSensor, Sensor>();
            config.CreateMap<db.SensorThingsThing, Thing>();
            Mapper = new MapperConfiguration(config).CreateMapper();
        }

        private T ConvertDbEntityGuidId<T, TDb>(TDb dbEntity) where T : SensorThingsGuidIdEntity where TDb : db.GuidIdEntity
        {
            using (Logging.MethodCall<T, TDb>(GetType()))
            {
                object result = default(T);
                switch (dbEntity)
                {
                    case db.SensorThingsDatastream dbDatastream:
                        result = ConvertDatastream(dbDatastream);
                        break;
                    case db.SensorThingsFeatureOfInterest dbFeatureOfInterest:
                        result = ConvertFeatureOfInterest(dbFeatureOfInterest);
                        break;
                    case db.SensorThingsHistoricalLocation dbHistoricalLocation:
                        result = ConvertHistoricalLocation(dbHistoricalLocation);
                        break;
                    case db.SensorThingsLocation dbLocation:
                        result = ConvertLocation(dbLocation);
                        break;
                    case db.SensorThingsObservation dbObservation:
                        result = ConvertObservation(dbObservation);
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
                        result.PhenomenonTimeInterval = new TimeInterval(dbDatastream.StartDate.Value, dbDatastream.EndDate.Value);
                        result.ResultTimeInterval = new TimeInterval(dbDatastream.StartDate.Value, dbDatastream.EndDate.Value);
                    }
                    //Logging.Verbose("Result: {@Result}", result);
                    return result;
                }
            }

            FeatureOfInterest ConvertFeatureOfInterest(db.SensorThingsFeatureOfInterest dbFeatureOfInterest)
            {
                using (Logging.MethodCall(GetType()))
                {
                    var result = Mapper.Map<FeatureOfInterest>(dbFeatureOfInterest);
                    var dbLocation = DbContext.SensorThingsLocations.AsNoTracking().First(i => i.Id == dbFeatureOfInterest.Id);
                    result.Feature = new GeoJSONPoint(GeographyPoint.Create(dbLocation.Latitude, dbLocation.Longitude, dbLocation.Elevation));
                    //Logging.Verbose("Result: {@Result}", result);
                    return result;
                }
            }

            HistoricalLocation ConvertHistoricalLocation(db.SensorThingsHistoricalLocation dbHistoricalLocation)
            {
                using (Logging.MethodCall(GetType()))
                {
                    var result = Mapper.Map<HistoricalLocation>(dbHistoricalLocation);
                    result.TimeString = new TimeString(dbHistoricalLocation.StartDate ?? dbHistoricalLocation.EndDate);
                    //Logging.Verbose("Result: {@Result}", result);
                    return result;
                }
            }

            Location ConvertLocation(db.SensorThingsLocation dbLocation)
            {
                using (Logging.MethodCall(GetType()))
                {
                    var result = Mapper.Map<Location>(dbLocation);
                    result.location = new GeoJSONPoint(GeographyPoint.Create(dbLocation.Latitude, dbLocation.Longitude, dbLocation.Elevation));
                    return result;
                }
            }

            Observation ConvertObservation(db.SensorThingsObservation dbObservation)
            {
                using (Logging.MethodCall(GetType()))
                {
                    var result = Mapper.Map<Observation>(dbObservation);
                    result.PhenomenonTimeString = new TimeString(dbObservation.Date);
                    result.ResultTimeString = new TimeString(dbObservation.Date);
                    result.Result = dbObservation.Value;
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

        private T ConvertDbEntityGuidId<T>(TDbEntity dbEntity) where T : SensorThingsGuidIdEntity
        {
            return ConvertDbEntityGuidId<T, TDbEntity>(dbEntity);
        }

        private T ConvertDbEntityIntId<T, TDb>(TDb dbEntity) where T : SensorThingsIntIdEntity where TDb : db.IntIdEntity
        {
            using (Logging.MethodCall<T, TDb>(GetType()))
            {
                object result = default(T);
                switch (dbEntity)
                {
                    case db.SensorThingsObservation dbObservation:
                        result = ConvertObservation(dbObservation);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                return (T)result;
            }

            Observation ConvertObservation(db.SensorThingsObservation dbObservation)
            {
                using (Logging.MethodCall(GetType()))
                {
                    var result = Mapper.Map<Observation>(dbObservation);
                    result.PhenomenonTimeString = new TimeString(dbObservation.Date);
                    result.ResultTimeString = new TimeString(dbObservation.Date);
                    result.Result = dbObservation.Value;
                    //Logging.Verbose("Result: {@Result}", result);
                    return result;
                }
            }
        }

        private void UpdateRequest(bool isMany)
        {
            BaseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority) + "/SensorThings";
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
            Request.Headers.TryAddWithoutValidation("Prefer", "odata.metadata=full");
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
            using (Logging.MethodCall<TDbEntity>(GetType()))
            {
                try
                {
                    UpdateRequest(true);
                    Logging.Verbose("uri: {uri}", Request.RequestUri.ToString());
                    var result = new List<TEntity>();
                    foreach (var dbEntity in DbContext.Set<TDbEntity>().AsNoTracking().Take(MaxAll))
                    {
                        result.Add(ConvertDbEntityGuidId<TEntity>(dbEntity));
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

        //[ODataRoute("({id})")] required on derived class
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
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
                        result.Add(ConvertDbEntityGuidId<TEntity>(dbEntity));
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

        private TDbRelatedEntity LoadRelatedSingle<TDbRelatedEntity>(Guid id) where TDbRelatedEntity : db.GuidIdEntity
        {
            using (Logging.MethodCall<TDbEntity, TDbRelatedEntity>(GetType()))
            {
                try
                {
                    var result = LoadRelatedMany<TDbRelatedEntity>(id).FirstOrDefault();
                    return (TDbRelatedEntity)result;
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        private IQueryable<TDbRelatedEntity> LoadRelatedMany<TDbRelatedEntity>(Guid id) where TDbRelatedEntity : db.GuidIdEntity
        {
            using (Logging.MethodCall<TDbEntity, TDbRelatedEntity>(GetType()))
            {
                try
                {
                    var dbEntity = DbContext.Set<TDbEntity>().AsNoTracking().First(i => i.Id == id);
                    var result = default(IQueryable<TDbRelatedEntity>);
                    switch (dbEntity)
                    {
                        // Datastream
                        case db.SensorThingsDatastream datastream when typeof(TDbRelatedEntity) == typeof(db.SensorThingsThing):
                            result = (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsThings.AsNoTracking().Where(i => i.Id == datastream.InstrumentId);
                            break;
                        case db.SensorThingsDatastream datastream when typeof(TDbRelatedEntity) == typeof(db.SensorThingsSensor):
                            result = (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsSensors.AsNoTracking().Where(i => i.Id == datastream.Id);
                            break;
                        case db.SensorThingsDatastream datastream when typeof(TDbRelatedEntity) == typeof(db.SensorThingsObservedProperty):
                            var dbSensor = DbContext.SensorThingsSensors.AsNoTracking().First(i => i.Id == datastream.Id);
                            result = (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsObservedProperies.AsNoTracking().Where(i => i.Id == dbSensor.PhenomenonOfferingId);
                            break;
                        case db.SensorThingsDatastream datastream when typeof(TDbRelatedEntity) == typeof(db.SensorThingsObservation):
                            result = (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsObservations.AsNoTracking().Where(i =>
                                (i.SensorId == datastream.Id) && (i.PhenomenonOfferingID == datastream.PhenomenonOfferingId) && (i.PhenomenonUnitId == datastream.PhenomenonUnitId));
                            break;
                        // FeatureOfInterest
                        //case db.SensorThingsFeatureOfInterest featureOfInterest when typeof(TDbRelatedEntity) == typeof(db.SensorThingsObservation):
                        //    var dbDatastream = DbContext.SensorThingsDatastreams.AsNoTracking().First(i => i.InstrumentId == featureOfInterest.Id);
                        //    result = (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsObservations.AsNoTracking().Where(i =>
                        //        (i.SensorId == dbDatastream.Id) && (i.PhenomenonOfferingID == dbDatastream.PhenomenonOfferingId) && (i.PhenomenonUnitId == dbDatastream.PhenomenonUnitId));
                        //    break;
                        // HistoricalLocation
                        case db.SensorThingsHistoricalLocation historicalLocation when typeof(TDbRelatedEntity) == typeof(db.SensorThingsLocation):
                            result = (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsLocations.AsNoTracking().Where(i => i.Id == historicalLocation.Id);
                            break;
                        case db.SensorThingsHistoricalLocation historicalLocation when typeof(TDbRelatedEntity) == typeof(db.SensorThingsThing):
                            result = (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsThings.AsNoTracking().Where(i => i.Id == historicalLocation.Id);
                            break;
                        // Location
                        case db.SensorThingsLocation location when typeof(TDbRelatedEntity) == typeof(db.SensorThingsHistoricalLocation):
                            result = (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsHistoricalLocations.AsNoTracking().Where(i => i.Id == location.Id);
                            break;
                        case db.SensorThingsLocation location when typeof(TDbRelatedEntity) == typeof(db.SensorThingsThing):
                            result = (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsThings.AsNoTracking().Where(i => i.Id == location.Id);
                            break;
                        // Observation
                        case db.SensorThingsObservation observation when typeof(TDbRelatedEntity) == typeof(db.SensorThingsDatastream):
                            result = (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsDatastreams.AsNoTracking().Where(i =>
                                (i.Id == observation.SensorId) && (i.PhenomenonOfferingId == observation.PhenomenonOfferingID) && (i.PhenomenonUnitId == observation.PhenomenonUnitId));
                            break;
                        // ObservedProperty
                        case db.SensorThingsObservedProperty observedProperty when typeof(TDbRelatedEntity) == typeof(db.SensorThingsDatastream):
                            dbSensor = DbContext.SensorThingsSensors.AsNoTracking().First(i => i.PhenomenonOfferingId == observedProperty.Id);
                            result = (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsDatastreams.AsNoTracking().Where(i =>
                                (i.Id == dbSensor.Id) && (i.PhenomenonOfferingId == dbSensor.PhenomenonOfferingId));
                            break;
                        // Sensor
                        case db.SensorThingsSensor sensor when typeof(TDbRelatedEntity) == typeof(db.SensorThingsDatastream):
                            result = (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsDatastreams.AsNoTracking().Where(i => i.Id == sensor.Id);
                            break;
                        // Thing
                        case db.SensorThingsThing thing when typeof(TDbRelatedEntity) == typeof(db.SensorThingsDatastream):
                            result = (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsDatastreams.AsNoTracking().Where(i => i.InstrumentId == thing.Id);
                            break;
                        case db.SensorThingsThing thing when typeof(TDbRelatedEntity) == typeof(db.SensorThingsHistoricalLocation):
                            result = (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsHistoricalLocations.AsNoTracking().Where(i => i.Id == thing.Id);
                            break;
                        case db.SensorThingsThing thing when typeof(TDbRelatedEntity) == typeof(db.SensorThingsLocation):
                            result = (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsLocations.AsNoTracking().Where(i => i.Id == thing.Id);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    return result.Take(MaxAll);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        private IQueryable<TDbRelatedEntity> LoadRelatedManyIntId<TDbRelatedEntity>(Guid id) where TDbRelatedEntity : db.IntIdEntity
        {
            using (Logging.MethodCall<TDbEntity, TDbRelatedEntity>(GetType()))
            {
                try
                {
                    var dbEntity = DbContext.Set<TDbEntity>().AsNoTracking().First(i => i.Id == id);
                    var result = default(IQueryable<TDbRelatedEntity>);
                    switch (dbEntity)
                    {
                        // Datastream
                        case db.SensorThingsDatastream datastream when typeof(TDbRelatedEntity) == typeof(db.SensorThingsObservation):
                            var dbSensor = DbContext.SensorThingsSensors.AsNoTracking().First(i => i.Id == datastream.Id);
                            result = (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsObservations.AsNoTracking().Where(i =>
                                (i.SensorId == dbSensor.Id) && (i.PhenomenonOfferingID == dbSensor.PhenomenonOfferingId) && (i.PhenomenonUnitId == datastream.PhenomenonUnitId));
                            break;
                        // FeatureOfInterest
                        case db.SensorThingsFeatureOfInterest featureOfInterest when typeof(TDbRelatedEntity) == typeof(db.SensorThingsObservation):
                            var dbDatastream = DbContext.SensorThingsDatastreams.AsNoTracking().First(i => i.InstrumentId == featureOfInterest.Id);
                            result = (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsObservations.AsNoTracking().Where(i =>
                                (i.SensorId == dbDatastream.Id) && (i.PhenomenonOfferingID == dbDatastream.PhenomenonOfferingId) && (i.PhenomenonUnitId == dbDatastream.PhenomenonUnitId));
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    return result.Take(MaxAll);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        //[ODataRoute("({id}/TRelatedEntity)]")] required on calling class
        //[EnableQuery(PageSize = PageSize, MaxTop = MaxTop) required on calling class

        protected SingleResult<TRelatedEntity> GetRelatedSingle<TRelatedEntity, TDbRelatedEntity>(Guid id) where TRelatedEntity : SensorThingsGuidIdEntity where TDbRelatedEntity : db.GuidIdEntity
        {
            using (Logging.MethodCall<TRelatedEntity, TDbRelatedEntity>(GetType()))
            {
                try
                {
                    UpdateRequest(false);
                    Logging.Verbose("uri: {uri}", Request.RequestUri.ToString());
                    var result = new List<TRelatedEntity>();
                    var dbRelatedEntity = LoadRelatedSingle<TDbRelatedEntity>(id);
                    if (dbRelatedEntity != null)
                    {
                        result.Add(ConvertDbEntityGuidId<TRelatedEntity, TDbRelatedEntity>(dbRelatedEntity));
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

        //[ODataRoute("({id}/TRelatedEntities)]")] required on calling class
        //[EnableQuery(PageSize = PageSize, MaxTop = MaxTop) required on calling class
        protected IQueryable<TRelatedEntity> GetRelatedMany<TRelatedEntity, TDbRelatedEntity>(Guid id) where TDbRelatedEntity : db.GuidIdEntity where TRelatedEntity : SensorThingsGuidIdEntity
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
                        result.Add(ConvertDbEntityGuidId<TRelatedEntity, TDbRelatedEntity>(dbRelatedEntity));
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

        //[ODataRoute("({id}/TRelatedEntities)]")] required on calling class
        //[EnableQuery(PageSize = PageSize, MaxTop = MaxTop) required on calling class
        protected IQueryable<TRelatedEntity> GetRelatedManyIntId<TRelatedEntity, TDbRelatedEntity>(Guid id) where TDbRelatedEntity : db.IntIdEntity where TRelatedEntity : SensorThingsIntIdEntity
        {
            using (Logging.MethodCall<TRelatedEntity, TDbRelatedEntity>(GetType()))
            {
                try
                {
                    UpdateRequest(true);
                    Logging.Verbose("uri: {uri}", Request.RequestUri.ToString());
                    var result = new List<TRelatedEntity>();
                    foreach (var dbRelatedEntity in LoadRelatedManyIntId<TDbRelatedEntity>(id))
                    {
                        result.Add(ConvertDbEntityIntId<TRelatedEntity, TDbRelatedEntity>(dbRelatedEntity));
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

    public abstract class SensorThingsIntIdController<TEntity, TDbEntity> : ODataController where TEntity : SensorThingsIntIdEntity, new() where TDbEntity : db.IntIdEntity
    {
        protected static string BaseUrl { get; set; }
        public const int PageSize = 25;
        public const int MaxTop = 500;
        public const int MaxAll = 10000;
        private db.ObservationsDbContext dbContext = null;
        protected db.ObservationsDbContext DbContext => (dbContext != null) ? dbContext : new db.ObservationsDbContext(TenantAuthorizationAttribute.GetTenantFromHeaders(Request));
        protected IMapper Mapper { get; private set; } = null;

        protected SensorThingsIntIdController() : base()
        {
            var config = new MapperConfigurationExpression();
            config.CreateMap<db.SensorThingsDatastream, Datastream>();
            config.CreateMap<db.SensorThingsFeatureOfInterest, FeatureOfInterest>();
            config.CreateMap<db.SensorThingsObservation, Observation>();
            Mapper = new MapperConfiguration(config).CreateMapper();
        }

        private T ConvertDbEntityIntId<T, TDb>(TDb dbEntity) where T : SensorThingsIntIdEntity where TDb : db.IntIdEntity
        {
            using (Logging.MethodCall<T, TDb>(GetType()))
            {
                object result = default(T);
                switch (dbEntity)
                {
                    case db.SensorThingsObservation dbObservation:
                        result = ConvertObservation(dbObservation);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                return (T)result;
            }

            Observation ConvertObservation(db.SensorThingsObservation dbObservation)
            {
                using (Logging.MethodCall(GetType()))
                {
                    var result = Mapper.Map<Observation>(dbObservation);
                    result.PhenomenonTimeString = new TimeString(dbObservation.Date);
                    result.ResultTimeString = new TimeString(dbObservation.Date);
                    result.Result = dbObservation.Value;
                    //Logging.Verbose("Result: {@Result}", result);
                    return result;

                }
            }
        }

        private T ConvertDbEntityIntId<T>(TDbEntity dbEntity) where T : SensorThingsIntIdEntity
        {
            return ConvertDbEntityIntId<T, TDbEntity>(dbEntity);
        }

        private T ConvertDbEntityGuidId<T, TDb>(TDb dbEntity) where T : SensorThingsGuidIdEntity where TDb : db.GuidIdEntity
        {
            using (Logging.MethodCall<T, TDb>(GetType()))
            {
                object result = default(T);
                switch (dbEntity)
                {
                    case db.SensorThingsDatastream dbDatastream:
                        result = ConvertDatastream(dbDatastream);
                        break;
                    case db.SensorThingsFeatureOfInterest dbFeatureOfInterest:
                        result = ConvertFeatureOfInterest(dbFeatureOfInterest);
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
                        result.PhenomenonTimeInterval = new TimeInterval(dbDatastream.StartDate.Value, dbDatastream.EndDate.Value);
                        result.ResultTimeInterval = new TimeInterval(dbDatastream.StartDate.Value, dbDatastream.EndDate.Value);
                    }
                    //Logging.Verbose("Result: {@Result}", result);
                    return result;
                }
            }

            FeatureOfInterest ConvertFeatureOfInterest(db.SensorThingsFeatureOfInterest dbFeatureOfInterest)
            {
                using (Logging.MethodCall(GetType()))
                {
                    var result = Mapper.Map<FeatureOfInterest>(dbFeatureOfInterest);
                    var dbLocation = DbContext.SensorThingsLocations.AsNoTracking().First(i => i.Id == dbFeatureOfInterest.Id);
                    result.Feature = new GeoJSONPoint(GeographyPoint.Create(dbLocation.Latitude, dbLocation.Longitude, dbLocation.Elevation));
                    //Logging.Verbose("Result: {@Result}", result);
                    return result;
                }
            }
        }

        private void UpdateRequest(bool isMany)
        {
            BaseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority) + "/SensorThings";
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
        //[ODataRoute] required on derived class
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        public virtual IQueryable<TEntity> GetAll()
        {
            using (Logging.MethodCall<TDbEntity>(GetType()))
            {
                try
                {
                    UpdateRequest(true);
                    Logging.Verbose("uri: {uri}", Request.RequestUri.ToString());
                    var result = new List<TEntity>();
                    foreach (var dbEntity in DbContext.Set<TDbEntity>().AsNoTracking().Take(MaxAll))
                    {
                        result.Add(ConvertDbEntityIntId<TEntity>(dbEntity));
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

        //[ODataRoute("({id})")] required on derived class
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        public virtual SingleResult<TEntity> GetById([FromODataUri] int id)
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
                        result.Add(ConvertDbEntityIntId<TEntity>(dbEntity));
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

        private TDbRelatedEntity LoadRelatedSingle<TDbRelatedEntity>(int id) where TDbRelatedEntity : db.GuidIdEntity
        {
            using (Logging.MethodCall<TDbEntity, TDbRelatedEntity>(GetType()))
            {
                try
                {
                    var result = LoadRelatedMany<TDbRelatedEntity>(id).FirstOrDefault();
                    return (TDbRelatedEntity)result;
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        private IQueryable<TDbRelatedEntity> LoadRelatedMany<TDbRelatedEntity>(int id) where TDbRelatedEntity : db.GuidIdEntity
        {
            using (Logging.MethodCall<TDbEntity, TDbRelatedEntity>(GetType()))
            {
                try
                {
                    var dbEntity = DbContext.Set<TDbEntity>().AsNoTracking().First(i => i.Id == id);
                    var result = default(IQueryable<TDbRelatedEntity>);
                    switch (dbEntity)
                    {
                        // Observation
                        case db.SensorThingsObservation observation when typeof(TDbRelatedEntity) == typeof(db.SensorThingsDatastream):
                            result = (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsDatastreams.AsNoTracking().Where(i =>
                                (i.Id == observation.SensorId) && (i.PhenomenonOfferingId == observation.PhenomenonOfferingID) && (i.PhenomenonUnitId == observation.PhenomenonUnitId));
                            break;
                        case db.SensorThingsObservation observation when typeof(TDbRelatedEntity) == typeof(db.SensorThingsFeatureOfInterest):
                            var dbDatastream = DbContext.SensorThingsDatastreams.AsNoTracking().First(i =>
                                (i.Id == observation.SensorId) && (i.PhenomenonOfferingId == observation.PhenomenonOfferingID) && (i.PhenomenonUnitId == observation.PhenomenonUnitId));
                            result = (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsFeaturesOfInterest.AsNoTracking().Where(i => i.Id == dbDatastream.InstrumentId);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    return result.Take(MaxAll);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        //[ODataRoute("({id}/TRelatedEntity)]")] required on calling class
        //[EnableQuery(PageSize = PageSize, MaxTop = MaxTop) required on calling class

        protected SingleResult<TRelatedEntity> GetRelatedSingle<TRelatedEntity, TDbRelatedEntity>(int id) where TRelatedEntity : SensorThingsGuidIdEntity where TDbRelatedEntity : db.GuidIdEntity
        {
            using (Logging.MethodCall<TRelatedEntity, TDbRelatedEntity>(GetType()))
            {
                try
                {
                    UpdateRequest(false);
                    Logging.Verbose("uri: {uri}", Request.RequestUri.ToString());
                    var result = new List<TRelatedEntity>();
                    var dbRelatedEntity = LoadRelatedSingle<TDbRelatedEntity>(id);
                    if (dbRelatedEntity != null)
                    {
                        result.Add(ConvertDbEntityGuidId<TRelatedEntity, TDbRelatedEntity>(dbRelatedEntity));
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



    }
}
