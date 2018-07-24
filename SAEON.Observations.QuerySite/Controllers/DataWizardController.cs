using SAEON.Observations.QuerySite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class DataWizardController : BaseController<DataWizardModel>
    {
        //protected override async Task<DataModel> LoadModelAsync(DataModel model)
        //{
        //    return model;
        //}

        protected override DataWizardModel LoadModel(DataWizardModel model)
        {
            return model;
        }

        // GET: Data
        public ActionResult Index()
        {
            ViewBag.WebAPIUrl = Properties.Settings.Default.WebAPIUrl;
            return View(CreateModel());
        }

        #region Locations
        #endregion
    }
}