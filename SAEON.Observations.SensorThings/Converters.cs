using AutoMapper;
using Microsoft.Spatial;
using SAEON.Logs;
using System;
using System.Collections.Generic;
using db = SAEON.Observations.Core.Entities;

namespace SAEON.Observations.SensorThings
{
    public static class Converters
    {
        public static Thing ConvertThing(IMapper mapper, db.SensorThingsThing dbThing)
        {
            using (Logging.MethodCall(typeof(Converters)))
            {
                var result = mapper.Map<Thing>(dbThing);
                result.Properties.Add("kind", dbThing.Kind);
                if (!string.IsNullOrWhiteSpace(dbThing.Url)) result.Properties.Add("url", dbThing.Url);
                if (dbThing.StartDate.HasValue) result.Properties.Add("startDate", dbThing.StartDate.Value.ToString("o"));
                if (dbThing.EndDate.HasValue) result.Properties.Add("endDate", dbThing.EndDate.Value.ToString("o"));
                //Logging.Verbose("Result: {@Result}", result);
                return result;
            }
        }

        public static Location ConvertLocation(IMapper mapper, db.SensorThingsLocation dbLocation)
        {
            using (Logging.MethodCall(typeof(Converters)))
            {
                var result = mapper.Map<Location>(dbLocation);
                result.location = GeographyPoint.Create(dbLocation.Latitude, dbLocation.Longitude, dbLocation.Elevation);
                //Logging.Verbose("Result: {@Result}", result);
                return result;
            }
        }

        public static HistoricalLocation ConvertHistoricalLocation(IMapper mapper, db.SensorThingsLocation dbLocation, db.SensorThingsThing dbThing)
        {
            using (Logging.MethodCall(typeof(Converters)))
            {
                var location = ConvertLocation(mapper, dbLocation);
                var thing = ConvertThing(mapper, dbThing);
                var result = new HistoricalLocation { Id = location.Id, Time = dbLocation.StartDate ?? dbLocation.EndDate ?? DateTime.Now, Thing = thing, Locations = new List<Location> { location } };
                //Logging.Verbose("Result: {@Result}", result);
                return result;
            }
        }

        public static Datastream ConvertDatastream(IMapper mapper, db.SensorThingsDatastream dbDatastream, db.SensorThingsThing dbThing)
        {
            GeographyPolygon CreatePolygon(double top, double left, double bottom, double right)
            {
                return GeographyFactory.Polygon().Ring(left, top).LineTo(left, top).LineTo(right, top).LineTo(right, bottom).LineTo(left, bottom).LineTo(left, top).Build();
            }

            using (Logging.MethodCall(typeof(Converters)))
            {
                var result = mapper.Map<Datastream>(dbDatastream);
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
                result.Thing = ConvertThing(mapper, dbThing);
                //Logging.Verbose("Result: {@Result}", result);
                return result;
            }
        }

        public static ObservedProperty ConverObservedProperty(IMapper mapper, db.SensorThingsObservedPropery dbObservedProperty)
        {
            var result = mapper.Map<ObservedProperty>(dbObservedProperty);
            //Logging.Verbose("Result: {@Result}", result);
            return result;
        }
    }
}
