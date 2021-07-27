using SAEON.Logs;
using System;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{

    [RoutePrefix("Dataset/10.15493")]
    public class DatasetController : BaseController
    {
        [Route("{id}")]
        public ActionResult Index(string id)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var doi = "10.15493/" + id;
                    return RedirectToAction("Dataset", "DataWizard");
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
