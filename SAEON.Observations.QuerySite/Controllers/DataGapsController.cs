using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    [ResourceAuthorize("Observations.Admin", "DataGaps")]
    public class DataGapsController : BaseWebApiController
    {
        // GET: DataGaps
        public ActionResult Index()
        {
            return View();
        }
    }
}