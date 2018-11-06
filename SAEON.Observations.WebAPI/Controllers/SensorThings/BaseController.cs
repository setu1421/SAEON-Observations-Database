using SAEON.Logs;
using SAEON.Observations.Core.Entities;
using SAEON.SensorThings;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public abstract class BaseController<TEntity> : SensorThingsApiController<TEntity> where TEntity : SensorThingEntity
    {
        protected ObservationsDbContext db = null;

        public BaseController()
        {
            using (Logging.MethodCall(GetType()))
            {
                db = new ObservationsDbContext();
            }
        }

        ~BaseController()
        {
            db = null;
        }

    }
}
