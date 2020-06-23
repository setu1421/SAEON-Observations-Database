using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.Core.Entities;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    /// <summary>
    /// Instruments
    /// </summary>
    [ODataRoutePrefix("InventorySensors")]
    public class InventorySensorsODController : BaseController<InventorySensor>
    {

        // GET: odata/InventorySensors
        /// <summary>
        /// Inventory of Sensors
        /// </summary>
        /// <returns>ListOf(InventorySensor)</returns>
        [ODataRoute]
        public override IQueryable<InventorySensor> GetAll()
        {
            return base.GetAll();
        }
    }
}
