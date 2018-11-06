using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class InventoryController : BaseController
    {
        // GET: Inventory
        public ActionResult Index()
        {
            ViewBag.WebAPIUrl = Properties.Settings.Default.WebAPIUrl;
            return View();
        }

        //public async Task<ContentResult> GetInventory()
        //{
        //    return await GetODataList<Inventory>("Internal/Inventory" + Request.QueryString.ToString());
        //}

    }
}