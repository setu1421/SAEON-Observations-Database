using SAEON.AspNet.WebApi;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers
{
    [RoutePrefix("Claims")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    [ClientAuthorization("SAEON.Observations.QuerySite")]
    public class ClaimsController : ApiController
    {
        [HttpGet]
        [Route]
        public IQueryable<string> GetAll()
        {
            var cp = (ClaimsPrincipal)User;
            return cp.Claims.Select(i => $"{i.Type} = {i.Value}").AsQueryable();
        }
    }
}
