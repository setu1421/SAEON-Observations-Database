using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.Core;
using System;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    [ODataRoutePrefix("Datasets")]
    public class DatasetsController : CodedNamedODataController<VDatasetExpansion>
    {
        [ODataRoute("({id})/Station")]
        public Station GetStation(Guid id)
        {
            return GetSingle(id, s => s.Station);
        }

        [ODataRoute("({id})/Phenomenon")]
        public Phenomenon GetPhenomenon(Guid id)
        {
            return GetSingle(id, s => s.Phenomenon);
        }

        [ODataRoute("({id})/Offering")]
        public Offering GetOffering(Guid id)
        {
            return GetSingle(id, s => s.Offering);
        }

        [ODataRoute("({id})/Unit")]
        public Unit GetUnit(Guid id)
        {
            return GetSingle(id, s => s.Unit);
        }

    }
}
