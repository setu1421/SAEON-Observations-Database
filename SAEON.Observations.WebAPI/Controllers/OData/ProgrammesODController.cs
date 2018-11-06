using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.Core.Entities;
using System;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    /// <summary>
    /// Programmes
    /// </summary>
    [ODataRoutePrefix("Programmes")]
    public class ProgrammesODController : NamedController<Programme>
    {

        // GET: odata/Programmes
        /// <summary>
        /// Get all Programmes
        /// </summary>
        /// <returns>ListOf(Programme)</returns>
        [EnableQuery, ODataRoute]
        public override IQueryable<Programme> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/Programmes(5)
        /// <summary>
        /// Programme by Id
        /// </summary>
        /// <param name="id">Id of Programme</param>
        /// <returns>Programme</returns>
        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<Programme> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        // GET: odata/Programmes(5)/Projects
        /// <summary>
        /// Projects for the Programme
        /// </summary>
        /// <param name="id">Id of the Programme</param>
        /// <returns>ListOf(Project)</returns>
        [EnableQuery, ODataRoute("({id})/Projects")]
        public IQueryable<Project> GetProjects([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Projects, i => i.Programme);
        }
    }
}
