using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
/*
 * using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{
    public static class SensorThingsConfig
    {
        public static string BaseUrl { get; set; }
    }

    public abstract class SensorThingEntity
    {
        public string EntitySetName { get; protected set; }

        public Guid Id { get; set; }
        public string SelfLink { get { return $"{SensorThingsConfig.BaseUrl}/{EntitySetName}({Id})"; } set {; } }
        public List<string> NavigationLinks { get; } = new List<string>();

        [JsonIgnore]
        public virtual JObject AsJSON
        {
            get
            {
                var result = new JObject
                {
                    new JProperty("@iot.id", Id),
                    new JProperty("@iot.selfLink", SelfLink)
                };
                foreach (var link in NavigationLinks)
                {
                    result.Add($"{link}@iot.navigationLink", $"{SensorThingsConfig.BaseUrl}/{EntitySetName}({Id})/{link}");
                }
                return result;
            }
        }
    }

    public abstract class NamedSensorThingEntity : SensorThingEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        public override JObject AsJSON
        {
            get
            {
                var result = base.AsJSON;
                result.Add(new JProperty("name", Name));
                result.Add(new JProperty("description", Description));
                return result;
            }
        }
    }

    public class Thing : NamedSensorThingEntity
    {
        public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();
        //public Location Location { get; set; } = null;
        //public List<HistoricalLocation> HistoricalLocations { get; } = new List<HistoricalLocation>();
        //public List<Datastream> Datastreams { get; } = new List<Datastream>();

        public Thing() : base()
        {
            EntitySetName = "Things";
            NavigationLinks.Add("Location");
            NavigationLinks.Add("HistoricalLocations");
            NavigationLinks.Add("Datastreams");
        }

        public override JObject AsJSON
        {
            get
            {
                var result = base.AsJSON;
                if (Properties.Any())
                {
                    var properties = new JObject();
                    foreach (var property in Properties)
                    {
                        properties.Add(new JProperty(property.Key, property.Value));
                    }
                    result.Add(new JProperty("properties", properties));
                }
                return result;
            }
        }
    }


}
*/