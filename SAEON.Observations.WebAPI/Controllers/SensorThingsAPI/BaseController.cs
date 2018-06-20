using SAEON.Logs;
using SAEON.Observations.Core.Entities;
using SAEON.SensorThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BaseController<TEntity> : SensorThingsApiController<TEntity> where TEntity : SensorThingEntity
    {
        protected readonly ObservationsDbContext db = null;

        public BaseController()
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
