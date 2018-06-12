using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class BaseController : ApiController
    {
        protected readonly ObservationsDbContext db = null;

        public BaseController() : base()
        {
            using (Logging.MethodCall(GetType()))
            {
                db = new ObservationsDbContext();
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