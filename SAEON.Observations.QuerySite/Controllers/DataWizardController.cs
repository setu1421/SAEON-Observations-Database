using Newtonsoft.Json;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using SAEON.Observations.QuerySite.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class DataWizardController : BaseController<DataWizardModel>
    {
        protected override async Task<DataWizardModel> LoadModelAsync(DataWizardModel model)
        {
            model.Clear();
            model.Locations.AddRange(await GetListAsync<LocationNode>("Internal/Locations"));
            model.Features.AddRange(await GetListAsync<FeatureNode>("Internal/Features"));
            var mapPoints = model.Locations.Where(i => i.Key.StartsWith("STA~")).Select(
                loc => new MapPoint { Key = loc.Key, Title = loc.Name, Latitude = loc.Latitude.Value, Longitude = loc.Longitude.Value, Elevation = loc.Elevation, Url = loc.Url });
            model.MapPoints.AddRange(mapPoints);
            model.IsAuthenticated = User.Identity.IsAuthenticated;
            model.UserQueries.AddRange(await GetUserQueriesList());
            return model;
        }


        // GET: Data
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
        public async Task<ActionResult> Index()
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
        {
            ViewBag.WebAPIUrl = Properties.Settings.Default.WebAPIUrl;
            return View(await CreateModelAsync());
        }

        #region State
        [HttpGet]
        public JsonResult GetState()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    var model = SessionModel;
                    var result = new StateModel
                    {
                        IsAuthenticated = model.IsAuthenticated,
                        LoadEnabled = model.IsAuthenticated && model.UserQueries.Any(),
                        SaveEnabled = model.IsAuthenticated && model.LocationsSelected.Any() && model.FeaturesSelected.Any(),
                        SearchEnabled = model.LocationsSelected.Any() && model.FeaturesSelected.Any(),
                        DownloadEnabled = model.IsAuthenticated && model.LocationsSelected.Any() && model.FeaturesSelected.Any() && model.HaveSearched
                    };
                    Logging.Verbose("State: {@State}", result);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        #endregion

        #region Locations

        [HttpGet]
        public JsonResult GetLocations()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return GetListAsJson(SessionModel.Locations);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public JsonResult GetLocationsSelected()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return GetListAsJson(SessionModel.LocationsSelected);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public PartialViewResult GetLocationsHtml()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    ViewBag.WebAPIUrl = Properties.Settings.Default.WebAPIUrl;
                    return PartialView("_LocationsHtml", SessionModel);
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
                    ViewBag.WebAPIUrl = Properties.Settings.Default.WebAPIUrl;
                    return PartialView("_LocationsSelectedHtml", SessionModel);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

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

        #endregion Locations

        #region Features

        [HttpGet]
        public JsonResult GetFeatures()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return GetListAsJson(SessionModel.Features);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public JsonResult GetFeaturesSelected()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return GetListAsJson(SessionModel.FeaturesSelected);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public PartialViewResult GetFeaturesHtml()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    ViewBag.WebAPIUrl = Properties.Settings.Default.WebAPIUrl;
                    return PartialView("_FeaturesHtml", SessionModel);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public PartialViewResult GetFeaturesSelectedHtml()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    ViewBag.WebAPIUrl = Properties.Settings.Default.WebAPIUrl;
                    return PartialView("_FeaturesSelectedHtml", SessionModel);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

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

        #endregion Features

        #region Filters

        [HttpGet]
        public PartialViewResult GetFiltersHtml()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return PartialView("_FiltersHtml", SessionModel);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

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

        #endregion Filters

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

        #endregion Map

        #region Approximation

        [HttpGet]
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
        public async Task<PartialViewResult> SetApproximation()
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
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
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
        public async Task<JsonResult> GetApproximation()
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    var model = SessionModel;
                    var input = new DataWizardDataInput();
                    input.Organisations.AddRange(model.Organisations);
                    input.Sites.AddRange(model.Sites);
                    input.Stations.AddRange(model.Stations);
                    input.Phenomena.AddRange(model.Phenomena);
                    input.Offerings.AddRange(model.Offerings);
                    input.Units.AddRange(model.Units);
                    input.StartDate = model.StartDate.ToUniversalTime();
                    input.EndDate = model.EndDate.ToUniversalTime();
                    model.Approximation = await PostEntityAsync<DataWizardDataInput, DataWizardApproximation>("Internal/DataWizard/Approximation", input);
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

        #endregion Approximation

        #region Data

        [HttpGet]
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
        public async Task<EmptyResult> GetData()
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    var model = SessionModel;
                    var input = new DataWizardDataInput();
                    input.Organisations.AddRange(model.Organisations);
                    input.Sites.AddRange(model.Sites);
                    input.Stations.AddRange(model.Stations);
                    input.Phenomena.AddRange(model.Phenomena);
                    input.Offerings.AddRange(model.Offerings);
                    input.Units.AddRange(model.Units);
                    input.StartDate = model.StartDate.ToUniversalTime();
                    input.EndDate = model.EndDate.ToUniversalTime();
                    model.DataOutput = await PostEntityAsync<DataWizardDataInput, DataWizardDataOutput>("Internal/DataWizard/GetData", input);
                    model.HaveSearched = true;
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

        #endregion Data

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

        #endregion Table

        #region Chart

        [HttpGet]
        public PartialViewResult GetChartHtml()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    var sessionModel = SessionModel;
                    //Logging.Verbose("Model: {@model}", model);
                    return PartialView("_ChartHtml", sessionModel);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        #endregion Chart

        #region UserQueries

        //[Authorize]
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
        private async Task<List<UserQuery>> GetUserQueriesList()
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
        {
            if (User.Identity.IsAuthenticated)
            {
                return (await GetListAsync<UserQuery>("Internal/UserQueries"));
            }
            else
            {
                return new List<UserQuery>();
            }
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetUserQueries()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return GetListAsJson(SessionModel.UserQueries);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost]
        [Authorize]
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
        public async Task<EmptyResult> LoadQuery(LoadQueryModel input)
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    Logging.Verbose("Input: {@input}", input);
                    var model = SessionModel;
                    //Logging.Verbose("Model: {@model}", model);
                    await LoadModelAsync(model);
                    var userQuery = model.UserQueries.FirstOrDefault(i => i.Name == input.Name);
                    if (userQuery == null)
                    {
                        throw new HttpException((int)HttpStatusCode.NotFound, $"UserQuery not found {input?.Name}");
                    }

                    var wizardInput = JsonConvert.DeserializeObject<DataWizardDataInput>(userQuery.QueryInput);
                    // Locations
                    List<string> locations = new List<string>();
                    locations.AddRange(model.Locations.Where(i => wizardInput.Organisations.Contains(i.Id)).Select(i => i.Key));
                    locations.AddRange(model.Locations.Where(i => wizardInput.Sites.Contains(i.Id)).Select(i => i.Key));
                    locations.AddRange(model.Locations.Where(i => wizardInput.Stations.Contains(i.Id)).Select(i => i.Key));
                    UpdateLocationsSelected(locations);
                    // Features
                    List<string> features = new List<string>();
                    features.AddRange(model.Features.Where(i => wizardInput.Phenomena.Contains(i.Id)).Select(i => i.Key));
                    features.AddRange(model.Features.Where(i => wizardInput.Offerings.Contains(i.Id)).Select(i => i.Key));
                    features.AddRange(model.Features.Where(i => wizardInput.Units.Contains(i.Id)).Select(i => i.Key));
                    UpdateFeaturesSelected(features);
                    // Filters
                    model.StartDate = wizardInput.StartDate;
                    model.EndDate = wizardInput.EndDate;
                    SessionModel = model;
                    //Logging.Verbose("Model: {@model}", model);
                    return null;
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public PartialViewResult GetLoadQueryDialog()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return PartialView("_LoadQueryDialog", SessionModel);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        #endregion UserQueries

        #region SaveQueryDialog

        [HttpPost]
        [Authorize]
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
        public async Task<EmptyResult> SaveQuery(SaveQueryModel input)
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    Logging.Verbose("Input: {@Input}", input);
                    var model = SessionModel;
                    //Logging.Verbose("Model: {@model}", model);
                    var queryInput = new DataWizardDataInput();
                    queryInput.Organisations.AddRange(model.Organisations);
                    queryInput.Sites.AddRange(model.Sites);
                    queryInput.Stations.AddRange(model.Stations);
                    queryInput.Phenomena.AddRange(model.Phenomena);
                    queryInput.Offerings.AddRange(model.Offerings);
                    queryInput.Units.AddRange(model.Units);
                    queryInput.StartDate = model.StartDate.ToUniversalTime();
                    queryInput.EndDate = model.EndDate.ToUniversalTime();
                    var userQuery = new UserQuery
                    {
                        Name = input.Name,
                        Description = input.Description,
                        QueryInput = JsonConvert.SerializeObject(queryInput)
                    };
                    Logging.Verbose("UserQuery: {@UserQuery}", userQuery);
                    await PostEntityAsync<UserQuery, UserQuery>("Internal/UserQueries", userQuery);
                    model.UserQueries.Clear();
                    model.UserQueries.AddRange(await GetUserQueriesList());
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

        [HttpGet]
        public PartialViewResult GetSaveQueryDialog()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return PartialView("_SaveQueryDialog", SessionModel);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        #endregion SaveQueryDialog

        #region Download

        [HttpGet]
        public PartialViewResult GetDownloadDialog()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return PartialView("_DownloadDialog", SessionModel);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        [Authorize]
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
        public async Task<JsonResult> GetDownload()
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    var model = SessionModel;
                    //Logging.Verbose("Model: {@model}", model);
                    var input = new DataWizardDataInput();
                    input.Organisations.AddRange(model.Organisations);
                    input.Sites.AddRange(model.Sites);
                    input.Stations.AddRange(model.Stations);
                    input.Phenomena.AddRange(model.Phenomena);
                    input.Offerings.AddRange(model.Offerings);
                    input.Units.AddRange(model.Units);
                    input.StartDate = model.StartDate.ToUniversalTime();
                    input.EndDate = model.EndDate.ToUniversalTime();
                    var userDownload = await PostEntityAsync<DataWizardDataInput, UserDownload>("Internal/DataWizard/GetDownload", input);
                    Logging.Verbose("UserDownload: {@userDownload}", userDownload);
                    return Json(new { url = Url.Action("Details", "UserDownloads", new { userDownload.Id })}, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet, Authorize]
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
        public async Task<ActionResult> ViewDownload(Guid id)
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
        {
            var userDownload = await GetEntityAsync<UserDownload>($"Internal/UserDownloads/{id}");
            if (userDownload == null) throw new ArgumentNullException(nameof(id));
            return View(userDownload);
        }

#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
        public async Task<FilePathResult> DownloadZip(Guid id)
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
        {
            var userDownload = await GetEntityAsync<UserDownload>($"Internal/UserDownloads/{id}");
            if (userDownload == null) throw new ArgumentNullException(nameof(id));
            return new FilePathResult(userDownload.ZipFullName, MediaTypeNames.Application.Zip);
        }
        #endregion Download
    }
}