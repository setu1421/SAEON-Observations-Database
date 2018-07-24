using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    [ODataRoutePrefix("Inventory")]
    public class InventoryController : BaseODataController<Inventory>
    {
        protected override List<Expression<Func<Inventory, object>>> GetOrderBys()
        {
            var result = base.GetOrderBys();
            result.Add(i => i.SiteName);
            result.Add(i => i.StationName);
            result.Add(i => i.InstrumentName);
            result.Add(i => i.SensorName);
            result.Add(i => i.PhenomenonName);
            result.Add(i => i.OfferingName);
            result.Add(i => i.UnitName);
            return result;
        }

        [EnableQuery, ODataRoute]
        public override IQueryable<Inventory> GetAll()
        {
            return base.GetAll();
        }
    }
}
