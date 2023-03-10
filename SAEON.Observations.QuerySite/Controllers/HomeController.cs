using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.QuerySite.Models;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;

namespace SAEON.Observations.QuerySite.Controllers
{
#if ResponseCaching
    [OutputCache(Duration = Defaults.CacheDuration)]
#endif
    public class HomeController : BaseController<HomeModel>
    {
        protected override async Task<HomeModel> LoadModelAsync(HomeModel model)
        {
            if (model is null) throw new ArgumentNullException(nameof(model));
            model.Clear();
            var inventorySample = (await GetListAsync<InventorySnapshot>("Internal/InventorySnapshots")).First();
            model.Organisations = inventorySample.Organisations;
            model.Programmes = inventorySample.Programmes;
            model.Projects = inventorySample.Projects;
            model.Sites = inventorySample.Sites;
            model.Stations = inventorySample.Stations;
            model.Instruments = inventorySample.Instruments;
            model.Sensors = inventorySample.Sensors;
            model.Phenomena = inventorySample.Phenomena;
            model.Offerings = inventorySample.Offerings;
            model.UnitsOfMeasure = inventorySample.UnitsOfMeasure;
            model.Variables = inventorySample.Variables;
            model.Datasets = inventorySample.Datasets;
            model.Observations = inventorySample.Observations;
            var locationNodes = await GetListAsync<LocationTreeNode>("Internal/Locations");
            var mapPoints = locationNodes.Where(i => i.Key.StartsWith("STA~")).Select(
                loc => new MapPoint { Key = loc.Key, Title = loc.Name, Latitude = loc.Latitude.Value, Longitude = loc.Longitude.Value, Elevation = loc.Elevation, Url = loc.Url });
            model.MapPoints.AddRange(mapPoints);
            return model;
        }

        [OutputCache(Duration = 0, Location = OutputCacheLocation.None, NoStore = true)]
        public async Task<ActionResult> Index()
        {
            var model = await CreateModelAsync();
            ViewBag.Authorization = await GetAuthorizationAsync();
            ViewBag.Tenant = Tenant;
            ViewBag.WebAPIUrl = ConfigurationManager.AppSettings["WebAPIUrl"];
            SessionModel = model;
            return View(model);
        }

        [HttpGet]
        public JsonResult GetMapPoints()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    //SAEONLogs.Verbose("MapPoints: {@MapPoints}", SessionModel.MapPoints);
                    return GetListAsJson(SessionModel.MapPoints);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [Route("About")]
        public ActionResult About()
        {
            return View();
        }

        [Route("Contact")]
        public ActionResult Contact()
        {
            return View();
        }

        [Route("ConditionsOfUse")]
        public ActionResult ConditionsOfUse()
        {
            return View();
        }

        [Route("DataUsage")]
        public ActionResult DataUsage()
        {
            return View();
        }

        [Route("Disclaimer")]
        public ActionResult Disclaimer()
        {
            return View();
        }

        [Route("HowToCite")]
        public ActionResult HowToCite()
        {
            return View();
        }

        [Route("Privacy")]
        public ActionResult Privacy()
        {
            return View();
        }

        [Route("SetTenant/{Name}")]
        [OutputCache(Duration = 0, Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult SetTenant(string name)
        {
            using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "Name", name } }))
            {
                SAEONLogs.Verbose("Tenant: {Tenant}", name);
                Session[Constants.SessionKeyTenant] = name;
                return RedirectToAction("Index", "Home");
            }
        }

        [Route("Test")]
        public ActionResult Test()
        {
            return View(new DataWizardDataInput());
        }
    }
}