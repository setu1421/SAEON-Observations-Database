using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SAEON.AspNet.Mvc;
using SAEON.Core;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
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