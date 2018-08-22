using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.Core.Entities;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    [ODataRoutePrefix("Inventory")]
    public class InventoryController : BaseODataController<Inventory>
    {
        [EnableQuery, ODataRoute]
        public override IQueryable<Inventory> GetAll()
        {
            return base.GetAll();
        }
    }
}
