using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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

        public async Task<IEnumerable<Inventory>> GetInventory()
        {
            return await GetList<Inventory>("Internal/Inventory/"+Request.QueryString);
        }

    }
}