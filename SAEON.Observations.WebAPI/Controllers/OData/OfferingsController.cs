using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.Core;
using System;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    [ODataRoutePrefix("Offerings")]
    public class OfferingsController : NamedODataController<Offering>
    {
        [ODataRoute("({id})/Phenomena")]
        public IQueryable<Phenomenon> GetPhenomena(Guid id)
        {
            return GetManyWithGuidId(id, i => i.Phenomena);
        }
    }
}
