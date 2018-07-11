using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers
{
    [RoutePrefix("Test")]
    public class TestController : ApiController
    {
        [HttpGet]
        [Route]
        public IQueryable<string> GetAll()
        {
            return new List<string> { "1", "2" }.AsQueryable();
        }
    }
}
