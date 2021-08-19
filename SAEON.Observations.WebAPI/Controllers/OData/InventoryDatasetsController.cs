using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    public class InventoryDatasetsController : ODataController<InventoryDataset>
    {
        protected override List<Expression<Func<InventoryDataset, bool>>> GetWheres()
        {
            var result = base.GetWheres();
            result.Add(i => i.VerifiedCount > 0);
            return result;
        }
    }
}
