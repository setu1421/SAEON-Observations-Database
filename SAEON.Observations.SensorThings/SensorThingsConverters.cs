using AutoMapper;
using SAEON.Logs;
using db = SAEON.Observations.Core.Entities;

namespace SAEON.Observations.SensorThings
{
    public static class SensorThingsConverters
    {
        public static Thing ConvertThing(IMapper mapper, db.SensorThingsThing dbEntity)
        {
            using (Logging.MethodCall(typeof(SensorThingsConverters)))
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
            using (Logging.MethodCall(typeof(SensorThingsConverters)))
            {
                var result = mapper.Map<Location>(dbEntity);
                result.location = new GeometryLocation(dbEntity.Latitude, dbEntity.Longitude, dbEntity.Elevation);
                //Logging.Verbose("Result: {@Result}", result);
                return result;
            }
        }
    }
}
