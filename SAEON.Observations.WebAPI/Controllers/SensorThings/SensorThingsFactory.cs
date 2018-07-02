using e = SAEON.Observations.Core.Entities;
using SAEON.SensorThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
{
    public static class SensorThingsFactory
    {
        #region Things

        public static Thing ThingFromStation(e.Station station, Uri uri, Location location = null)
        {
            var result = new Thing
            {
                BaseUrl = uri.GetLeftPart(UriPartial.Authority) + "/SensorThings",
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
                BaseUrl = uri.GetLeftPart(UriPartial.Authority) + "/SensorThings",
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
                BaseUrl = uri.GetLeftPart(UriPartial.Authority) + "/SensorThings",
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
                BaseUrl = uri.GetLeftPart(UriPartial.Authority) + "/SensorThings",
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
                BaseUrl = uri.GetLeftPart(UriPartial.Authority) + "/SensorThings",
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
                BaseUrl = uri.GetLeftPart(UriPartial.Authority) + "/SensorThings",
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

    }
}