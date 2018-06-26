using Newtonsoft.Json.Linq;
using e = SAEON.Observations.Core.Entities;
using SAEON.SensorThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
{
    [RoutePrefix("SensorThings/Locations")]
    public class LocationsSOSController : BaseController<Location>
    {

        protected override Location GetEntity(int id)
        {
            var station = db.Stations.Where(i => i.Latitude.HasValue && i.Longitude.HasValue).ToList().FirstOrDefault(i => i.Id.GetHashCode() == id);
            if (station != null) return SensorThingsFactory.LocationFromStation(station, Request.RequestUri);
            var instrument = db.Instruments.Where(i => i.Latitude.HasValue && i.Longitude.HasValue).ToList().FirstOrDefault(i => i.Id.GetHashCode() == id);
            if (instrument != null) return SensorThingsFactory.LocationFromInstrument(instrument, Request.RequestUri);
            var sensor = db.Sensors.Where(i => i.Latitude.HasValue && i.Longitude.HasValue).ToList().FirstOrDefault(i => i.Id.GetHashCode() == id);
            if (sensor != null) return SensorThingsFactory.LocationFromSensor(sensor, Request.RequestUri);
            return null;
        }

        protected override List<Location> GetEntities()
        {
            var result = base.GetEntities();
            foreach (var station in db.Stations.Where(i => i.Latitude.HasValue && i.Longitude.HasValue))
            {
                if (!result.Any(i => i.Name == $"Station {station.Name}"))
                {
                    result.Add(SensorThingsFactory.LocationFromStation(station, Request.RequestUri));
                }
            }
            foreach (var instrument in db.Instruments.Where(i => i.Latitude.HasValue && i.Longitude.HasValue))
            {
                if (!result.Any(i => i.Name == $"Instrument {instrument.Name}"))
                {
                    result.Add(SensorThingsFactory.LocationFromInstrument(instrument, Request.RequestUri));
                }
            }
            foreach (var sensor in db.Sensors.Where(i => i.Latitude.HasValue && i.Longitude.HasValue))
            {
                if (!result.Any(i => i.Name == $"Sensor {sensor.Name}"))
                {
                    result.Add(SensorThingsFactory.LocationFromSensor(sensor, Request.RequestUri));
                }
            }
            return result.OrderBy(i => i.Name).ToList();
        }

        protected List<Thing> GetRelatedThings(int id)
        {
            var result = new List<Thing>();
            var location = GetEntity(id);
            if (location != null)
            {
                result.AddRange(location.Things);
                //foreach (var station in db.Stations.Where(i => i.Latitude.HasValue && i.Longitude.HasValue).ToList().Where(i => i.Id.GetHashCode() == location.Id))
                //{
                //    result.Add(SensorThingsFactory.ThingFromStation(station, Request.RequestUri));
                //}
                //foreach (var instrument in db.Instruments.Where(i => i.Latitude.HasValue && i.Longitude.HasValue).ToList().Where(i => i.Id.GetHashCode() == location.Id))
                //{
                //    result.Add(SensorThingsFactory.ThingFromInstrument(instrument, Request.RequestUri));
                //}
                //foreach (var sensor in db.Sensors.Where(i => i.Latitude.HasValue && i.Longitude.HasValue).ToList().Where(i => i.Id.GetHashCode() == location.Id))
                //{
                //    result.Add(SensorThingsFactory.ThingFromSensor(sensor, Request.RequestUri));
                //}
            }
            return result.OrderBy(i => i.Name).ToList();
        }

        public override JToken GetAll()
        {
            return base.GetAll();
        }

        [Route("~/SensorThings/Locations({id:int})")]
        public override JToken GetById([FromUri] int id)
        {
            return base.GetById(id);
        }

        [HttpGet]
        [Route("~/SensorThings/Locations({id:int})/Things")]
        public JToken GetThings([FromUri] int id)
        {
            return GetMany<Thing>(id, GetRelatedThings);
        }


    }
}
