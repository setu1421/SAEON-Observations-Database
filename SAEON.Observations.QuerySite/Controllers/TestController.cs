using RestSharp;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    [RoutePrefix("Test"), Route("{action=index}")]
    public class TestController : BaseRestController<Site>
    {
        public TestController()
        {
            Resource = "Sites";
        }

        public override Task<ActionResult> Index()
        {
            return base.Index();
        }


    }
}