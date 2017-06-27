using SAEON.Logs;
using SAEON.Observations.Core.Entities;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    [Authorize]
    public class BaseController : ApiController
    {
        protected ObservationsDbContext db = null;

        public BaseController() : base()
        {
            using (Logging.MethodCall(GetType()))
            {
                db = new ObservationsDbContext();
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Database.CommandTimeout = 30 * 60;
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