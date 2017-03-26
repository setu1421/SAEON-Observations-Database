using SAEON.Observations.Core;
using SAEON.Observations.QuerySite.Models;
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
        public QueryController()
        {
            using (Logging.MethodCall(this.GetType()))
            {
            }
        }

        protected override void Dispose(bool disposing)
        {
            using (Logging.MethodCall(this.GetType()))
            {
            }
            base.Dispose(disposing);
        }

        // GET: Query
        public async Task<ActionResult> Index()
        {
            //ViewBag.WebAPIUrl = Properties.Settings.Default.WebAPIUrl;
            ////if (CurrentSession["Locations"] == null)
            //{
            //    CurrentSession["Locations"] = GetLocations();
            //}
            //ViewBag.Locations = (List<Location>)CurrentSession["Locations"];
            //CurrentSession["SelectedLocations"] = new List<Location>();
            //ViewBag.SelectedLocations = (List<Location>)CurrentSession["SelectedLocations"];
            ////if (CurrentSession["Features"] == null)
            //{
            //    CurrentSession["Features"] = GetFeatures();
            //}
            //ViewBag.Features = (List<Feature>)CurrentSession["Features"];
            //CurrentSession["SelectedFeatures"] = new List<Feature>();
            //ViewBag.SelectedFeatures = (List<Feature>)CurrentSession["SelectedFeatures"];
            var model = new QueryModel { Locations = await GetLocations(), Features = await GetFeatures() };
            model.SelectedFeatures.Add(model.Features.First(i => i.Key.StartsWith("OFF~")));
            model.SelectedLocations.Add(model.Locations.First(i => i.Key.StartsWith("STA~")));
            return View("Wizard", model);
        }

        //public PartialViewResult StoreSelectedFeatures(List<string> features)
        //{
        //    using (Logging.MethodCall(this.GetType()))
        //    {
        //        try
        //        {
        //            Logging.Verbose("SelectedFeatures: {features}", features);
        //            var selectedFeatures = new List<Feature>();
        //            if (features != null)
        //            {
        //                var offerings = features.Where(l => l.StartsWith("|OFF|")).ToList();
        //                Logging.Verbose("SelectedOfferings: {offerings}", offerings);
        //                selectedFeatures = ((List<Feature>)CurrentSession["Features"])
        //                    .Where(l => offerings.Contains(l.Key))
        //                    .OrderBy(l => l.Text)
        //                    .ToList();
        //            }
        //            Logging.Verbose("SelectedFeatures: {@selectedFeatures}", selectedFeatures);
        //            CurrentSession["SelectedFeatures"] = selectedFeatures;
        //            return PartialView("SelectedFeaturesPost", selectedFeatures);
        //        }
        //        catch (Exception ex)
        //        {
        //            Logging.Exception(ex);
        //            throw;
        //        }
        //    }
        //}

        //public PartialViewResult StoreSelectedLocations(List<string> locations)
        //{
        //    using (Logging.MethodCall(this.GetType()))
        //    {
        //        try
        //        {
        //            Logging.Verbose("SelectedLocations: {locations}", locations);
        //            var selectedLocations = new List<Location>();
        //            if (locations != null)
        //            {
        //                var stations = locations.Where(l => l.StartsWith("|STA|")).ToList();
        //                Logging.Verbose("SelectedStations: {stations}", stations);
        //                selectedLocations = ((List<Location>)CurrentSession["Locations"])
        //                    .Where(l => stations.Contains(l.Key))
        //                    .OrderBy(l => l.Text)
        //                    .ToList();
        //            }
        //            Logging.Verbose("SelectedLocations: {@locations}", selectedLocations);
        //            CurrentSession["SelectedLocations"] = selectedLocations;
        //            return PartialView("SelectedLocationsPost", selectedLocations);
        //        }
        //        catch (Exception ex)
        //        {
        //            Logging.Exception(ex);
        //            throw;
        //        }
        //    }
        //}

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