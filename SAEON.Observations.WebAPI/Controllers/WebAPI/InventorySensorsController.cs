using SAEON.Observations.Core.Entities;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    /// <summary>
    /// </summary>
    [RoutePrefix("Api/InventorySensors")]
    public class InventorySensorsController : BaseEntityController<InventorySensor>
    {
        /// <summary>
        /// Inventory of Sensors
        /// </summary>
        /// <returns>ListOf(InventorySensor)</returns>
        [ResponseType(typeof(InventorySensor))]
        public override IQueryable<InventorySensor> GetAll()
        {
            return base.GetAll();
        }

    }
}
