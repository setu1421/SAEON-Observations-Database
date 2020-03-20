using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.Core.Entities;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    /// <summary>
    /// Instruments
    /// </summary>
    [ODataRoutePrefix("Inventory")]
    public class InventoryODController : BaseController<Inventory>
    {

        // GET: odata/Instruments
        /// <summary>
        /// Get all Instruments
        /// </summary>
        /// <returns>ListOf(Instrument)</returns>
        [ODataRoute]
        public override IQueryable<Inventory> GetAll()
        {
            return base.GetAll();
        }
    }
}
