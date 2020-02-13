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
                if (dbContext == null)
                {
                    dbContext = new db.ObservationsDbContext(TenantAuthorizationAttribute.GetTenantFromHeaders(Request));
                    //dbContext.Configuration.AutoDetectChangesEnabled = false;
                }
                return dbContext;
            }
        }
        protected IMapper Mapper { get; private set; } = null;

        protected SensorThingsController() : base()
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

            HistoricalLocation ConvertHistoricalLocation(db.SensorThingsHistoricalLocation dbHistoricalLocation/*, Location location = null, Thing thing = null*/)
            {
                using (Logging.MethodCall(GetType()))
                {
                    var result = Mapper.Map<HistoricalLocation>(dbHistoricalLocation);
                    result.TimeString = new TimeString(dbHistoricalLocation.StartDate ?? dbHistoricalLocation.EndDate);
                    //if (location == null)
                    //{
                    //    var dbLocation = DbContext.SensorThingsLocations.AsNoTracking().First(i => i.Id == dbHistoricalLocation.Id);
                    //    location = ConvertLocation(dbLocation);
                    //}
                    //if (thing == null)
                    //{
                    //    var dbThing = DbContext.SensorThingsThings.AsNoTracking().First(i => i.Id == dbHistoricalLocation.Id);
                    //    thing = ConvertThing(dbThing);
                    //}
                    //result.Thing = thing;
                    //result.Locations.Add(location);
                    //Logging.Verbose("Result: {@Result}", result);
                    return result;
                }
            }

            Location ConvertLocation(db.SensorThingsLocation dbLocation/*, Thing thing = null*/)
            {
                using (Logging.MethodCall(GetType()))
                {
                    var result = Mapper.Map<Location>(dbLocation);
                    result.location = new GeoJSONPoint(GeographyPoint.Create(dbLocation.Latitude, dbLocation.Longitude, dbLocation.Elevation));
                    //if (thing == null)
                    //{
                    //    var dbThing = DbContext.SensorThingsThings.AsNoTracking().First(i => i.Id == dbLocation.Id);
                    //    thing = ConvertThing(dbThing);
                    //}
                    //result.Things.Add(thing);
                    //var dbHistoricalLocation = DbContext.SensorThingsHistoricalLocations.AsNoTracking().First(i => i.Id == dbLocation.Id);
                    //result.HistoricalLocations.Add(ConvertHistoricalLocation(dbHistoricalLocation, result, thing));
                    //Logging.Verbose("Result: {@Result}", result);
                    return result;
                }
            }

            Observation ConvertObservation(db.SensorThingsObservation dbObservation/*, Datastream datastream = null, FeatureOfInterest featureOfInterest = null*/)
            {
                using (Logging.MethodCall(GetType()))
                {
                    var result = Mapper.Map<Observation>(dbObservation);
                    result.PhenomenonTimeString = new TimeString(dbObservation.Date);
                    result.ResultTimeString = new TimeString(dbObservation.Date);
                    result.Result = dbObservation.Value;
                    //if (datastream == null)
                    //{
                    //    var dbDatastream = DbContext.SensorThingsDatastreams.AsNoTracking().First(i => i.Id == dbObservation.SensorId);
                    //    datastream = ConvertDatastream(dbDatastream);
                    //}
                    //result.Datastream = datastream;
                    //if (featureOfInterest == null)
                    //{

                    //}
                    //result.FeatureOfInterest = featureOfInterest;
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
                    foreach (var dbEntity in DbContext.Set<TDbEntity>())
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
                            return (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsThings.AsNoTracking().Where(i => i.Id == datastream.InstrumentId);
                        case db.SensorThingsDatastream datastream when typeof(TDbRelatedEntity) == typeof(db.SensorThingsSensor):
                            return (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsSensors.AsNoTracking().Where(i => i.Id == datastream.Id);
                        case db.SensorThingsDatastream datastream when typeof(TDbRelatedEntity) == typeof(db.SensorThingsObservedProperty):
                            var dbSensor = DbContext.SensorThingsSensors.AsNoTracking().First(i => i.Id == datastream.Id);
                            return (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsObservedProperies.AsNoTracking().Where(i => i.Id == dbSensor.PhenomenonOfferingId);
                        case db.SensorThingsDatastream datastream when typeof(TDbRelatedEntity) == typeof(db.SensorThingsObservation):
                            dbSensor = DbContext.SensorThingsSensors.AsNoTracking().First(i => i.Id == datastream.Id);
                            return (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsObservations.AsNoTracking().Where(i =>
                                (i.SensorId == dbSensor.Id) && (i.PhenomenonOfferingID == dbSensor.PhenomenonOfferingId) && (i.PhenomenonUnitId == datastream.PhenomenonUnitId));
                        // FeatureOfInterest
                        //case db.SensorThingsFeatureOfInterest featureOfInterest when typeof(TDbRelatedEntity) == typeof(db.SensorThingsObservation):
                        //    return (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsObservations.AsNoTracking().Where(i => i.Id == new Guid());
                        // HistoricalLocation
                        case db.SensorThingsHistoricalLocation historicalLocation when typeof(TDbRelatedEntity) == typeof(db.SensorThingsLocation):
                            return (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsLocations.AsNoTracking().Where(i => i.Id == historicalLocation.Id);
                        case db.SensorThingsHistoricalLocation historicalLocation when typeof(TDbRelatedEntity) == typeof(db.SensorThingsThing):
                            return (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsThings.AsNoTracking().Where(i => i.Id == historicalLocation.Id);
                        // Location
                        case db.SensorThingsLocation location when typeof(TDbRelatedEntity) == typeof(db.SensorThingsHistoricalLocation):
                            return (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsHistoricalLocations.AsNoTracking().Where(i => i.Id == location.Id);
                        case db.SensorThingsLocation location when typeof(TDbRelatedEntity) == typeof(db.SensorThingsThing):
                            return (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsThings.AsNoTracking().Where(i => i.Id == location.Id);
                        // Observation
                        case db.SensorThingsObservation observation when typeof(TDbRelatedEntity) == typeof(db.SensorThingsDatastream):
                            return (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsDatastreams.AsNoTracking().Where(i =>
                                (i.Id == observation.SensorId) && (i.PhenomenonOfferingId == observation.PhenomenonOfferingID) && (i.PhenomenonUnitId == observation.PhenomenonUnitId));
                        //case db.SensorThingsObservation observation when typeof(TDbRelatedEntity) == typeof(db.SensorThingsFeatureOfInterest):
                        //    return (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsFeateuresOfInterest.AsNoTracking().Where(i => i.Id == observation.SensorId);
                        // ObservedProperty
                        case db.SensorThingsObservedProperty observedProperty when typeof(TDbRelatedEntity) == typeof(db.SensorThingsDatastream):
                            dbSensor = DbContext.SensorThingsSensors.AsNoTracking().First(i => i.PhenomenonOfferingId == observedProperty.Id);
                            return (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsDatastreams.AsNoTracking().Where(i => i.Id == dbSensor.PhenomenonOfferingId);
                        // Sensor
                        case db.SensorThingsSensor sensor when typeof(TDbRelatedEntity) == typeof(db.SensorThingsDatastream):
                            return (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsDatastreams.AsNoTracking().Where(i => i.Id == sensor.Id);
                        // Thing
                        case db.SensorThingsThing thing when typeof(TDbRelatedEntity) == typeof(db.SensorThingsDatastream):
                            return (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsDatastreams.AsNoTracking().Where(i => i.InstrumentId == thing.Id);
                        case db.SensorThingsThing thing when typeof(TDbRelatedEntity) == typeof(db.SensorThingsHistoricalLocation):
                            return (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsHistoricalLocations.AsNoTracking().Where(i => i.Id == thing.Id);
                        case db.SensorThingsThing thing when typeof(TDbRelatedEntity) == typeof(db.SensorThingsLocation):
                            return (IQueryable<TDbRelatedEntity>)DbContext.SensorThingsLocations.AsNoTracking().Where(i => i.Id == thing.Id);
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
                    UpdateRequest(false);
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
