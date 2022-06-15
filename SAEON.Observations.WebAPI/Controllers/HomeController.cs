using Microsoft.AspNetCore.Mvc;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
#if ResponseCaching
    [ResponseCache(Duration = Defaults.CacheDuration)]
#endif
    public class HomeController : BaseMvcController
    {
        [Route("About")]
        public IActionResult About()
        {
            return View();
        }

        [Route("ConditionsOfUse")]
        public IActionResult ConditionsOfUse()
        {
            return View();
        }

        [Route("Contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [Route("Disclaimer")]
        public IActionResult Disclaimer()
        {
            return View();
        }

        [Route("GraphQL")]
        public IActionResult GraphQL()
        {
            return View();
        }

        [Route("HowToCite")]
        public IActionResult HowToCite()
        {
            return View();
        }

        public IActionResult Index()
        {
            var model = DbContext.InventorySnapshots.OrderByDescending(i => i.When).First();
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("Privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        [ResponseCache(Duration = Defaults.ApiCacheDuration)]
        public JsonResult GetMapPoints()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    //SAEONLogs.Verbose("MapPoints: {@MapPoints}", SessionModel.MapPoints);
                    var locationNodes = new List<LocationTreeNode>();
                    var mapPoints = new List<MapPoint>();
                    var stationId = new Guid();
                    foreach (var location in DbContext.VLocations.OrderBy(i => i.OrganisationName).ThenBy(i => i.ProgrammeName).ThenBy(i => i.ProjectName).ThenBy(i => i.SiteName).ThenBy(i => i.StationName))
                    {
                        if (location.StationID != stationId)
                        {
                            stationId = location.StationID;
                            mapPoints.Add(new MapPoint
                            {
                                Title = location.StationName,
                                Latitude = location.Latitude.Value,
                                Longitude = location.Longitude.Value,
                                Elevation = location.Elevation,
                                Url = location.StationUrl
                            });
                        }
                    }
                    return new JsonResult(mapPoints);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }
    }
}
