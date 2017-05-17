using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    [RoutePrefix("Claims")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    public class ClaimsController : ApiController
    {
        [HttpGet]
        [Route]
        public IQueryable<string> GetAll()
        {
            var cp = (ClaimsPrincipal)User;
            return cp.Claims.Select(i => $"{i.Subject} = {i.Value}").AsQueryable();
        }

    }
}
