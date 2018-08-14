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
        [EnableQuery, ODataRoute]
        public override IQueryable<Inventory> GetAll()
        {
            return base.GetAll();
        }
    }
}
