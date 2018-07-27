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
            model.Locations.AddRange(await GetList<Location>("Internal/Locations"));
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
                    var sessionModel = SessionModel;
                    //Logging.Verbose("SessionModel.Locations: {@sessionModelLocations}", sessionModel.Locations);
                    Logging.Verbose("SessionModel.LocationsSelected: {@sessionModelLocationsSelected}", sessionModel.LocationsSelected);
                    Logging.Verbose("Locations: {locations}", locations);
                    sessionModel.LocationsSelected.Clear();
                    var selectedLocations = new List<Location>();
                    if (locations != null)
                    {
                        // Clear IsChecked for all
                        sessionModel.Locations.ForEach(i => i.IsChecked = false);
                        // Set IsCheck for selected
                        foreach (var location in sessionModel.Locations.Where(i => locations.Contains(i.Key)))
                        {
                            location.IsChecked = true;
                        }
                        // Organisations
                        var organisations = locations.Where(l => l.StartsWith("ORG~")).ToList();
                        Logging.Verbose("SelectedOrganisations: {organisations}", organisations);
                        selectedLocations.AddRange(sessionModel.Locations.Where(l => organisations.Contains(l.Key)).OrderBy(l => l.Name).ToList());
                        // Sites
                        var sites = locations.Where(l => l.StartsWith("SIT~")).ToList();
                        Logging.Verbose("SelectedSites: {sites}", sites);
                        selectedLocations.AddRange(sessionModel.Locations.Where(l => sites.Contains(l.Key)).OrderBy(l => l.Name).ToList());
                        // States
                        var stations = locations.Where(l => l.StartsWith("STA~")).ToList();
                        Logging.Verbose("SelectedStations: {stations}", stations);
                        selectedLocations.AddRange(sessionModel.Locations.Where(l => stations.Contains(l.Key)).OrderBy(l => l.Name).ToList());
                    }
                    Logging.Verbose("SelectedLocations: {@locations}", selectedLocations);
                    sessionModel.LocationsSelected.AddRange(selectedLocations);
                    LoadMapPoints(sessionModel);
                    SessionModel = sessionModel;
                    Logging.Verbose("SessionModel.LocationsSelected: {@sessionModelLocationsSelected}", sessionModel.LocationsSelected);
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
        #endregion
    }
}