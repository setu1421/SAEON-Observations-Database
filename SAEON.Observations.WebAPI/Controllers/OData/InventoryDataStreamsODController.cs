using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.Core.Entities;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    /// <summary>
    /// Instruments
    /// </summary>
    [ODataRoutePrefix("InventoryDataStreams")]
    public class InventoryDataStreamsODController : BaseController<InventoryDataStream>
    {

        // GET: odata/InventoryDataStreams
        /// <summary>
        /// Inventory of DataStreams
        /// </summary>
        /// <returns>ListOf(InventoryDataStream)</returns>
        [ODataRoute]
        public override IQueryable<InventoryDataStream> GetAll()
        {
            return base.GetAll();
        }
    }
}
