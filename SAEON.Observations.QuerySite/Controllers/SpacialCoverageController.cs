using SAEON.Observations.Core;
using SAEON.Observations.QuerySite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    [ResourceAuthorize("Observations.Admin", "SpacialCoverage")]
    public class SpacialCoverageController : BaseWebApiController
    {
        private SpacialCoverageModel SessionModel
        {
            get
            {
                return GetSessionModel<SpacialCoverageModel>();
            }
            set
            {
                SetSessionModel<SpacialCoverageModel>(value);
            }
        }

        private async Task<SpacialCoverageModel> CreateSessionModel()
        {
            var sessionModel = new SpacialCoverageModel()
            {
                Locations = await GetLocationsList(),
                Features = await GetFeaturesList(),
            };
            return sessionModel;
        }

        // GET: Query
        public async Task<ActionResult> Index()
        {
            using (Logging.MethodCall(GetType()))
            {
                var sessionModel = await CreateSessionModel();
                SessionModel = sessionModel;
                //Logging.Verbose("Model: {@model}", model);
                return View(sessionModel);
            }
        }

        #region Locations
        private async Task<List<Location>> GetLocationsList()
        {
            return (await GetList<Location>("Locations")).ToList();
        }

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
        public PartialViewResult GetLocationsHtml()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return PartialView("LocationsHtml", SessionModel);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public PartialViewResult GetSelectedLocationsHtml()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return PartialView("SelectedLocationsHtml", SessionModel);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost]
        public PartialViewResult UpdateSelectedLocations(List<string> locations)
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    Logging.Verbose("SelectedLocations: {locations}", locations);
                    var sessionModel = SessionModel;
                    var selectedLocations = new List<Location>();
                    if (locations != null)
                    {
                        // Set IsCheck for all
                        foreach (var location in sessionModel.Locations.Where(i => locations.Contains(i.Key)))
                        {
                            location.IsChecked = true;
                        }
                        // Only select stations
                        var stations = locations.Where(l => l.StartsWith("STA~")).ToList();
                        Logging.Verbose("SelectedStations: {stations}", stations);
                        selectedLocations = sessionModel.Locations.Where(l => stations.Contains(l.Key)).OrderBy(l => l.Name).ToList();
                    }
                    Logging.Verbose("SelectedLocations: {@locations}", selectedLocations);
                    sessionModel.SelectedLocations.Clear();
                    sessionModel.SelectedLocations.AddRange(selectedLocations);
                    SessionModel = sessionModel;
                    //Logging.Verbose("Model: {@model}", model);
                    return GetSelectedLocationsHtml();
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
        private async Task<List<Feature>> GetFeaturesList()
        {
            return (await GetList<Feature>("Features")).ToList();
        }

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
        public PartialViewResult GetFeaturesHtml()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return PartialView("FeaturesHtml", SessionModel);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        public PartialViewResult GetSelectedFeaturesHtml()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return PartialView("SelectedFeaturesHtml", SessionModel);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost]
        public PartialViewResult UpdateSelectedFeatures(List<string> features)
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    Logging.Verbose("SelectedFeatures: {features}", features);
                    var sessionModel = SessionModel;
                    var selectedFeatures = new List<Feature>();
                    if (features != null)
                    {
                        // Set IsCheck for all
                        foreach (var feature in sessionModel.Features.Where(i => features.Contains(i.Key)))
                        {
                            feature.IsChecked = true;
                        }
                        // Only select offerings
                        var offerings = features.Where(l => l.StartsWith("OFF~")).ToList();
                        Logging.Verbose("SelectedOfferings: {offerings}", offerings);
                        selectedFeatures = sessionModel.Features.Where(l => offerings.Contains(l.Key)).OrderBy(l => l.Text).ToList();
                    }
                    Logging.Verbose("SelectedFeatures: {@features}", selectedFeatures);
                    sessionModel.SelectedFeatures.Clear();
                    sessionModel.SelectedFeatures.AddRange(selectedFeatures);
                    SessionModel = sessionModel;
                    //Logging.Verbose("Model: {@model}", model);
                    return GetSelectedFeaturesHtml();
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
        [HttpGet]
        public PartialViewResult GetFiltersHtml()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return PartialView("FiltersHtml", SessionModel);
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
                    Logging.Verbose("StartDate: {startDate}", startDate);
                    var sessionModel = SessionModel;
                    sessionModel.StartDate = startDate;
                    sessionModel.EndDate = endDate;
                    SessionModel = sessionModel;
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
        #endregion

        #region Data
        [HttpGet]
        public async Task<EmptyResult> GetData()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    var sessionModel = SessionModel;
                    //Logging.Verbose("Model: {@model}", model);
                    var input = new SpacialCoverageInput
                    {
                        Stations = sessionModel.SelectedLocations.Select(i => i.Id).ToList(),
                        PhenomenaOfferings = sessionModel.SelectedFeatures.Select(i => i.Id).ToList(),
                        StartDate = new DateTime(sessionModel.StartDate.Year, sessionModel.StartDate.Month, 1),
                        EndDate = new DateTime(sessionModel.EndDate.Year, sessionModel.EndDate.Month, 1).AddMonths(1).AddDays(-1)
                    };
                    Logging.Verbose("Input: {@input}", input);
                    var results = (await Post<SpacialCoverageInput, SpacialCoverageOutput>("SpacialCoverage", input));
                    //Logging.Verbose("Results: {@results}", results);
                    sessionModel.Results = results;
                    SessionModel = sessionModel;
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
        #endregion

        #region Map
        [HttpGet]
        public PartialViewResult GetStationsMap()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return PartialView("StationsMap");
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
