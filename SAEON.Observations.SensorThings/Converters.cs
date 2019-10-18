using AutoMapper;
using Microsoft.Spatial;
using SAEON.Logs;
using db = SAEON.Observations.Core.Entities;

namespace SAEON.Observations.SensorThings
{
    public static class Converters
    {
        public static Thing ConvertThing(IMapper mapper, db.SensorThingsThing dbEntity)
        {
            using (Logging.MethodCall(typeof(Converters)))
            {
                var result = mapper.Map<Thing>(dbEntity);
                result.Properties.Add("kind", dbEntity.Kind);
                if (!string.IsNullOrWhiteSpace(dbEntity.Url)) result.Properties.Add("url", dbEntity.Url);
                if (dbEntity.StartDate.HasValue) result.Properties.Add("startDate", dbEntity.StartDate.Value.ToString("o"));
                if (dbEntity.EndDate.HasValue) result.Properties.Add("EndDate", dbEntity.EndDate.Value.ToString("o"));
                //Logging.Verbose("Result: {@Result}", result);
                return result;
            }
        }

        public static Location ConvertLocation(IMapper mapper, db.SensorThingsLocation dbEntity)
        {
            using (Logging.MethodCall(typeof(Converters)))
            {
                var result = mapper.Map<Location>(dbEntity);
                result.location = GeographyPoint.Create(dbEntity.Latitude, dbEntity.Longitude, dbEntity.Elevation);
                //Logging.Verbose("Result: {@Result}", result);
                return result;
            }
        }

        public static Datastream ConvertDatastream(IMapper mapper, db.SensorThingsDatastream dbEntity)
        {
            GeographyPolygon CreatePolygon(double top, double left, double bottom, double right)
            {
                return GeographyFactory.Polygon().Ring(left, top).LineTo(left, top).LineTo(right, top).LineTo(right, bottom).LineTo(left, bottom).LineTo(left, top).Build();
            }

            using (Logging.MethodCall(typeof(Converters)))
            {
                var result = mapper.Map<Datastream>(dbEntity);
                result.UnitOfMeasurement = new UnitOfMeasurement
                {
                    Name = $"{dbEntity.PhenomenonName} - {dbEntity.OfferingName} - {dbEntity.UnitOfMeasureUnit}",
                    Symbol = dbEntity.UnitOfMeasureSymbol,
                    Definition = string.IsNullOrWhiteSpace(dbEntity.PhenomenonUrl) ? "http://data.saeon.ac.za" : dbEntity.PhenomenonUrl
                };
                result.Name = $"{dbEntity.Name} - {dbEntity.PhenomenonName} - {dbEntity.OfferingName} - {dbEntity.UnitOfMeasureUnit}";
                result.Description = $"{dbEntity.Description} - {dbEntity.PhenomenonDescription} - {dbEntity.OfferingDescription} - {dbEntity.UnitOfMeasureUnit}";
                if (dbEntity.LatitudeNorth.HasValue && dbEntity.LongitudeWest.HasValue && dbEntity.LatitudeSouth.HasValue && dbEntity.LongitudeEast.HasValue)
                {
                    result.ObservedArea = CreatePolygon(dbEntity.LatitudeNorth.Value, dbEntity.LongitudeWest.Value, dbEntity.LatitudeSouth.Value, dbEntity.LongitudeEast.Value);
                }
                if (dbEntity.StartDate.HasValue && dbEntity.EndDate.HasValue)
                {
                    result.PhenomenonTime = new TimeInterval(dbEntity.StartDate.Value, dbEntity.EndDate.Value);
                    result.ResultTime = new TimeInterval(dbEntity.StartDate.Value, dbEntity.EndDate.Value);
                }
                //Logging.Verbose("Result: {@Result}", result);
                return result;
            }
        }
    }
}
