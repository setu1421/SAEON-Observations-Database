using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAEON.Core;
using SAEON.Observations.Auth;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class QueryController : BaseController
    {

        public IActionResult Datasets()
        {
            ViewData["Tenant"] = Tenant;
            ViewData["Authorization"] = "bearer " + HttpContext.Session.GetString(ODPAuthenticationDefaults.SessionAccessToken);
            ViewData["WebAPIUrl"] = Config["WebAPIUrl"].AddTrailingForwardSlash() + "Internal/InventoryDatasets";
            return View();
        }
    }
}
