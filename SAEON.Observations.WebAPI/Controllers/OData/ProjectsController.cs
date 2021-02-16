using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.Core;
using System;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    [ODataRoutePrefix("Projects")]
    public class ProjectsController : NamedODataController<Project>
    {
        [ODataRoute("({id})/Programme")]
        public Programme GetProgramme(Guid id)
        {
            return GetSingle(id, i => i.Programme);
        }

        [ODataRoute("({id})/Stations")]
        public IQueryable<Station> GetStations(Guid id)
        {
            return GetManyWithGuidId(id, s => s.Stations);
        }

    }
}
