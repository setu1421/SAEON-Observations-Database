using Microsoft.AspNet.OData;
using SAEON.AspNet.WebApi;
using SAEON.Observations.Core.Entities;
using SAEON.Observations.SensorThings;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ODataRouteName("SensorThings")]
    [TenantAuthorization]
    public abstract class BaseGuidIdController<TSensorThingsEntity, TDbEntity> : SensorThingsGuidIdController<TSensorThingsEntity, TDbEntity> where TSensorThingsEntity : SensorThingsGuidIdEntity, new() where TDbEntity : GuidIdEntity
    {
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [ODataRouteName("SensorThings")]
    [TenantAuthorization]
    public abstract class BaseIntIdController<TSensorThingsEntity, TDbEntity> : SensorThingsIntIdController<TSensorThingsEntity, TDbEntity> where TSensorThingsEntity : SensorThingsIntIdEntity, new() where TDbEntity : IntIdEntity
    {
    }
}
