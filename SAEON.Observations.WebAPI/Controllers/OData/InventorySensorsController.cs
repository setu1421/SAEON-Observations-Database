using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    public class InventorySensorsController : ODataController<VInventorySensor>
    {
        protected override List<Expression<Func<VInventorySensor, bool>>> GetWheres()
        {
            var result = base.GetWheres();
            result.Add(i => i.IsValid);
            return result;
        }
    }
}
