using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAEON.Logs;
using SAEON.Observations.Core.Entities;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public abstract class BaseController : Controller
    {
        protected ObservationsDbContext db = null;

        public BaseController(ObservationsDbContext context) : base()
        {
            using (Logging.MethodCall(GetType()))
            {
                db = context;
                db.ChangeTracker.AutoDetectChangesEnabled = false;
                db.Database.SetCommandTimeout(30 * 60);
            }
        }

        protected override void Dispose(bool disposing)
        {
            using (Logging.MethodCall(GetType()))
            {
                if (disposing)
                {
                    if (db != null)
                    {
                        db.Dispose();
                    }
                }
                base.Dispose(disposing);
            }
        }

    }
}
