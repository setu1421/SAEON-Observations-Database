using SAEON.SensorThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
{
    [RoutePrefix("SensorThings/Datastreams")]
    public class DatastreamsSTController : BaseController<Datastream>
    {
        protected override List<Datastream> GetEntities()
        {
            var result = base.GetEntities();
            return result;
        }


    }
}
