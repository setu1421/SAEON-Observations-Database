using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.Core.SensorThings
{
    public static class ValueCodes
    {
        public static readonly string GeoJson = "application/vnd.geo+json";
    }

    public class BaseSensorThingEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Url]
        public string SelfLink { get; set; }
        [Url]
        public string NavigationLink { get; set; }
    }

    public class Thing : BaseSensorThingEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public Dictionary<string,string> Properties { get; set; }
    }

    public class Location : BaseSensorThingEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string EncodingType { get; set; }
        [Required]
        public string LocationType { get; set; }
    }

}
