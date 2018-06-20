using Newtonsoft.Json.Linq;
using SAEON.SensorThings;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
{
    [RoutePrefix("SensorThings/Locations")]
    public class LocationsSOSController : BaseController<Location>
    {
        protected override List<Location> GetList()
        {
            var result = base.GetList();
            foreach (var station in db.Stations.OrderBy(i => i.Name))
            {
                var name = $"Station {station.Name}";
                if ((station.Latitude.HasValue && station.Longitude.HasValue) && (!result.Any(i => (i.Name == name))))
                {
                    var location = new Location
                    {
                        Uri = Request.RequestUri,
                        Id = station.Id.GetHashCode(),
                        Name = name,
                        Description = station.Description,
                        Latitude = station.Latitude.Value,
                        Longitude = station.Longitude.Value,
                        Elevation = station.Elevation
                    };
                    result.Add(location);
                }
            }
            foreach (var instrument in db.Instruments.OrderBy(i => i.Name))
            {
                var name = $"Instrument {instrument.Name}";
                if ((instrument.Latitude.HasValue && instrument.Longitude.HasValue) && (!result.Any(i => (i.Name == name))))
                {
                    var location = new Location
                    {
                        Uri = Request.RequestUri,
                        Id = instrument.Id.GetHashCode(),
                        Name = name,
                        Description = instrument.Description,
                        Latitude = instrument.Latitude.Value,
                        Longitude = instrument.Longitude.Value,
                        Elevation = instrument.Elevation
                    };
                    result.Add(location);
                }
            }
            foreach (var sensor in db.Sensors.OrderBy(i => i.Name))
            {
                var name = $"Sensor {sensor.Name}";
                if ((sensor.Latitude.HasValue && sensor.Longitude.HasValue) && (!result.Any(i => (i.Name == name))))
                {
                    var location = new Location
                    {
                        Uri = Request.RequestUri,
                        Id = sensor.Id.GetHashCode(),
                        Name = name,
                        Description = sensor.Description,
                        Latitude = sensor.Latitude.Value,
                        Longitude = sensor.Longitude.Value,
                        Elevation = sensor.Elevation
                    };
                    result.Add(location);
                }
            }
            return result;
        }

        //[HttpGet]
        //[Route]
        public override JToken GetAll()
        {
            return base.GetAll();
        }

    }
}
