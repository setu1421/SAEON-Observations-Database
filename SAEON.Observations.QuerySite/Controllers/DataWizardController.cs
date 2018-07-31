using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.QuerySite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class DataWizardController : BaseController<DataWizardModel>
    {
        //protected override async Task<DataModel> LoadModelAsync(DataModel model)
        //{
        //    return model;
        //}

        protected override async Task<DataWizardModel> LoadModelAsync(DataWizardModel model)
        {
            model.Locations.Clear();
            model.Locations.AddRange(await GetList<LocationNode>("Internal/Locations"));
            model.MapPoints.Clear();
            model.MapPoints.AddRange(model.Locations.Where(i => i.Key.StartsWith("STA~")).Select(
                loc => new MapPoint { Key = loc.Key, Title = loc.Name, Latitude = loc.Latitude.Value, Longitude = loc.Longitude.Value, Elevation = loc.Elevation, Url = loc.Url }));
            return model;
        }

        // GET: Data
        public async Task<ActionResult> Index()
        {
            ViewBag.WebAPIUrl = Properties.Settings.Default.WebAPIUrl;
            return View(await CreateModelAsync());
        }

        #region Locations
        [HttpPost]
        public PartialViewResult UpdateLocationsSelected(List<string> locations)
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    var model = SessionModel;
                    Logging.Verbose("Locations: {locations}", locations);
                    // Clear all
                    model.Locations.ForEach(i => i.IsChecked = false);
                    model.MapPoints.ForEach(i => i.IsSelected = false);
                    // Clear 
                    model.LocationsSelected.Clear();
                    model.Organisations.Clear();
                    model.Sites.Clear();
                    model.Stations.Clear();
                    if (locations != null)
                    {
                        var mapPoints = new List<MapPoint>();
                        // Set IsCheck for selected
                        foreach (var location in model.Locations.Where(i => locations.Contains(i.Key)))
                        {
                            location.IsChecked = true;
                        }
                        // Organisations
                        var orgKeys = locations.Where(l => l.StartsWith("ORG~")).ToList();
                        var orgs = model.Locations.Where(l => orgKeys.Contains(l.Key)).OrderBy(l => l.Name);
                        model.LocationsSelected.AddRange(orgs);
                        model.Organisations.AddRange(orgs.Select(i => i.Id));
                        foreach (var orgKey in orgKeys)
                        {
                            foreach (var loc in model.Locations.Where(i => i.Key.StartsWith("STA~") && i.Key.Contains(orgKey)))
                            {
                                model.MapPoints.First(i => i.Key == loc.Key).IsSelected = true;
                            }
                        }
                        // Sites
                        var siteKeys = locations.Where(l => l.StartsWith("SIT~")).ToList();
                        var sites = model.Locations.Where(l => siteKeys.Contains(l.Key)).OrderBy(l => l.Name);
                        model.LocationsSelected.AddRange(sites);
                        model.Sites.AddRange(sites.Select(i => i.Id));
                        foreach (var siteKey in siteKeys)
                        {
                            foreach (var loc in model.Locations.Where(i => i.Key.StartsWith("STA~") && i.Key.Contains(siteKey)))
                            {
                                model.MapPoints.First(i => i.Key == loc.Key).IsSelected = true;
                            }
                        }
                        // Stations
                        var stationKeys = locations.Where(l => l.StartsWith("STA~")).ToList();
                        var stations = model.Locations.Where(l => stationKeys.Contains(l.Key)).OrderBy(l => l.Name);
                        model.LocationsSelected.AddRange(stations);
                        model.Stations.AddRange(stations.Select(i => i.Id));
                        foreach (var stationKey in stationKeys)
                        {
                            foreach (var loc in model.Locations.Where(i => i.Key.StartsWith("STA~") && i.Key.Contains(stationKey)))
                            {
                                model.MapPoints.First(i => i.Key == loc.Key).IsSelected = true;
                            }
                        }
                    }
                    SessionModel = model;
                    Logging.Verbose("LocationsSelected: {@LocationsSelected}", model.LocationsSelected);
                    Logging.Verbose("Organisations: {Organisations}", model.Organisations);
                    Logging.Verbose("Sites: {Sites}", model.Sites);
                    Logging.Verbose("Stations: {Stations}", model.Stations);
                    Logging.Verbose("MapPoints: {@MapPoints}", model.MapPoints);
                    //Logging.Verbose("Model: {@model}", model);
                    return GetLocationsSelectedHtml();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public PartialViewResult GetLocationsSelectedHtml()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return PartialView("_LocationsSelectedHtml", SessionModel);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        #endregion

        #region Map
        private void LoadMapPoints(DataWizardModel sessionModel)
        {

        }

        [HttpGet]
        public JsonResult GetMapPoints()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return GetListAsJson(SessionModel.MapPoints);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        #endregion
    }
}