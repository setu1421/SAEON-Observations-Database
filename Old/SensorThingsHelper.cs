using SAEON.Logs;
using SAEON.Observations.Core.GeoJSON;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public static List<sos.Datastream> Datastreams = new List<sos.Datastream>();
        public static List<sos.Sensor> Sensors = new List<sos.Sensor>();
        public static List<sos.ObservedProperty> ObservedProperties = new List<sos.ObservedProperty>();
        public static List<sos.Observation> Observations = new List<sos.Observation>();
        public static List<sos.FeatureOfInterest> FeaturesOfInterest = new List<sos.FeatureOfInterest>();

        private static sos.Location AddLocation(Guid id, string name, string description, double latitude, double longitude, double? elevation)
        {
            var location = Locations.FirstOrDefault(i => (i.Name == name) && (i.Description == description) && (i?.Coordinate.Latitude == latitude) && (i?.Coordinate.Longitude == longitude) && (!(elevation.HasValue && (i?.Coordinate?.Elevation.HasValue ?? false) || (i?.Coordinate.Elevation == elevation))));
            if (location == null)
            {
                location = new sos.Location
                {
                    Id = id,
                    Name = name,
                    Description = description,
                    Coordinate = new GeoJSONCoordinate { Latitude = latitude, Longitude = longitude, Elevation = elevation }
                };
                location.GenerateSensorThingsProperties();
                Locations.Add(location);
            }
            return location;
        }

        /*
        private static sos.ObservedProperty AddObservedProperty(ef.vSensorThingsDatastream datastream)
        {
            var observedProperty = ObservedProperties.FirstOrDefault(i => i.Id == datastream.Id);
            if (observedProperty == null)
            {
                observedProperty = new sos.ObservedProperty { Id = datastream.Id, Name = $"{datastream.Phenomenon} {datastream.Offering}",
                    Description = $"{datastream.Phenomenon} {datastream.Offering} {datastream.Unit} {datastream.Symbol}",
                    Definition = string.IsNullOrEmpty(datastream.Url) ? "http://www.saeon.ac.za" : datastream.Url };
                observedProperty.GenerateSensorThingsProperties();
                ObservedProperties.Add(observedProperty);
            }
            return observedProperty;
        }
        */

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
                            Id = organisation.Id,
                            Name = organisation.Name,
                            Description = organisation.Description
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
                            Id = site.Id,
                            Name = site.Name,
                            Description = site.Description
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
                            Id = station.Id,
                            Name = station.Name,
                            Description = station.Description
                        };
                        thing.AddProperty("Kind", "Station");
                        thing.AddProperty("Url", station.Url);
                        thing.AddProperty("StartDate", station?.StartDate);
                        thing.AddProperty("EndDate", station?.EndDate);
                        if (station.Latitude.HasValue && station.Longitude.HasValue)
                        {
                            var location = AddLocation(station.Id, station.Name, station.Description, station.Latitude.Value, station.Longitude.Value, station.Elevation);
                            thing.Locations.Add(location);
                            location.Things.Add(thing);
                            var historicalLocation = new sos.HistoricalLocation { Id = station.Id, Time = station?.StartDate ?? DateTime.Now };
                            historicalLocation.GenerateSensorThingsProperties();
                            historicalLocation.Locations.Add(location);
                            historicalLocation.Thing = thing;
                            thing.HistoricalLocations.Add(historicalLocation);
                            var featureOfInterest = new sos.FeatureOfInterest { Id = station.Id, Name = station.Name, Description = station.Description ,
                                Coordinate = new GeoJSONCoordinate { Latitude = station.Latitude.Value, Longitude = station.Longitude.Value, Elevation = station.Elevation }
                            };
                            featureOfInterest.GenerateSensorThingsProperties();
                            FeaturesOfInterest.Add(featureOfInterest);
                        }
                        thing.GenerateSensorThingsProperties();
                        Things.Add(thing);
                    }
                    /*
                    foreach (var instrument in db.Instruments.Include(i => i.Sensors).OrderBy(i => i.Name))
                    {
                        var thing = new sos.Thing
                        {
                            Id = instrument.Id,
                            Name = instrument.Name,
                            Description = instrument.Description
                        };
                        thing.AddProperty("Kind", "Instrument");
                        thing.AddProperty("Url", instrument.Url);
                        thing.AddProperty("StartDate", instrument?.StartDate);
                        thing.AddProperty("EndDate", instrument?.EndDate);
                        if (instrument.Latitude.HasValue && instrument.Longitude.HasValue)
                        {
                            var location = AddLocation(instrument.Id, instrument.Name, instrument.Description, instrument.Latitude.Value, instrument.Longitude.Value, instrument.Elevation);
                            thing.Locations.Add(location);
                            location.Things.Add(thing);
                            var historicalLocation = new sos.HistoricalLocation { Id = instrument.Id, Time = instrument?.StartDate ?? DateTime.Now };
                            historicalLocation.GenerateSensorThingsProperties();
                            historicalLocation.Locations.Add(location);
                            historicalLocation.Thing = thing;
                            thing.HistoricalLocations.Add(historicalLocation);
                            var featureOfInterest = new sos.FeatureOfInterest
                            {
                                Id = instrument.Id,
                                Name = instrument.Name,
                                Description = instrument.Description,
                                Coordinate = new GeoJSONCoordinate { Latitude = instrument.Latitude.Value, Longitude = instrument.Longitude.Value, Elevation = instrument.Elevation }
                            };
                            featureOfInterest.GenerateSensorThingsProperties();
                            FeaturesOfInterest.Add(featureOfInterest);
                        }
                        thing.GenerateSensorThingsProperties();
                        Things.Add(thing);
                        foreach (var sensor in instrument.Sensors.OrderBy(i => i.Name))
                        {
                            var sosSensor = new sos.Sensor { Id = sensor.Id, Name = sensor.Name, Description = sensor.Description };
                            if (sensor.Url.EndsWith(".pdf", StringComparison.CurrentCultureIgnoreCase))
                            {
                                sosSensor.EncodingType = sos.ValueCodes.Pdf;
                                sosSensor.Metadata = sensor.Url;
                            }
                            sosSensor.GenerateSensorThingsProperties();
                            Sensors.Add(sosSensor);
                            //foreach (var Datastream in db.vSensorThingsDatastreams.Where(i => i.SensorId == sensor.Id).OrderBy(i => i.Phenomenon).ThenBy(i => i.Offering).ThenBy(i => i.Unit))
                            //{
                            //    var sosDatastream = new sos.Datastream
                            //    {
                            //        id = Datastream.SensorId,
                            //        name = $"{Datastream.Phenomenon} {Datastream.Offering} {Datastream.Unit}",
                            //        description = $"{Datastream.Phenomenon} {Datastream.Offering} {Datastream.Unit}",
                            //        unitOfMeasurement = new sos.UnitOfMeasurement { name = Datastream.Unit, symbol = Datastream.Symbol, definition="http://www.saeon.ac.za" },
                            //        observationType = sos.ValueCodes.OM_Measurement
                            //    };
                            //    sosDatastream.GenerateSensorThingsProperties();
                            //    Datastreams.Add(sosDatastream);
                            //    sosDatastream.Thing = thing;
                            //    thing.Datastreams.Add(sosDatastream);
                            //    sosDatastream.Sensor = sosSensor;
                            //    sosSensor.Datastreams.Add(sosDatastream);
                            //    var sosObservedProperty = AddObservedProperty(Datastream);
                            //    sosObservedProperty.Datastream = sosDatastream;
                            //    sosDatastream.ObservedProperty = sosObservedProperty;
                            //}
                        }
                    }
                */
                }
            }
        }
    }
}