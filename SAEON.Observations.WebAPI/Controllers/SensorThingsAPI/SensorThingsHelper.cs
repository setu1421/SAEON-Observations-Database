using SAEON.Observations.Core;
using SAEON.Observations.Core.GeoJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using ef = SAEON.Observations.Core.Entities;
using sos = SAEON.Observations.Core.SensorThings;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
{
    public static class SensorThingsHelper
    {
        public static List<sos.Thing> Things = new List<sos.Thing>();
        public static List<sos.Location> Locations = new List<sos.Location>();
        public static List<sos.HistoricalLocation> HistoricalLocations = new List<sos.HistoricalLocation>();

        private static sos.Location AddLocation(string name, string description, double latitude, double longitude, int? elevation)
        {
            var location = Locations.FirstOrDefault(i => (i.name == name) && (i.description == description) && (i?.Coordinate.Latitude == latitude) && (i?.Coordinate.Longitude == longitude) && (!(elevation.HasValue && (i?.Coordinate?.Elevation.HasValue ?? false) || (i?.Coordinate.Elevation == elevation))));
            if (location == null)
            {
                location = new sos.Location
                {
                    id = Guid.NewGuid(),
                    name = name,
                    description = description,
                    Coordinate = new Coordinate { Latitude = latitude, Longitude = longitude, Elevation = elevation }
                };
                location.GenerateSensorThingsProperties();
                Locations.Add(location);
            }
            return location;
        }

        public static void Load()
        {
            using (Logging.MethodCall(typeof(SensorThingsHelper)))
            {
                using (ef.ObservationsDbContext db = new ef.ObservationsDbContext())
                {
                    Things.Clear();
                    Locations.Clear();
                    foreach (var organisation in db.Organisations.OrderBy(i => i.Name))
                    {
                        var thing = new sos.Thing
                        {
                            id = Guid.NewGuid(),
                            name = organisation.Name,
                            description = organisation.Description
                        };
                        thing.AddProperty("Kind", "Organisation");
                        thing.AddProperty("Url", organisation.Url);
                        thing.GenerateSensorThingsProperties();
                        Things.Add(thing);
                    }
                    foreach (var site in db.Sites.OrderBy(i => i.Name))
                    {
                        var thing = new sos.Thing
                        {
                            id = Guid.NewGuid(),
                            name = site.Name,
                            description = site.Description
                        };
                        thing.AddProperty("Kind", "Site");
                        thing.AddProperty("Url", site.Url);
                        thing.AddProperty("StartDate", site?.StartDate);
                        thing.AddProperty("EndDate", site?.EndDate);
                        thing.GenerateSensorThingsProperties();
                        Things.Add(thing);
                    }
                    foreach (var station in db.Stations.OrderBy(i => i.Name))
                    {
                        var thing = new sos.Thing
                        {
                            id = Guid.NewGuid(),
                            name = station.Name,
                            description = station.Description
                        };
                        thing.AddProperty("Kind", "Station");
                        thing.AddProperty("Url", station.Url);
                        thing.AddProperty("StartDate", station?.StartDate);
                        thing.AddProperty("EndDate", station?.EndDate);
                        if (station.Latitude.HasValue && station.Longitude.HasValue)
                        {
                            var location = AddLocation(station.Name, station.Description, station.Latitude.Value, station.Longitude.Value, station.Elevation);
                            thing.Locations.Add(location);
                            location.Things.Add(thing);
                            var historicalLocation = new sos.HistoricalLocation { Time = station?.StartDate ?? DateTime.Now };
                            historicalLocation.GenerateSensorThingsProperties();
                            historicalLocation.Locations.Add(location);
                            historicalLocation.Thing = thing;
                            thing.HistoricalLocations.Add(historicalLocation);
                        }
                        thing.GenerateSensorThingsProperties();
                        Things.Add(thing);
                    }
                    foreach (var instrument in db.Instruments.OrderBy(i => i.Name))
                    {
                        var thing = new sos.Thing
                        {
                            id = Guid.NewGuid(),
                            name = instrument.Name,
                            description = instrument.Description
                        };
                        thing.AddProperty("Kind", "Instrument");
                        thing.AddProperty("Url", instrument.Url);
                        thing.AddProperty("StartDate", instrument?.StartDate);
                        thing.AddProperty("EndDate", instrument?.EndDate);
                        thing.GenerateSensorThingsProperties();
                        Things.Add(thing);
                    }
                }
            }
        }
    }
}