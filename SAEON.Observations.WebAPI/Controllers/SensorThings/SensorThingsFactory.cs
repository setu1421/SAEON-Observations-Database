using e = SAEON.Observations.Core.Entities;
using SAEON.SensorThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAEON.Logs;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{
    public static class SensorThingsFactory
    {
        public static List<Thing> Things { get; } = new List<Thing>();
        public static List<Location> Locations { get; } = new List<Location>();
        public static List<HistoricalLocation> HistoricalLocations { get; } = new List<HistoricalLocation>();
        public static List<Datastream> Datastreams { get; } = new List<Datastream>();
        public static List<Sensor> Sensors { get; } = new List<Sensor>();
        public static List<ObservedProperty> ObservedProperties { get; } = new List<ObservedProperty>();
        public static List<Observation> Observations { get; } = new List<Observation>();
        public static List<FeatureOfInterest> FeaturesOfInterest { get; } = new List<FeatureOfInterest>();

        public static void Load()
        {
            Location AddLocation(Guid id, string name, string description, double latitude, double longitude, double? elevation)
            {
                var location = Locations.FirstOrDefault(i => (i.Name == name) && (i.Description == description) &&
                        (i.Coordinate.Latitude == latitude) && (i.Coordinate.Longitude == longitude) && 
                        (!(i.Coordinate.Elevation.HasValue && elevation.HasValue) || (i.Coordinate.Elevation == elevation)));
                if (location == null)
                {
                    location = new Location
                    {
                        Id = id.GetHashCode(),
                        Name = name,
                        Description = description,
                        Coordinate = new Coordinate(latitude,longitude,elevation)
                    };
                    Locations.Add(location);
                }
                return location;
            }

            Location AddStationLocation(e.Station station, Thing thing)
            {
                Location location = null; 
                if (station.Latitude.HasValue && station.Longitude.HasValue)
                {
                    location = AddLocation(station.Id, station.Name, station.Description, station.Latitude.Value, station.Longitude.Value, station.Elevation);
                    thing.Location = location;
                    location.Things.Add(thing);
                    var historicalLocation = new HistoricalLocation(station?.StartDate);
                    historicalLocation.Locations.Add(location);
                    location.HistoricalLocations.Add(historicalLocation);
                    historicalLocation.Thing = thing;
                    thing.HistoricalLocations.Add(historicalLocation);
                }
                return location;
            }

            Location GetStationLocation(e.Station station)
            {
                return Locations.FirstOrDefault(i => (i.Id == station.Id.GetHashCode()) && (i.Name == station.Name) && (i.Description == station.Description) &&
                            (i.Coordinate.Latitude == station.Latitude) && (i.Coordinate.Longitude == station.Longitude) &&
                            (!(i.Coordinate.Elevation.HasValue && station.Elevation.HasValue) || (i.Coordinate.Elevation == station.Elevation)));
            }

            Location AddInstrumentLocation(e.Station station, e.Instrument instrument, Thing thing)
            {
                Location location = null;
                if (instrument.Latitude.HasValue && instrument.Longitude.HasValue)
                {
                    location = AddLocation(instrument.Id, instrument.Name, instrument.Description, instrument.Latitude.Value, instrument.Longitude.Value, instrument.Elevation);
                }
                else
                {
                    location = GetStationLocation(station);
                }
                if (location != null)
                {
                    thing.Location = location;
                    location.Things.Add(thing);
                    var historicalLocation = new HistoricalLocation(instrument?.StartDate ?? station?.StartDate);
                    historicalLocation.Locations.Add(location);
                    location.HistoricalLocations.Add(historicalLocation);
                    historicalLocation.Thing = thing;
                    thing.HistoricalLocations.Add(historicalLocation);
                }
                return location;
            }

            Location GetInstrumentLocation(e.Instrument instrument)
            {
                return Locations.FirstOrDefault(i => (i.Id == instrument.Id.GetHashCode()) && (i.Name == instrument.Name) && (i.Description == instrument.Description) &&
                            (i.Coordinate.Latitude == instrument.Latitude) && (i.Coordinate.Longitude == instrument.Longitude) &&
                            (!(i.Coordinate.Elevation.HasValue && instrument.Elevation.HasValue) || (i.Coordinate.Elevation == instrument.Elevation)));
            }

            Location AddSensorLocation(e.Station station, e.Instrument instrument, e.Sensor sensor, Thing thing)
            {
                Location location = null;
                if (sensor.Latitude.HasValue && sensor.Longitude.HasValue)
                {
                    location = AddLocation(sensor.Id, sensor.Name, sensor.Description, sensor.Latitude.Value, sensor.Longitude.Value, sensor.Elevation);
                }
                else
                {
                    location = GetInstrumentLocation(instrument) ?? GetStationLocation(station);
                }
                if (location != null)
                {
                    thing.Location = location;
                    location.Things.Add(thing);
                    var historicalLocation = new HistoricalLocation(instrument?.StartDate ?? station?.StartDate);
                    historicalLocation.Locations.Add(location);
                    location.HistoricalLocations.Add(historicalLocation);
                    historicalLocation.Thing = thing;
                    thing.HistoricalLocations.Add(historicalLocation);
                }
                return location;
            }

            Location GetSensorLocation(e.Sensor sensor)
            {
                return Locations.FirstOrDefault(i => (i.Id == sensor.Id.GetHashCode()) && (i.Name == sensor.Name) && (i.Description == sensor.Description) &&
                            (i.Coordinate.Latitude == sensor.Latitude) && (i.Coordinate.Longitude == sensor.Longitude) &&
                            (!(i.Coordinate.Elevation.HasValue && sensor.Elevation.HasValue) || (i.Coordinate.Elevation == sensor.Elevation)));
            }

            using (Logging.MethodCall(typeof(SensorThingsFactory)))
            {
                using (var db = new e.ObservationsDbContext())
                {
                    Things.Clear();
                    Locations.Clear();
                    HistoricalLocations.Clear();
                    Datastreams.Clear();
                    Sensors.Clear();
                    ObservedProperties.Clear();
                    FeaturesOfInterest.Clear();
                    e.Site dbSite = null;
                    e.Station dbStation = null;
                    e.Instrument dbInstrument = null;
                    e.Sensor dbSensor = null;
                    foreach (var inventory in db.Inventory)
                    {
                        if (dbSite?.Code != inventory.SiteCode)
                        {
                            dbSite = db.Sites.First(i => i.Code == inventory.SiteCode);
                            dbStation = null;
                            dbInstrument = null;
                            dbSensor = null;
                            var thing = new Thing
                            {
                                Id = dbSite.Id.GetHashCode(),
                                Name = dbSite.Name,
                                Description = dbSite.Description
                            };
                            thing.Properties.Add("Kind", "Site");
                            if (!string.IsNullOrWhiteSpace(dbSite.Url)) thing.Properties.Add("Url", dbSite.Url);
                            if (dbSite.StartDate.HasValue) thing.Properties.Add("StartDate", dbSite.StartDate);
                            if (dbSite.EndDate.HasValue) thing.Properties.Add("EndDate", dbSite.EndDate);
                            Things.Add(thing);
                        }
                        if (dbStation?.Code != inventory.StationCode)
                        {
                            dbStation = db.Stations.First(i => i.Code == inventory.StationCode);
                            dbInstrument = null;
                            dbSensor = null;
                            var thing = new Thing
                            {
                                Id = dbStation.Id.GetHashCode(),
                                Name = dbStation.Name,
                                Description = dbStation.Description
                            };
                            thing.Properties.Add("Kind", "Station");
                            if (!string.IsNullOrWhiteSpace(dbStation.Url)) thing.Properties.Add("Url", dbStation.Url);
                            if (dbStation.StartDate.HasValue) thing.Properties.Add("StartDate", dbStation.StartDate);
                            if (dbStation.EndDate.HasValue) thing.Properties.Add("EndDate", dbStation.EndDate);
                            Things.Add(thing);
                            AddStationLocation(dbStation, thing);
                        }
                        if (dbInstrument?.Code != inventory.InstrumentCode)
                        {
                            dbInstrument = db.Instruments.First(i => i.Code == inventory.InstrumentCode);
                            dbSensor = null;
                            var thing = new Thing
                            {
                                Id = dbInstrument.Id.GetHashCode(),
                                Name = dbInstrument.Name,
                                Description = dbInstrument.Description
                            };
                            thing.Properties.Add("Kind", "Instrument");
                            if (!string.IsNullOrWhiteSpace(dbInstrument.Url)) thing.Properties.Add("Url", dbInstrument.Url);
                            if (dbInstrument.StartDate.HasValue) thing.Properties.Add("StartDate", dbInstrument.StartDate);
                            if (dbInstrument.EndDate.HasValue) thing.Properties.Add("EndDate", dbInstrument.EndDate);
                            Things.Add(thing);
                            var location = AddInstrumentLocation(dbStation, dbInstrument, thing);
                            if (location != null)
                            {
                                // Feature of interest
                                var featureOfInterest = new FeatureOfInterest
                                {
                                    Id = dbInstrument.Id.GetHashCode(),
                                    Name = dbInstrument.Name,
                                    Description = dbInstrument.Description,
                                    Coordinate = location.Coordinate
                                };
                                FeaturesOfInterest.Add(featureOfInterest);
                            }
                        }
                        if (dbSensor?.Code != inventory.SensorCode)
                        {
                            dbSensor = db.Sensors.First(i => i.Code == inventory.SensorCode);
                            var thing = new Thing
                            {
                                Id = dbSensor.Id.GetHashCode(),
                                Name = dbSensor.Name,
                                Description = dbSensor.Description
                            };
                            thing.Properties.Add("Kind", "Sensor");
                            if (!string.IsNullOrWhiteSpace(dbSensor.Url)) thing.Properties.Add("Url", dbSensor.Url);
                            //if (sensor.StartDate.HasValue) thing.Properties.Add("StartDate", sensor.StartDate);
                            //if (sensor.EndDate.HasValue) thing.Properties.Add("EndDate", sensor.EndDate);
                            Things.Add(thing);
                            var location = AddSensorLocation(dbStation, dbInstrument, dbSensor, thing);
                            var phenomenon = db.Phenomena.First(i => i.Code == inventory.PhenomenonCode);
                            var offering = db.Offerings.First(i => i.Code == inventory.OfferingCode);
                            var unit = db.Units.First(i => i.Code == inventory.UnitCode);
                            // Sensor
                            var sensor = new Sensor
                            {
                                Id = dbSensor.Id.GetHashCode(),
                                Name = dbSensor.Name,
                                Description = dbSensor.Description,
                                Metadata = dbSensor.Url
                            };
                            Sensors.Add(sensor);
                            // Datastream
                            var datastream = new Datastream
                            {
                                Id = dbSensor.Id.GetHashCode(),
                                Name = $"{dbSensor.Name} - {phenomenon.Name} - {offering.Name} - {unit.Name}",
                                Description = $"{phenomenon.Name} - {offering.Name} - {unit.Name}",
                                Thing = thing
                            };
                            datastream.UnitOfMeasurement.Name = $"{unit.Name}";
                            datastream.UnitOfMeasurement.Symbol = $"{unit.Symbol}";
                            if (inventory.TopLatitude.HasValue && inventory.BottomLatitude.HasValue && inventory.LeftLongitude.HasValue && inventory.RightLongitude.HasValue)
                            {
                                datastream.ObservedArea = new BoundingBox
                                {
                                    Top = inventory.TopLatitude.Value,
                                    Bottom = inventory.BottomLatitude.Value,
                                    Left = inventory.LeftLongitude.Value,
                                    Right = inventory.RightLongitude.Value
                                };
                            }
                            if (inventory.StartDate.HasValue && inventory.EndDate.HasValue)
                            {
                                datastream.PhenomenonTime = new TimeInterval(inventory.StartDate.Value, inventory.EndDate.Value);
                            }
                            Datastreams.Add(datastream);
                            sensor.Datastream = datastream;
                            datastream.Sensor = sensor;
                        }
                    }

                }
            }
        }

        /*
        #region Things
        public static Thing ThingFromStation(e.Station station, Uri uri, Location location = null)
        {
            var result = new Thing
            {
                //BaseUrl = uri.GetLeftPart(UriPartial.Authority) + "/SensorThings",
                Id = station.Id.GetHashCode(),
                Name = $"Station {station.Name}",
                Description = station.Description
            };
            result.Properties.Add("kind", "Station");
            if (!string.IsNullOrWhiteSpace(station.Url))
            {
                result.Properties.Add("url", station.Url);
            }
            if (station.StartDate.HasValue)
            {
                result.Properties.Add("startDate", station.StartDate.Value.ToString("o"));
            }
            if (station.EndDate.HasValue)
            {
                result.Properties.Add("endDate", station.EndDate.Value.ToString("o"));
            }
            if (location == null)
                result.Location = LocationFromStation(station, uri, result);
            else
                result.Location = location;
            var historicalLocation = new HistoricalLocation { Thing = result, Time = DateTime.Now };
            historicalLocation.Locations.Add(result.Location);
            result.HistoricalLocations.Add(historicalLocation);
            return result;
        }

        public static Thing ThingFromInstrument(e.Instrument instrument, Uri uri, Location location = null)
        {
            var result = new Thing
            {
                //BaseUrl = uri.GetLeftPart(UriPartial.Authority) + "/SensorThings",
                Id = instrument.Id.GetHashCode(),
                Name = $"Instrument {instrument.Name}",
                Description = instrument.Description
            };
            result.Properties.Add("kind", "Instrument");
            if (!string.IsNullOrWhiteSpace(instrument.Url))
            {
                result.Properties.Add("url", instrument.Url);
            }
            if (instrument.StartDate.HasValue)
            {
                result.Properties.Add("startDate", instrument.StartDate.Value.ToString("o"));
            }
            if (instrument.EndDate.HasValue)
            {
                result.Properties.Add("endDate", instrument.EndDate.Value.ToString("o"));
            }
            if (location == null)
                result.Location = LocationFromInstrument(instrument, uri, result);
            else
                result.Location = location;
            var historicalLocation = new HistoricalLocation { Thing = result, Time = DateTime.Now };
            historicalLocation.Locations.Add(result.Location);
            result.HistoricalLocations.Add(historicalLocation);
            return result;
        }

        public static Thing ThingFromSensor(e.Sensor sensor, Uri uri, Location location = null)
        {
            var result = new Thing
            {
                //BaseUrl = uri.GetLeftPart(UriPartial.Authority) + "/SensorThings",
                Id = sensor.Id.GetHashCode(),
                Name = $"Sensor {sensor.Name}",
                Description = sensor.Description
            };
            result.Properties.Add("kind", "Sensor");
            if (!string.IsNullOrWhiteSpace(sensor.Url))
            {
                result.Properties.Add("url", sensor.Url);
            }
            if (location == null)
                result.Location = LocationFromSensor(sensor, uri, result);
            else
                result.Location = location;
            var historicalLocation = new HistoricalLocation { Thing = result, Time = DateTime.Now };
            historicalLocation.Locations.Add(result.Location);
            result.HistoricalLocations.Add(historicalLocation);
            return result;
        }
        #endregion

        #region Locations
        public static Location LocationFromStation(e.Station station, Uri uri, Thing thing = null)
        {
            var result = new Location
            {
                //BaseUrl = uri.GetLeftPart(UriPartial.Authority) + "/SensorThings",
                Id = station.Id.GetHashCode(),
                Name = $"Station {station.Name}",
                Description = station.Description,
                Latitude = station.Latitude.Value,
                Longitude = station.Longitude.Value,
                Elevation = station.Elevation,
            };
            if (thing == null)
            {
                thing = ThingFromStation(station, uri, result);
            }
            result.Things.Add(thing);
            var historicalLocation = new HistoricalLocation { Thing = thing, Time = DateTime.Now };
            historicalLocation.Locations.Add(thing.Location);
            result.HistoricalLocations.Add(historicalLocation);
            return result;
        }

        public static Location LocationFromInstrument(e.Instrument instrument, Uri uri, Thing thing = null)
        {
            var result = new Location
            {
                //BaseUrl = uri.GetLeftPart(UriPartial.Authority) + "/SensorThings",
                Id = instrument.Id.GetHashCode(),
                Name = $"Instrument {instrument.Name}",
                Description = instrument.Description,
                Latitude = instrument.Latitude.Value,
                Longitude = instrument.Longitude.Value,
                Elevation = instrument.Elevation
            };
            if (thing == null)
            {
                thing = ThingFromInstrument(instrument, uri, result);
            }
            result.Things.Add(thing);
            var historicalLocation = new HistoricalLocation { Thing = thing, Time = DateTime.Now };
            historicalLocation.Locations.Add(thing.Location);
            result.HistoricalLocations.Add(historicalLocation);
            return result;
        }

        public static Location LocationFromSensor(e.Sensor sensor, Uri uri, Thing thing = null)
        {
            var result = new Location
            {
                //BaseUrl = uri.GetLeftPart(UriPartial.Authority) + "/SensorThings",
                Id = sensor.Id.GetHashCode(),
                Name = $"Sensor {sensor.Name}",
                Description = sensor.Description,
                Latitude = sensor.Latitude.Value,
                Longitude = sensor.Longitude.Value,
                Elevation = sensor.Elevation
            };
            if (thing == null)
            {
                thing = ThingFromSensor(sensor, uri, result);
            }
            result.Things.Add(thing);
            var historicalLocation = new HistoricalLocation { Thing = thing, Time = DateTime.Now };
            historicalLocation.Locations.Add(thing.Location);
            result.HistoricalLocations.Add(historicalLocation);
            return result;
        }
        #endregion
        */
    }
}