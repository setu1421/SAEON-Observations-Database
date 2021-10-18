using SAEON.Observations.Core;
using System.Collections.Generic;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    public class InventorySnapshotsController : InternalListController<InventorySnapshot>
    {
        protected override List<InventorySnapshot> GetList()
        {
            var result = base.GetList();
            result.AddRange(DbContext.InventorySnapshots.OrderByDescending(i => i.When));
            return result;
        }

    }
}
