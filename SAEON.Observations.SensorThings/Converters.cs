using AutoMapper;
using Microsoft.Spatial;
using SAEON.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using db = SAEON.Observations.Core.Entities;

namespace SAEON.Observations.SensorThings
{
    public class Converter
    {
        protected db.ObservationsDbContext DbContext { get; private set; } = null;
        protected IMapper Mapper { get; private set; } = null;

        //private List<Datastream> Datastreams => new List<Datastream>();
        //private List<HistoricalLocation> HistoricalLocations => new List<HistoricalLocation>();
        //private List<Location> Locations => new List<Location>();
        //private List<ObservedProperty> ObservedProperties => new List<ObservedProperty>();
        //private List<Sensor> Sensors => new List<Sensor>();
        //private List<Thing> Things => new List<Thing>();

        public Converter(db.ObservationsDbContext dbContext, IMapper mapper)
        {
            DbContext = dbContext;
            Mapper = mapper;
        }

        public Datastream ConvertDatastream(db.SensorThingsDatastream dbDatastream/*, Thing thing = null, Sensor sensor = null, ObservedProperty observedProperty = null*/)
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
                //if (thing == null)
                //{
                //    var dbThing = DbContext.SensorThingsThings.First(i => i.Id == dbDatastream.InstrumentId);
                //    thing = ConvertThing(dbThing, new List<Datastream> { result });
                //}
                //result.Thing = thing;
                //if (sensor == null)
                //{
                //    var dbSensor = DbContext.SensorThingsSensors.First(i => i.Id == dbDatastream.Id);
                //    sensor = ConvertSensor(dbSensor);
                //    if (observedProperty == null)
                //    {
                //        var dbObservedPropery = DbContext.SensorThingsObservedProperies.First(i => i.Id == dbSensor.PhenomenonOfferingId);
                //        observedProperty = ConvertObservedProperty(dbObservedPropery);
                //    }
                //}
                //result.Sensor = sensor;
                //result.ObservedProperty = observedProperty;
                //Logging.Verbose("Result: {@Result}", result);
                return result;
            }
        }

        public HistoricalLocation ConvertHistoricalLocation(db.SensorThingsLocation dbLocation, Thing thing = null)
        {
            using (Logging.MethodCall(GetType()))
            {
                var location = ConvertLocation(dbLocation);
                if (thing == null)
                {
                    var dbThing = DbContext.SensorThingsThings.First(i => i.Id == dbLocation.Id);
                    thing = ConvertThing(dbThing);
                }
                var result = ConvertHistoricalLocation(dbLocation.StartDate ?? dbLocation.EndDate ?? DateTime.Now, location, thing);
                //Logging.Verbose("Result: {@Result}", result);
                return result;
            }
        }

        private HistoricalLocation ConvertHistoricalLocation(DateTime time, Location location, Thing thing)
        {
            using (Logging.MethodCall(GetType()))
            {
                var result = new HistoricalLocation { Id = location.Id, Time = time, Thing = thing, Locations = new List<Location> { location } };
                //Logging.Verbose("Result: {@Result}", result);
                return result;
            }
        }

        public Location ConvertLocation(db.SensorThingsLocation dbLocation, Thing thing = null)
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
                result.HistoricalLocations.Add(ConvertHistoricalLocation(dbLocation.StartDate ?? dbLocation.EndDate ?? DateTime.Now, result, thing));
                //Logging.Verbose("Result: {@Result}", result);
                return result;
            }
        }

        public ObservedProperty ConvertObservedProperty(db.SensorThingsObservedProperty dbObservedProperty)
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

        public Sensor ConvertSensor(db.SensorThingsSensor dbSensor/*, List<Datastream> datastreams = null*/)
        {
            using (Logging.MethodCall(GetType()))
            {
                var result = Mapper.Map<Sensor>(dbSensor);
                result.Metdadata = dbSensor.Url;
                //if (datastreams == null)
                //{
                //    datastreams = new List<Datastream>();
                //    var dbDatastreams = DbContext.SensorThingsDatastreams.Where(i => i.Id == dbSensor.Id);
                //    foreach (var dbDatastream in dbDatastreams)
                //    {
                //        datastreams.Add(ConvertDatastream(dbDatastream, sensor: result));
                //    }
                //}
                //result.Datastreams.AddRange(datastreams);
                //Logging.Verbose("Result: {@Result}", result);
                return result;
            }
        }

        public Thing ConvertThing(db.SensorThingsThing dbThing/*, List<Datastream> datastreams = null*/)
        {
            using (Logging.MethodCall(GetType()))
            {
                var result = Mapper.Map<Thing>(dbThing);
                result.Properties.Add("kind", dbThing.Kind);
                if (!string.IsNullOrWhiteSpace(dbThing.Url)) result.Properties.Add("url", dbThing.Url);
                if (dbThing.StartDate.HasValue) result.Properties.Add("startDate", dbThing.StartDate.Value.ToString("o"));
                if (dbThing.EndDate.HasValue) result.Properties.Add("endDate", dbThing.EndDate.Value.ToString("o"));
                //var dbLocation = DbContext.SensorThingsLocations.Where(i => i.Id == dbThing.Id).FirstOrDefault();
                //if (dbLocation != null)
                //{
                //    var location = ConvertLocation(dbLocation, result);
                //    result.Locations.Add(location);
                //    result.HistoricalLocations.Add(ConvertHistoricalLocation(dbLocation.StartDate ?? dbLocation.EndDate ?? DateTime.Now, location, result));
                //}
                //if (datastreams == null)
                //{
                //    datastreams = new List<Datastream>();
                //    var dbDatastreams = DbContext.SensorThingsDatastreams.Where(i => i.InstrumentId == dbThing.Id);
                //    foreach (var dbDatastream in dbDatastreams)
                //    {
                //        datastreams.Add(ConvertDatastream(dbDatastream, result));
                //    }
                //}
                //result.Datastreams.AddRange(datastreams);
                //Logging.Verbose("Result: {@Result}", result);
                return result;
            }
        }

    }
}
