using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using ef = SAEON.Observations.Core.Entities;
using geoF = GeoJSON.Net.Feature;
using geoG = GeoJSON.Net.Geometry;
using sos = SAEON.Observations.Core.SensorThings;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
{
    public static class SensorThingsHelper
    {
        public static List<sos.Thing> Things = new List<sos.Thing>();
        public static List<sos.Location> Locations = new List<sos.Location>();

        private static sos.Location AddLocation(string name, string description, double latitude, double longitude, int? elevation)
        {
            var location = Locations.FirstOrDefault(i => (i.Name == name) && (i.Description == description) && (i?.Point.Latitude == latitude) && (i?.Point.Longitude == longitude));
            if (location == null)
            {
                location = new sos.Location
                {
                    Name = name,
                    Point = new sos.LatLong(latitude, longitude)
                };
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
                            Id = Guid.NewGuid(),
                            Name = organisation.Name,
                            Description = organisation.Description
                        };
                        thing.AddProperty("Kind", "Organisation");
                        thing.AddProperty("Url", organisation.Url);
                        Things.Add(thing);
                    }
                    foreach (var site in db.Sites.OrderBy(i => i.Name))
                    {
                        var thing = new sos.Thing
                        {
                            Id = Guid.NewGuid(),
                            Name = site.Name,
                            Description = site.Description
                        };
                        thing.AddProperty("Kind", "Site");
                        thing.AddProperty("Url", site.Url);
                        thing.AddProperty("StartDate", site?.StartDate);
                        thing.AddProperty("EndDate", site?.EndDate);
                        Things.Add(thing);
                    }
                    foreach (var station in db.Stations.OrderBy(i => i.Name))
                    {
                        var thing = new sos.Thing
                        {
                            Id = Guid.NewGuid(),
                            Name = station.Name,
                            Description = station.Description
                        };
                        thing.AddProperty("Kind", "Station");
                        thing.AddProperty("Url", station.Url);
                        thing.AddProperty("StartDate", station?.StartDate);
                        thing.AddProperty("EndDate", station?.EndDate);
                        if (station.Latitude.HasValue && station.Longitude.HasValue)
                        {
                            var location = AddLocation(station.Name, station.Description, station.Latitude.Value, station.Longitude.Value, station.Elevation);
                            location.GenerateSensorThingsProperties();
                            thing.Locations.Add(location);
                            location.Things.Add(thing);
                        }
                        Things.Add(thing);
                    }
                    foreach (var instrument in db.Instruments.OrderBy(i => i.Name))
                    {
                        var thing = new sos.Thing
                        {
                            Id = Guid.NewGuid(),
                            Name = instrument.Name,
                            Description = instrument.Description
                        };
                        thing.AddProperty("Kind", "Instrument");
                        thing.AddProperty("Url", instrument.Url);
                        thing.AddProperty("StartDate", instrument?.StartDate);
                        thing.AddProperty("EndDate", instrument?.EndDate);
                        Things.Add(thing);
                    }
                }
            }
        }
    }
}