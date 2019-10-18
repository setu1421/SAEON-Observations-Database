using SAEON.AspNet.WebApi;
using SAEON.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    [RoutePrefix("Internal/Test")]
    [TenantAuthorization]
    [Authorize]
    public class TestController : ApiController
    {
        [HttpGet]
        [Route]
        public IQueryable<string> GetAll()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return new List<string> { "One", "Two", "Three" }.AsQueryable();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }
    }
}
