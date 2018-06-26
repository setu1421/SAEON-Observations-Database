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

        public static Thing ThingFromStation(e.Station station, Uri uri)
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
            result.Location = LocationFromStation(station, uri);
            //result.HistoricalLocations.Add();
            return result;
        }

        public static Thing ThingFromInstrument(e.Instrument instrument, Uri uri)
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
            result.Location = LocationFromInstrument(instrument, uri);
            return result;
        }

        public static Thing ThingFromSensor(e.Sensor sensor, Uri uri)
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
            result.Location = LocationFromSensor(sensor, uri);
            return result;
        }
        #endregion

        #region Locations
        public static Location LocationFromStation(e.Station station, Uri uri)
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
            result.Things.Add(ThingFromStation(station, uri));
            return result;
        }

        public static Location LocationFromInstrument(e.Instrument instrument, Uri uri)
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
            result.Things.Add(ThingFromInstrument(instrument, uri));
            return result;
        }

        public static Location LocationFromSensor(e.Sensor sensor, Uri uri)
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
            result.Things.Add(ThingFromSensor(sensor, uri));
            return result;
        }
        #endregion

        /*
        #region HistoricalLocations
        public static HistoricalLocation HistoricalLocationFromStation(e.Station station, Uri uri)
        {
            return new HistoricalLocation
            {
                BaseUrl = uri.GetLeftPart(UriPartial.Authority) + "/SensorThings",
                Id = station.Id.GetHashCode(),
                Name = $"Station {station.Name}",
                Description = station.Description,
                Latitude = station.Latitude.Value,
                Longitude = station.Longitude.Value,
                Elevation = station.Elevation
            };
        }

        public static HistoricalLocation HistoricalLocationFromInstrument(e.Instrument instrument, Uri uri)
        {
            return new HistoricalLocation
            {
                BaseUrl = uri.GetLeftPart(UriPartial.Authority) + "/SensorThings",
                Id = instrument.Id.GetHashCode(),
                Name = $"Instrument {instrument.Name}",
                Description = instrument.Description,
                Latitude = instrument.Latitude.Value,
                Longitude = instrument.Longitude.Value,
                Elevation = instrument.Elevation
            };
        }

        public static HistoricalLocation HistoricalLocationFromSensor(e.Sensor sensor, Uri uri)
        {
            return new HistoricalLocation
            {
                BaseUrl = uri.GetLeftPart(UriPartial.Authority) + "/SensorThings",
                Id = sensor.Id.GetHashCode(),
                Name = $"Sensor {sensor.Name}",
                Description = sensor.Description,
                Latitude = sensor.Latitude.Value,
                Longitude = sensor.Longitude.Value,
                Elevation = sensor.Elevation
            };
        }
        #endregion
    */
    }
}