using SAEON.Observations.Core;
using Syncfusion.JavaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    [Authorize]
    public class QueryController : BaseWebApiController
    {

        // GET: Query
        public ActionResult Index()
        {
            ViewBag.WebAPIUrl = Properties.Settings.Default.WebAPIUrl;
            if (System.Web.HttpContext.Current.Session["Locations"] == null)
            {
                System.Web.HttpContext.Current.Session["Locations"] = new List<Location>();
            }
            ViewBag.Locations = (List<Location>)System.Web.HttpContext.Current.Session["Locations"];
            if (System.Web.HttpContext.Current.Session["Features"] == null)
            {
                System.Web.HttpContext.Current.Session["Features"] = new List<Feature>();
            }
            ViewBag.Features = (List<Feature>)System.Web.HttpContext.Current.Session["Features"];
            return View();
        }

        public async Task<PartialViewResult> StoreSelectedFeatures(List<string> selectedFeatures)
        {
            using (Logging.MethodCall(this.GetType()))
            {
                try
                {
                    Logging.Verbose("SelectedFeatures: {Features}", selectedFeatures);
                    var features = new List<Feature>();
                    if (selectedFeatures != null)
                    {
                        selectedFeatures = selectedFeatures.Where(l => l.StartsWith("|OFF|")).ToList();
                        Logging.Verbose("SelectedOfferings: {Features}", selectedFeatures);
                        features = (await GetFeatures()).Where(l => selectedFeatures.Contains(l.Key)).OrderBy(l => l.Text).ToList();
                    }
                    Logging.Verbose("Features: {@Features}", features);
                    TempData["Features"] = features;
                    System.Web.HttpContext.Current.Session["Features"] = features;
                    return PartialView("SelectedFeaturesPost", features);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        public async Task<PartialViewResult> StoreSelectedLocations(List<string> selectedLocations)
        {
            using (Logging.MethodCall(this.GetType()))
            {
                try
                {
                    Logging.Verbose("SelectedLocations: {locations}", selectedLocations);
                    var locations = new List<Location>();
                    if (selectedLocations != null)
                    {
                        selectedLocations = selectedLocations.Where(l => l.StartsWith("|STA|")).ToList();
                        Logging.Verbose("SelectedStations: {locations}", selectedLocations);
                        locations = (await GetLocations()).Where(l => selectedLocations.Contains(l.Key)).OrderBy(l => l.Text).ToList();
                    }
                    Logging.Verbose("Locations: {@locations}", locations);
                    TempData["Locations"] = locations;
                    System.Web.HttpContext.Current.Session["Locations"] = locations;
                    return PartialView("SelectedLocationsPost", locations);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        protected async Task<List<Feature>> GetFeatures()
        {
            return (await GetList<Feature>("Features")).ToList();
        }

        protected async Task<List<Location>> GetLocations()
        {
            return (await GetList<Location>("Locations")).ToList();
        }

    }
}