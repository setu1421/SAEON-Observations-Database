using SAEON.AspNet.WebApi;
using SAEON.Observations.Core.Entities;
using SAEON.Observations.SensorThings;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ODataRouteName("SensorThings")]
    [TenantAuthorization]
    public abstract class BaseController<TSensorThingsEntity, TDbEntity> : SensorThingsController<TSensorThingsEntity, TDbEntity> where TSensorThingsEntity : SensorThingsEntity, new() where TDbEntity : BaseIDEntity
    {
    }
}
