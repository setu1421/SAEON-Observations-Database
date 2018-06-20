using Newtonsoft.Json.Linq;
using SAEON.SensorThings;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
{
    [RoutePrefix("SensorThings/Things")]
    public class ThingsSOSController : BaseController<Thing>
    {
        protected override List<Thing> GetList()
        {
            var result = base.GetList();
            foreach (var site in db.Sites.OrderBy(i => i.Name))
            {
                var thing = new Thing
                {
                    Uri = Request.RequestUri,
                    Id = site.Id.GetHashCode(),
                    Name = $"Site {site.Name}",
                    Description = site.Description
                };
                thing.Properties.Add("kind", "Site");
                if (!string.IsNullOrWhiteSpace(site.Url))
                {
                    thing.Properties.Add("url", site.Url);
                }
                if (site.StartDate.HasValue)
                {
                    thing.Properties.Add("startDate", site.StartDate.Value.ToString("o"));
                }
                if (site.EndDate.HasValue)
                {
                    thing.Properties.Add("endDate", site.EndDate.Value.ToString("o"));
                }
                result.Add(thing);
            }
            foreach (var station in db.Stations.OrderBy(i => i.Name))
            {
                var thing = new Thing
                {
                    Uri = Request.RequestUri,
                    Id = station.Id.GetHashCode(),
                    Name = $"Station {station.Name}",
                    Description = station.Description
                };
                thing.Properties.Add("kind", "Station");
                if (!string.IsNullOrWhiteSpace(station.Url))
                {
                    thing.Properties.Add("url", station.Url);
                }
                if (station.StartDate.HasValue)
                {
                    thing.Properties.Add("startDate", station.StartDate.Value.ToString("o"));
                }
                if (station.EndDate.HasValue)
                {
                    thing.Properties.Add("endDate", station.EndDate.Value.ToString("o"));
                }
                result.Add(thing);
            }
            foreach (var instrument in db.Instruments.OrderBy(i => i.Name))
            {
                var thing = new Thing
                {
                    Uri = Request.RequestUri,
                    Id = instrument.Id.GetHashCode(),
                    Name = $"Instrument {instrument.Name}",
                    Description = instrument.Description
                };
                thing.Properties.Add("kind", "Instrument");
                if (!string.IsNullOrWhiteSpace(instrument.Url))
                {
                    thing.Properties.Add("url", instrument.Url);
                }
                if (instrument.StartDate.HasValue)
                {
                    thing.Properties.Add("startDate", instrument.StartDate.Value.ToString("o"));
                }
                if (instrument.EndDate.HasValue)
                {
                    thing.Properties.Add("endDate", instrument.EndDate.Value.ToString("o"));
                }
                result.Add(thing);
            }
            foreach (var sensor in db.Sensors.OrderBy(i => i.Name))
            {
                var thing = new Thing
                {
                    Uri = Request.RequestUri,
                    Id = sensor.Id.GetHashCode(),
                    Name = $"Sensor {sensor.Name}",
                    Description = sensor.Description
                };
                thing.Properties.Add("kind", "Sensor");
                if (!string.IsNullOrWhiteSpace(sensor.Url))
                {
                    thing.Properties.Add("url", sensor.Url);
                }
                result.Add(thing);
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
