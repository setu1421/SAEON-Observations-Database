using SAEON.AspNet.WebApi;
using SAEON.Observations.Core;
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
            var result = new List<string>
            {
                $"UserId: {User.GetUserId()}",
                $"UserName: {User.GetUserName()}"
            };
            var cp = (ClaimsPrincipal)User;
            result.AddRange(cp.Claims.Select(i => $"{i.Type} = {i.Value}").AsQueryable());
            return result.AsQueryable();
        }
    }
}
