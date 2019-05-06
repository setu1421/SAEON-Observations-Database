using SAEON.AspNet.WebApi;
using SAEON.Logs;
using SAEON.Observations.Core.Entities;
using SAEON.SensorThings;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [TenantAuthorization]
    public abstract class BaseController<TEntity> : SensorThingsApiController<TEntity> where TEntity : SensorThingEntity
    {
        private ObservationsDbContext dbContext = null;
        protected ObservationsDbContext DbContext
        {
            get
            {
                if (dbContext == null) dbContext = new ObservationsDbContext(TenantAuthorizationAttribute.GetTenantFromHeaders(Request));
                return dbContext;
            }
            private set => dbContext = value;
        }

        ~BaseController()
        {
            DbContext = null;
        }

    }
}
