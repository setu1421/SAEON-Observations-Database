/*
using Newtonsoft.Json.Linq;
using e = SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{
    [RoutePrefix("SensorThingsV2/Things")]
    public class ThingsSTControllerV2 : BaseControllerV2<Thing>
    {
        protected override IQueryable<Thing> GetEntities()
        {
            var result = new List<Thing>();
            e.Site dbSite = null;
            e.Station dbStation = null;
            e.Instrument dbInstrument = null;
            e.Sensor dbSensor = null;
            foreach (var inventory in DbContext.Inventory)
            {
                if (dbSite?.Code != inventory.SiteCode)
                {
                    dbSite = DbContext.Sites.First(i => i.Code == inventory.SiteCode);
                    dbStation = null;
                    dbInstrument = null;
                    dbSensor = null;
                    var thing = new Thing
                    {
                        Id = dbSite.Id,
                        Name = dbSite.Name,
                        Description = dbSite.Description
                    };
                    thing.Properties.Add("Kind", "Site");
                    if (!string.IsNullOrWhiteSpace(dbSite.Url)) thing.Properties.Add("Url", dbSite.Url);
                    if (dbSite.StartDate.HasValue) thing.Properties.Add("StartDate", dbSite.StartDate);
                    if (dbSite.EndDate.HasValue) thing.Properties.Add("EndDate", dbSite.EndDate);
                    result.Add(thing);
                }
                if (dbStation?.Code != inventory.StationCode)
                {
                    dbStation = DbContext.Stations.First(i => i.Code == inventory.StationCode);
                    dbInstrument = null;
                    dbSensor = null;
                    var thing = new Thing
                    {
                        Id = dbStation.Id,
                        Name = dbStation.Name,
                        Description = dbStation.Description
                    };
                    thing.Properties.Add("Kind", "Station");
                    if (!string.IsNullOrWhiteSpace(dbStation.Url)) thing.Properties.Add("Url", dbStation.Url);
                    if (dbStation.StartDate.HasValue) thing.Properties.Add("StartDate", dbStation.StartDate);
                    if (dbStation.EndDate.HasValue) thing.Properties.Add("EndDate", dbStation.EndDate);
                    result.Add(thing);
                    //AddStationLocation(dbStation, thing);
                }
                if (dbInstrument?.Code != inventory.InstrumentCode)
                {
                    dbInstrument = DbContext.Instruments.First(i => i.Code == inventory.InstrumentCode);
                    dbSensor = null;
                    var thing = new Thing
                    {
                        Id = dbInstrument.Id,
                        Name = dbInstrument.Name,
                        Description = dbInstrument.Description
                    };
                    thing.Properties.Add("Kind", "Instrument");
                    if (!string.IsNullOrWhiteSpace(dbInstrument.Url)) thing.Properties.Add("Url", dbInstrument.Url);
                    if (dbInstrument.StartDate.HasValue) thing.Properties.Add("StartDate", dbInstrument.StartDate);
                    if (dbInstrument.EndDate.HasValue) thing.Properties.Add("EndDate", dbInstrument.EndDate);
                    result.Add(thing);
                    //var location = AddInstrumentLocation(dbStation, dbInstrument, thing);
                    //if (location != null)
                    //{
                    //    // Feature of interest
                    //    var featureOfInterest = new FeatureOfInterest
                    //    {
                    //        Id = dbInstrument.Id.GetHashCode(),
                    //        Name = dbInstrument.Name,
                    //        Description = dbInstrument.Description,
                    //        Coordinate = location.Coordinate
                    //    };
                    //    FeaturesOfInterest.Add(featureOfInterest);
                    //}
                }
                if (dbSensor?.Code != inventory.SensorCode)
                {
                    dbSensor = DbContext.Sensors.First(i => i.Code == inventory.SensorCode);
                    var thing = new Thing
                    {
                        Id = dbSensor.Id,
                        Name = dbSensor.Name,
                        Description = dbSensor.Description
                    };
                    thing.Properties.Add("Kind", "Sensor");
                    if (!string.IsNullOrWhiteSpace(dbSensor.Url)) thing.Properties.Add("Url", dbSensor.Url);
                    //if (dbSensor.StartDate.HasValue) thing.Properties.Add("StartDate", dbSensor.StartDate);
                    //if (dbSensor.EndDate.HasValue) thing.Properties.Add("EndDate", dbSensor.EndDate);
                    result.Add(thing);
                    //var location = AddSensorLocation(dbStation, dbInstrument, dbSensor, thing);
                    //var phenomenon = db.Phenomena.First(i => i.Code == inventory.PhenomenonCode);
                    //var offering = db.Offerings.First(i => i.Code == inventory.OfferingCode);
                    //var unit = db.Units.First(i => i.Code == inventory.UnitCode);
                    //// Sensor
                    //var sensor = new Sensor
                    //{
                    //    Id = dbSensor.Id.GetHashCode(),
                    //    Name = dbSensor.Name,
                    //    Description = dbSensor.Description,
                    //    Metadata = dbSensor.Url
                    //};
                    //Sensors.Add(sensor);
                    //// Datastream
                    //var datastream = new Datastream
                    //{
                    //    Id = dbSensor.Id.GetHashCode(),
                    //    Name = $"{dbSensor.Name} - {phenomenon.Name} - {offering.Name} - {unit.Name}",
                    //    Description = $"{phenomenon.Name} - {offering.Name} - {unit.Name}",
                    //    Thing = thing
                    //};
                    //datastream.UnitOfMeasurement.Name = $"{unit.Name}";
                    //datastream.UnitOfMeasurement.Symbol = $"{unit.Symbol}";
                    //if (inventory.LatitudeNorth.HasValue && inventory.LatitudeSouth.HasValue && inventory.LongitudeWest.HasValue && inventory.LongitudeEast.HasValue)
                    //{
                    //    datastream.ObservedArea = new BoundingBox
                    //    {
                    //        Top = inventory.LatitudeNorth.Value,
                    //        Bottom = inventory.LatitudeSouth.Value,
                    //        Left = inventory.LongitudeWest.Value,
                    //        Right = inventory.LongitudeEast.Value
                    //    };
                    //}
                    //if (inventory.StartDate.HasValue && inventory.EndDate.HasValue)
                    //{
                    //    datastream.PhenomenonTime = new TimeInterval(inventory.StartDate.Value, inventory.EndDate.Value);
                    //}
                    //Datastreams.Add(datastream);
                    //sensor.Datastream = datastream;
                    //datastream.Sensor = sensor;
                }
            }
            return result.AsQueryable();
        }

        public override JToken GetAll()
        {
            return base.GetAll();
        }

        [Route("~/SensorThings/Things({id:guid})")]
        public override JToken GetById([FromUri] Guid id)
        {
            return base.GetById(id);
        }


        [Route("~/SensorThings/Things({id:int})/Location")]
        public JToken GetLocation([FromUri] int id)
        {
            return GetSingle(id, i => i.Location);
        }

        [Route("~/SensorThings/Things({id:int})/HistoricalLocations")]
        public JToken GetHistoricalLocations([FromUri] int id)
        {
            return GetMany(id, i => i.HistoricalLocations);
        }

        [Route("~/SensorThings/Things({id:int})/Datastreams")]
        public JToken GetDatastreams([FromUri] int id)
        {
            return GetMany(id, i => i.Datastreams);
        }

    }
}
*/