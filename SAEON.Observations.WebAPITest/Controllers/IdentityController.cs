using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SAEON.Observations.WebAPITest.Controllers
{
    [Route("identity")]
    [Authorize]
    [EnableCors(origins: "http://localhost:62751/", headers: "*", methods: "*")]
    public class IdentityController : ApiController
    {
        public IHttpActionResult Get()
        {
            Log.Information("IdentityController.Get");
            var user = User as ClaimsPrincipal;
            var claims = from c in user.Claims
                         select new
                         {
                             type = c.Type,
                             value = c.Value
                         };

            return Json(claims);
        }
    }
}
