using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.QuerySite.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class DataWizardController : BaseController<DataWizardModel>
    {
        protected override async Task<DataWizardModel> LoadModelAsync(DataWizardModel model)
        {
            model.Locations.Clear();
            model.Locations.AddRange(await GetList<LocationNode>("Internal/Locations"));
            model.Organisations.Clear();
            model.Sites.Clear();
            model.Stations.Clear();
            model.Features.Clear();
            model.Features.AddRange(await GetList<FeatureNode>("Internal/Features"));
            model.Phenomena.Clear();
            model.Offerings.Clear();
            model.Units.Clear();
            model.MapPoints.Clear();
            var mapPoints = model.Locations.Where(i => i.Key.StartsWith("STA~")).Select(
                loc => new MapPoint { Key = loc.Key, Title = loc.Name, Latitude = loc.Latitude.Value, Longitude = loc.Longitude.Value, Elevation = loc.Elevation, Url = loc.Url });
            model.MapPoints.AddRange(mapPoints);
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
                    Logging.Verbose("Locations: {Locations}", locations);
                    var model = SessionModel;
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
                            model.Locations.Where(i => i.Key.StartsWith("STA~") && i.Key.Contains(orgKey))
                                .Join(model.MapPoints, l => l.Key, m => m.Key, (l, m) => m)
                                .ToList()
                                .ForEach(i => i.IsSelected = true);
                        }
                        // Sites
                        var siteKeys = locations.Where(l => l.StartsWith("SIT~")).ToList();
                        var sites = model.Locations.Where(l => siteKeys.Contains(l.Key)).OrderBy(l => l.Name);
                        model.LocationsSelected.AddRange(sites);
                        model.Sites.AddRange(sites.Select(i => i.Id));
                        foreach (var siteKey in siteKeys)
                        {
                            model.Locations.Where(i => i.Key.StartsWith("STA~") && i.Key.Contains(siteKey))
                                .Join(model.MapPoints, l => l.Key, m => m.Key, (l, m) => m)
                                .ToList()
                                .ForEach(i => i.IsSelected = true);
                        }
                        // Stations
                        var stationKeys = locations.Where(l => l.StartsWith("STA~")).ToList();
                        var stations = model.Locations.Where(l => stationKeys.Contains(l.Key)).OrderBy(l => l.Name);
                        model.LocationsSelected.AddRange(stations);
                        model.Stations.AddRange(stations.Select(i => i.Id));
                        foreach (var stationKey in stationKeys)
                        {
                            model.Locations.Where(i => i.Key.StartsWith("STA~") && i.Key.Contains(stationKey))
                                .Join(model.MapPoints, l => l.Key, m => m.Key, (l, m) => m)
                                .ToList()
                                .ForEach(i => i.IsSelected = true);
                        }
                    } 
                    SessionModel = model;
                    Logging.Verbose("LocationsSelected: {@LocationsSelected}", model.LocationsSelected);
                    Logging.Verbose("Organisations: {Organisations}", model.Organisations);
                    Logging.Verbose("Sites: {Sites}", model.Sites);
                    Logging.Verbose("Stations: {Stations}", model.Stations);
                    Logging.Verbose("MapPoints: {@MapPoints}", model.MapPoints);
                    //return GetLocationsSelectedHtml();
                    return PartialView("_LocationsSelectedHtml", model);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }
        #endregion

        #region Features
        [HttpPost]
        public PartialViewResult UpdateFeaturesSelected(List<string> features)
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    Logging.Verbose("Features: {Features}", features);
                    var model = SessionModel;
                    // Clear all
                    model.Features.ForEach(i => i.IsChecked = false);
                    // Clear 
                    model.FeaturesSelected.Clear();
                    model.Phenomena.Clear();
                    model.Offerings.Clear();
                    model.Units.Clear();
                    if (features != null)
                    {
                        // Set IsCheck for selected
                        foreach (var feature in model.Features.Where(i => features.Contains(i.Key)))
                        {
                            feature.IsChecked = true;
                        }
                        // Phenomena
                        var phenomenaKeys = features.Where(l => l.StartsWith("PHE~")).ToList();
                        var phenomena = model.Features.Where(l => phenomenaKeys.Contains(l.Key)).OrderBy(l => l.Name);
                        model.FeaturesSelected.AddRange(phenomena);
                        model.Phenomena.AddRange(phenomena.Select(i => i.Id));
                        // Offerings
                        var offeringKeys = features.Where(l => l.StartsWith("OFF~")).ToList();
                        var offerings = model.Features.Where(l => offeringKeys.Contains(l.Key)).OrderBy(l => l.Name);
                        model.FeaturesSelected.AddRange(offerings);
                        model.Offerings.AddRange(offerings.Select(i => i.Id));
                        // Units
                        var unitKeys = features.Where(l => l.StartsWith("UNI~")).ToList();
                        var units = model.Features.Where(l => unitKeys.Contains(l.Key)).OrderBy(l => l.Name);
                        model.FeaturesSelected.AddRange(units);
                        model.Units.AddRange(units.Select(i => i.Id));
                    }
                    SessionModel = model;
                    Logging.Verbose("FeaturesSelected: {@FeaturesSelected}", model.FeaturesSelected);
                    Logging.Verbose("Phenomena: {Phenomena}", model.Phenomena);
                    Logging.Verbose("Offerings: {Offerings}", model.Offerings);
                    Logging.Verbose("Units: {Units}", model.Units);
                    return PartialView("_FeaturesSelectedHtml", model);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }
        #endregion

        #region Filters
        [HttpPost]
        public EmptyResult UpdateFilters(DateTime startDate, DateTime endDate)
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    Logging.Verbose("StartDate: {startDate} EndDate: {endDate}", startDate, endDate);
                    var model = SessionModel;
                    model.StartDate = startDate;
                    model.EndDate = endDate;
                    SessionModel = model;
                    return null;
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

        #region Approximation
        [HttpGet]
        public async Task<PartialViewResult> SetApproximation()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    await GetApproximation();
                    return PartialView("_ApproximationHtml", SessionModel);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetApproximation()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    var model = SessionModel;
                    var input = new DataWizardInput();
                    input.Organisations.AddRange(model.Organisations);
                    input.Sites.AddRange(model.Sites);
                    input.Stations.AddRange(model.Stations); 
                    input.Phenomena.AddRange(model.Phenomena);
                    input.Offerings.AddRange(model.Offerings);
                    input.Units.AddRange(model.Units);
                    input.StartDate = model.StartDate.ToUniversalTime();
                    input.EndDate = model.EndDate.ToUniversalTime();
                    model.Approximation = await PostEntity<DataWizardInput, DataWizardApproximation>("Internal/DataWizard/Approximation", input);
                    Logging.Verbose("RowCount: {RowCount}", model.Approximation.RowCount);
                    Logging.Verbose("Errors: {Errors}", model.Approximation.Errors);
                    SessionModel = model;
                    return Json(SessionModel.Approximation, JsonRequestBehavior.AllowGet); 
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        #endregion

        #region Data
        [HttpGet]
        public async Task<EmptyResult> GetData()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    var model = SessionModel;
                    var input = new DataWizardInput();
                    input.Organisations.AddRange(model.Organisations);
                    input.Sites.AddRange(model.Sites);
                    input.Stations.AddRange(model.Stations);
                    input.Phenomena.AddRange(model.Phenomena);
                    input.Offerings.AddRange(model.Offerings);
                    input.Units.AddRange(model.Units);
                    input.StartDate = model.StartDate.ToUniversalTime();
                    input.EndDate = model.EndDate.ToUniversalTime();
                    model.Output = await PostEntity<DataWizardInput, DataWizardOutput>("Internal/DataWizard/Execute", input);
                    SessionModel = model;
                    return new EmptyResult();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }
        #endregion

        #region Table
        [HttpGet]
        public PartialViewResult GetTableHtml()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    var sessionModel = SessionModel;
                    //Logging.Verbose("Model: {@model}", model);
                    return PartialView("_TableHtml", sessionModel);
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