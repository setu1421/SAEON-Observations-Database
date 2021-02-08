using Newtonsoft.Json;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.QuerySite.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{

    [RoutePrefix("Query/Data")]
    public class DataWizardController : BaseController<DataWizardModel>
    {
        protected override async Task<DataWizardModel> LoadModelAsync(DataWizardModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            model.Clear();
            model.LocationNodes.AddRange(await GetListAsync<LocationNode>("Internal/Locations"));
            model.VariableNodes.AddRange(await GetListAsync<VariableNode>("Internal/Variables"));
            var mapPoints = model.LocationNodes.Where(i => i.Key.StartsWith("STA~")).Select(
                loc => new MapPoint { Key = loc.Key, Title = loc.Name, Latitude = loc.Latitude.Value, Longitude = loc.Longitude.Value, Elevation = loc.Elevation, Url = loc.Url });
            model.MapPoints.AddRange(mapPoints);
            model.IsAuthenticated = User.Identity.IsAuthenticated;
            model.UserQueries.AddRange(await GetUserQueriesList());
            return model;
        }


        // GET: Data
        [HttpGet]
        [Route]
        public async Task<ActionResult> Index()
        {
            ViewBag.Authorization = await GetAuthorizationAsync();
            ViewBag.Tenant = Tenant;
            ViewBag.WebAPIUrl = ConfigurationManager.AppSettings["WebAPIUrl"];
            return View(await CreateModelAsync());
        }

        #region State
        [HttpGet]
        public JsonResult GetState()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var model = SessionModel;
                    var result = new StateModel
                    {
                        IsAuthenticated = model.IsAuthenticated,
                        LoadEnabled = model.IsAuthenticated && model.UserQueries.Any(),
                        SaveEnabled = model.IsAuthenticated && model.LocationNodesSelected.Any() && model.VariableNodesSelected.Any(),
                        SearchEnabled = model.LocationNodesSelected.Any() && model.VariableNodesSelected.Any() && (model.Approximation.RowCount > 0),
                        DownloadEnabled = model.IsAuthenticated && model.LocationNodesSelected.Any() && model.VariableNodesSelected.Any() && model.HaveSearched
                    };
                    SAEONLogs.Verbose("State: {@State}", result);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        #endregion

        #region Locations

        [HttpGet]
        public JsonResult GetLocations()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return GetListAsJson(SessionModel.LocationNodes);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public JsonResult GetLocationsSelected()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return GetListAsJson(SessionModel.LocationNodesSelected);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public PartialViewResult GetLocationsHtml()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    ViewBag.WebAPIUrl = ConfigurationManager.AppSettings["WebAPIUrl"];
                    return PartialView("_LocationsHtml", SessionModel);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public PartialViewResult GetLocationsSelectedHtml()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    ViewBag.WebAPIUrl = ConfigurationManager.AppSettings["WebAPIUrl"];
                    return PartialView("_LocationsSelectedHtml", SessionModel);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost]
        public PartialViewResult UpdateLocationsSelected(List<string> locations)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    SAEONLogs.Verbose("Locations: {Locations}", locations);
                    var model = SessionModel;
                    // Uncheck all
                    model.LocationNodes.ForEach(i => i.IsChecked = false);
                    model.MapPoints.ForEach(i => i.IsSelected = false);
                    // Clear
                    model.LocationNodesSelected.Clear();
                    model.Locations.Clear();
                    if (locations != null)
                    {
                        // Set IsChecked for selected
                        foreach (var location in model.LocationNodes.Where(i => locations.Contains(i.Key)))
                        {
                            location.IsChecked = true;
                        }
                        // Organisations
                        var organisationKeys = locations.Where(l => l.StartsWith("ORG~")).Distinct().ToList();
                        var organisations = model.LocationNodes.Where(l => organisationKeys.Contains(l.Key)).OrderBy(l => l.Name);
                        model.LocationNodesSelected.AddRange(organisations);
                        foreach (var organisationKey in organisationKeys)
                        {
                            model.LocationNodes.Where(i => i.Key.StartsWith("STA~") && i.Key.Contains(organisationKey))
                                .Join(model.MapPoints, l => l.Key, m => m.Key, (l, m) => m)
                                .ToList()
                                .ForEach(i => i.IsSelected = true);
                        }
                        // Programmes
                        var programmeKeys = locations.Where(l => l.StartsWith("PRG~")).Distinct().ToList();
                        var programmmes = model.LocationNodes.Where(l => programmeKeys.Contains(l.Key)).OrderBy(l => l.Name);
                        model.LocationNodesSelected.AddRange(programmmes);
                        foreach (var programmeKey in programmeKeys)
                        {
                            model.LocationNodes.Where(i => i.Key.StartsWith("STA~") && i.Key.Contains(programmeKey))
                                .Join(model.MapPoints, l => l.Key, m => m.Key, (l, m) => m)
                                .ToList()
                                .ForEach(i => i.IsSelected = true);
                        }
                        // Projects
                        var projectKeys = locations.Where(l => l.StartsWith("PRJ~")).Distinct().ToList();
                        var projects = model.LocationNodes.Where(l => projectKeys.Contains(l.Key)).OrderBy(l => l.Name);
                        model.LocationNodesSelected.AddRange(projects);
                        foreach (var projectKey in projectKeys)
                        {
                            model.LocationNodes.Where(i => i.Key.StartsWith("STA~") && i.Key.Contains(projectKey))
                                .Join(model.MapPoints, l => l.Key, m => m.Key, (l, m) => m)
                                .ToList()
                                .ForEach(i => i.IsSelected = true);
                        }
                        // Sites
                        var siteKeys = locations.Where(l => l.StartsWith("SIT~")).Distinct().ToList();
                        var sites = model.LocationNodes.Where(l => siteKeys.Contains(l.Key)).OrderBy(l => l.Name);
                        model.LocationNodesSelected.AddRange(sites);
                        foreach (var siteKey in siteKeys)
                        {
                            model.LocationNodes.Where(i => i.Key.StartsWith("STA~") && i.Key.Contains(siteKey))
                                .Join(model.MapPoints, l => l.Key, m => m.Key, (l, m) => m)
                                .ToList()
                                .ForEach(i => i.IsSelected = true);
                        }
                        // Stations
                        var stationKeys = locations.Where(l => l.StartsWith("STA~")).Distinct().ToList();
                        var stations = model.LocationNodes.Where(l => stationKeys.Contains(l.Key)).OrderBy(l => l.Name);
                        model.LocationNodesSelected.AddRange(stations);
                        foreach (var stationKey in stationKeys)
                        {
                            model.LocationNodes.Where(i => i.Key.StartsWith("STA~") && i.Key.Contains(stationKey))
                                .Join(model.MapPoints, l => l.Key, m => m.Key, (l, m) => m)
                                .ToList()
                                .ForEach(i => i.IsSelected = true);

                            var splits = stationKey.Split(new char[] { '|' });
                            var stationId = splits[0].Split(new char[] { '~' })[1];
                            var location = new Location
                            {
                                StationId = new Guid(stationId)
                            };
                            model.Locations.Add(location);
                        }
                    }
                    SessionModel = model;
                    SAEONLogs.Verbose("LocationNodesSelected: {@LocationNodesSelected}", model.LocationNodesSelected);
                    SAEONLogs.Verbose("Locations: {@Locations}", model.Locations);
                    SAEONLogs.Verbose("MapPoints: {@MapPoints}", model.MapPoints);
                    return PartialView("_LocationsSelectedHtml", model);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        #endregion Locations

        #region Variables

        [HttpGet]
        public JsonResult GetVariables()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return GetListAsJson(SessionModel.VariableNodes);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public JsonResult GetVariablesSelected()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return GetListAsJson(SessionModel.VariableNodesSelected);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public PartialViewResult GetVariablesHtml()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    ViewBag.WebAPIUrl = ConfigurationManager.AppSettings["WebAPIUrl"];
                    return PartialView("_VariablesHtml", SessionModel);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public PartialViewResult GetVariablesSelectedHtml()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    ViewBag.WebAPIUrl = ConfigurationManager.AppSettings["WebAPIUrl"];
                    return PartialView("_VariablesSelectedHtml", SessionModel);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost]
        public PartialViewResult UpdateVariablesSelected(List<string> variables)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    SAEONLogs.Verbose("Variables: {Variables}", variables);
                    var model = SessionModel;
                    // Uncheck all
                    model.VariableNodes.ForEach(i => i.IsChecked = false);
                    // Clear
                    model.VariableNodesSelected.Clear();
                    model.Variables.Clear();
                    if (variables != null)
                    {
                        // Set IsChecked for selected
                        foreach (var variable in model.VariableNodes.Where(i => variables.Contains(i.Key)))
                        {
                            variable.IsChecked = true;
                        }
                        // Phenomena
                        var phenomenaKeys = variables.Where(l => l.StartsWith("PHE~")).ToList();
                        var phenomena = model.VariableNodes.Where(l => phenomenaKeys.Contains(l.Key)).OrderBy(l => l.Name);
                        model.VariableNodesSelected.AddRange(phenomena);
                        //model.Phenomena.AddRange(phenomena.Select(i => i.Id));
                        // Offerings
                        var offeringKeys = variables.Where(l => l.StartsWith("OFF~")).ToList();
                        var offerings = model.VariableNodes.Where(l => offeringKeys.Contains(l.Key)).OrderBy(l => l.Name);
                        model.VariableNodesSelected.AddRange(offerings);
                        //model.Offerings.AddRange(offerings.Select(i => i.Id));
                        // Units
                        var unitKeys = variables.Where(l => l.StartsWith("UNI~")).ToList();
                        var units = model.VariableNodes.Where(l => unitKeys.Contains(l.Key)).OrderBy(l => l.Name);
                        model.VariableNodesSelected.AddRange(units);
                        foreach (var unitKey in unitKeys)
                        {
                            var splits = unitKey.Split(new char[] { '|' });
                            var unitId = splits[0].Split(new char[] { '~' })[1];
                            var offeringId = splits[1].Split(new char[] { '~' })[1];
                            var phenomenonId = splits[2].Split(new char[] { '~' })[1];
                            var variable = new Variable
                            {
                                PhenomenonId = new Guid(phenomenonId),
                                OfferingId = new Guid(offeringId),
                                UnitId = new Guid(unitId)
                            };
                            model.Variables.Add(variable);
                        }
                    }
                    SessionModel = model;
                    SAEONLogs.Verbose("VariableNodesSelected: {@VariableNodesSelected}", model.VariableNodesSelected);
                    SAEONLogs.Verbose("Variables: {@VariablesSelected}", model.Variables);
                    return PartialView("_VariablesSelectedHtml", model);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        #endregion Variables

        #region Filters

        [HttpGet]
        public PartialViewResult GetFiltersHtml()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return PartialView("_FiltersHtml", SessionModel);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost]
        public EmptyResult UpdateFilters(DateTime startDate, DateTime endDate, float elevationMinimum, float elevationMaximum)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    SAEONLogs.Verbose("StartDate: {startDate} EndDate: {endDate} ElevationMin: {elevationMin} ElevationMax: {elevationMax}", startDate, endDate, elevationMinimum, elevationMaximum);
                    var model = SessionModel;
                    model.StartDate = startDate;
                    model.EndDate = endDate;
                    model.ElevationMinimum = elevationMinimum;
                    model.ElevationMaximum = elevationMaximum;
                    SessionModel = model;
                    return null;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        #endregion Filters

        #region Map

        [HttpGet]
        public JsonResult GetMapPoints()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return GetListAsJson(SessionModel.MapPoints);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        #endregion Map

        #region Approximation

        [HttpGet]
        public async Task<PartialViewResult> SetApproximation()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    await GetApproximation();
                    return PartialView("_ApproximationHtml", SessionModel);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetApproximation()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var model = SessionModel;
                    SAEONLogs.Verbose("Model: {@Model}", model);
                    var input = new DataWizardDataInput();
                    input.Locations.AddRange(model.Locations);
                    input.Variables.AddRange(model.Variables);
                    input.StartDate = model.StartDate.ToUniversalTime();
                    input.EndDate = model.EndDate.ToUniversalTime();
                    input.ElevationMinimum = model.ElevationMinimum;
                    input.ElevationMaximum = model.ElevationMaximum;
                    model.Approximation = await PostEntityAsync<DataWizardDataInput, DataWizardApproximation>("Internal/DataWizard/Approximation", input);
                    SAEONLogs.Verbose("RowCount: {RowCount} Errors: {Errors}", model.Approximation.RowCount, model.Approximation.Errors);
                    SessionModel = model;
                    return Json(SessionModel.Approximation, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        #endregion Approximation

        #region Data

        [HttpGet]
        public async Task<EmptyResult> GetData()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var model = SessionModel;
                    var input = new DataWizardDataInput();
                    input.Locations.AddRange(model.Locations);
                    input.Variables.AddRange(model.Variables);
                    input.StartDate = model.StartDate.ToUniversalTime();
                    input.EndDate = model.EndDate.ToUniversalTime();
                    input.ElevationMinimum = model.ElevationMinimum;
                    input.ElevationMaximum = model.ElevationMaximum;
                    model.DataOutput = await PostEntityAsync<DataWizardDataInput, DataWizardDataOutput>("Internal/DataWizard/GetData", input);
                    model.HaveSearched = true;
                    SessionModel = model;
                    return new EmptyResult();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        #endregion Data

        #region Table

        [HttpGet]
        public PartialViewResult GetTableHtml()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var sessionModel = SessionModel;
                    //SAEONLogs.Verbose("Model: {@model}", model);
                    return PartialView("_TableHtml", sessionModel);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        #endregion Table

        #region Chart

        [HttpGet]
        public PartialViewResult GetChartHtml()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var sessionModel = SessionModel;
                    //SAEONLogs.Verbose("Model: {@model}", model);
                    return PartialView("_ChartHtml", sessionModel);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        #endregion Chart

        #region UserQueries

        //[Authorize]
        private async Task<List<UserQuery>> GetUserQueriesList()
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
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return GetListAsJson(SessionModel.UserQueries);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<EmptyResult> LoadQuery(LoadQueryModel input)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    SAEONLogs.Verbose("Input: {@input}", input);
                    var model = SessionModel;
                    //SAEONLogs.Verbose("Model: {@model}", model);
                    await LoadModelAsync(model);
                    var userQuery = model.UserQueries.FirstOrDefault(i => i.Name == input.Name);
                    if (userQuery == null)
                    {
                        throw new HttpException((int)HttpStatusCode.NotFound, $"UserQuery not found {input?.Name}");
                    }

                    var wizardInput = JsonConvert.DeserializeObject<DataWizardDataInput>(userQuery.QueryInput);
                    // Locations
                    List<string> locations = new List<string>();
                    //locations.AddRange(model.LocationNodes.Where(i => wizardInput.Organisations.Contains(i.Id)).Select(i => i.Key));
                    //locations.AddRange(model.LocationNodes.Where(i => wizardInput.Sites.Contains(i.Id)).Select(i => i.Key));
                    //locations.AddRange(model.LocationNodes.Where(i => wizardInput.Stations.Contains(i.Id)).Select(i => i.Key));
                    UpdateLocationsSelected(locations);
                    // Variables
                    List<string> variables = new List<string>();
                    //variables.AddRange(model.VariableNodes.Where(i => wizardInput.Phenomena.Contains(i.Id)).Select(i => i.Key));
                    //variables.AddRange(model.VariableNodes.Where(i => wizardInput.Offerings.Contains(i.Id)).Select(i => i.Key));
                    //variables.AddRange(model.VariableNodes.Where(i => wizardInput.Units.Contains(i.Id)).Select(i => i.Key));
                    UpdateVariablesSelected(variables);
                    // Filters
                    model.StartDate = wizardInput.StartDate;
                    model.EndDate = wizardInput.EndDate;
                    model.ElevationMinimum = wizardInput.ElevationMinimum;
                    model.ElevationMaximum = wizardInput.ElevationMaximum;
                    SessionModel = model;
                    //SAEONLogs.Verbose("Model: {@model}", model);
                    return null;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public PartialViewResult GetLoadQueryDialog()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return PartialView("_LoadQueryDialog", SessionModel);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        #endregion UserQueries

        #region SaveQueryDialog

        [HttpPost]
        [Authorize]
        public async Task<EmptyResult> SaveQuery(SaveQueryModel input)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    SAEONLogs.Verbose("Input: {@Input}", input);
                    var model = SessionModel;
                    //SAEONLogs.Verbose("Model: {@model}", model);
                    var queryInput = new DataWizardDataInput();
                    queryInput.Locations.AddRange(model.Locations);
                    queryInput.Variables.AddRange(model.Variables);
                    queryInput.StartDate = model.StartDate.ToUniversalTime();
                    queryInput.EndDate = model.EndDate.ToUniversalTime();
                    queryInput.ElevationMinimum = model.ElevationMinimum;
                    queryInput.ElevationMaximum = model.ElevationMaximum;
                    var userQuery = new UserQuery
                    {
                        Name = input.Name,
                        Description = input.Description,
                        QueryInput = JsonConvert.SerializeObject(queryInput)
                    };
                    SAEONLogs.Verbose("UserQuery: {@UserQuery}", userQuery);
                    await PostEntityAsync<UserQuery, UserQuery>("Internal/UserQueries", userQuery);
                    model.UserQueries.Clear();
                    model.UserQueries.AddRange(await GetUserQueriesList());
                    SessionModel = model;
                    return null;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public PartialViewResult GetSaveQueryDialog()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return PartialView("_SaveQueryDialog", SessionModel);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        #endregion SaveQueryDialog

        #region Download

        [HttpGet]
        public PartialViewResult GetDownloadDialog()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return PartialView("_DownloadDialog", SessionModel);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<JsonResult> GetDownload()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var model = SessionModel;
                    //SAEONLogs.Verbose("Model: {@model}", model);
                    var input = new DataWizardDataInput();
                    input.Locations.AddRange(model.Locations);
                    input.Variables.AddRange(model.Variables);
                    input.StartDate = model.StartDate.ToUniversalTime();
                    input.EndDate = model.EndDate.ToUniversalTime();
                    input.ElevationMinimum = model.ElevationMinimum;
                    input.ElevationMaximum = model.ElevationMaximum;
                    var userDownload = await PostEntityAsync<DataWizardDataInput, UserDownload>("Internal/DataWizard/GetDownload", input);
                    SAEONLogs.Verbose("UserDownload: {@userDownload}", userDownload);
                    return Json(new { url = Url.Action("ViewDownload", new { userDownload.Id }) }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> ViewDownload(Guid? id)
        {
            using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "Id", id } }))
            {
                try
                {
                    SAEONLogs.Verbose("Id: {Id}", id);
                    if ((id == null) || !id.HasValue)
                    {
                        return RedirectToAction("Index");
                    }

                    var userDownload = await GetEntityAsync<UserDownload>($"Internal/UserDownloads/{id}");
                    if (userDownload == null)
                    {
                        throw new ArgumentException($"Unable to find download {id}", nameof(id));
                    }

                    return View(userDownload);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public async Task<FileResult> DownloadZip(Guid? id)
        {
            using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "Id", id } }))
            {
                try
                {
                    if ((id == null) || !id.HasValue)
                    {
                        RedirectToAction("Index");
                        return null;
                    }

                    var userDownload = await GetEntityAsync<UserDownload>($"Internal/UserDownloads/{id}");
                    if (userDownload == null)
                    {
                        throw new ArgumentException($"Unable to find download {id}", nameof(id));
                    }

                    var stream = await GetStreamAsync($"Internal/DataWizard/DownloadZip/{id}");
                    using (var memStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memStream);
                        var bytes = memStream.ToArray();
                        return File(bytes, MediaTypeNames.Application.Zip, Path.GetFileName(userDownload.ZipFullName));
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        #endregion Download
    }
}