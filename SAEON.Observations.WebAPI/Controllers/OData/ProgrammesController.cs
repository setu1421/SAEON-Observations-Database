using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.Core;
using System;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    [ODataRoutePrefix("Programmes")]
    public class ProgrammesController : CodedNamedODataController<Programme>
    {
        [ODataRoute("({id})")]
        public override SingleResult<Programme> GetById(Guid id)
        {
            return base.GetById(id);
        }

        [ODataRoute("({id})/Projects")]
        public IQueryable<Project> GetProjects([FromODataUri] Guid id)
        {
            return GetManyWithGuidId(id, s => s.Projects);
        }

    }
}
