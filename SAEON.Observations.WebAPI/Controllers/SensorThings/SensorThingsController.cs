using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SAEON.AspNet.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{
    [RoutePrefix("SensorThings")]
    public class SensorThingsController : Controller
    {
        public class EntitySet
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }

        public class EntitySetList
        {
            public readonly List<EntitySet> Value = new List<EntitySet>();
        }

        [HttpGet, Route]
        public JsonNetResult Index()
        {
            var List = new EntitySetList();
            List.Value.Add(new EntitySet { Name = "Things", Url = Request.Url.AbsoluteUri +"/Things" });
            List.Value.Add(new EntitySet { Name = "Locations", Url = Request.Url.AbsoluteUri + "/Locations" });
            List.Value.Add(new EntitySet { Name = "Datastreams", Url = Request.Url.AbsoluteUri + "/Datastreams" });
            List.Value.Add(new EntitySet { Name = "Sensors", Url = Request.Url.AbsoluteUri + "/Sensors" });
            List.Value.Add(new EntitySet { Name = "Observations", Url = Request.Url.AbsoluteUri + "/Observations" });
            List.Value.Add(new EntitySet { Name = "ObservedProperties", Url = Request.Url.AbsoluteUri + "/ObservedProperties" });
            List.Value.Add(new EntitySet { Name = "FeaturesOfInterest", Url = Request.Url.AbsoluteUri + "/FeaturesOfInterest" });
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                SerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver()},
                Data = List
            };
            return jsonNetResult;
        }
    }
}