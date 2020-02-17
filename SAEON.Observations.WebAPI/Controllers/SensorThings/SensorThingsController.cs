//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;
//using SAEON.AspNet.Mvc;
//using SAEON.Logs;
//using System.Collections.Generic;
//using System.Web.Mvc;

//namespace SAEON.Observations.WebAPI.Controllers.SensorThings
//{
//    [RoutePrefix("SensorThings")]
//    public class SensorThingsController : Controller
//    {
//        public class EntitySet
//        {
//            public string Name { get; set; }
//            public string Url { get; set; }
//        }

//        public class EntitySetList
//        {
//            public readonly List<EntitySet> Value = new List<EntitySet>();
//        }

//        [HttpGet, Route]
//        public JsonNetResult Index()
//        {
//            using (Logging.MethodCall(GetType()))
//            {
//                Logging.Information("Request: {@request}", Request.Url.Host);
//                var List = new EntitySetList();
//                List.Value.Add(new EntitySet { Name = "Datastreams", Url = Request.Url.Host + "/Datastreams" });
//                List.Value.Add(new EntitySet { Name = "FeaturesOfInterest", Url = Request.Url.AbsoluteUri + "/FeaturesOfInterest" });
//                List.Value.Add(new EntitySet { Name = "Locations", Url = Request.Url.AbsoluteUri + "/Locations" });
//                List.Value.Add(new EntitySet { Name = "HistoricalLocations", Url = Request.Url.AbsoluteUri + "/HistoricalLocations" });
//                List.Value.Add(new EntitySet { Name = "Observations", Url = Request.Url.AbsoluteUri + "/Observations" });
//                List.Value.Add(new EntitySet { Name = "ObservedProperties", Url = Request.Url.AbsoluteUri + "/ObservedProperties" });
//                List.Value.Add(new EntitySet { Name = "Sensors", Url = Request.Url.AbsoluteUri + "/Sensors" });
//                List.Value.Add(new EntitySet { Name = "Things", Url = Request.Url.AbsoluteUri + "/Things" });
//                JsonNetResult jsonNetResult = new JsonNetResult
//                {
//                    Formatting = Formatting.Indented,
//                    SerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), Formatting = Formatting.Indented },
//                    Data = List
//                };
//                return jsonNetResult;
//            }
//        }
//    }
//}
