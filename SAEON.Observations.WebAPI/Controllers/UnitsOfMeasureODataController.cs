using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.ModelBinding;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.OData.Routing;
using SAEON.Observations.Core;

namespace SAEON.Observations.WebAPI.Controllers
{
    /// <summary>
    /// UnitsOfMeasure
    /// </summary>
    [ODataRoutePrefix("UnitsOfMeasure")]
    public class UnitsOfMeasureODataController : BaseODataController<UnitOfMeasure>
    {

        // GET: odata/UnitsOfMeasure
        /// <summary>
        /// Get a list of UnitsOfMeasure
        /// </summary>
        /// <returns>A list of UnitOfMeasure</returns>
        [EnableQuery, ODataRoute]
        public override IQueryable<UnitOfMeasure> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/UnitsOfMeasure(5)
        /// <summary>
        /// Get an UnitOfMeasure by Id
        /// </summary>
        /// <param name="id">Id of UnitOfMeasure</param>
        /// <returns>UnitOfMeasure</returns>
        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<UnitOfMeasure> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        // GET: odata/UnitsOfMeasure(5)
        /// <summary>
        /// Get an UnitOfMeasure by Name
        /// </summary>
        /// <param name="name">Name of UnitOfMeasure</param>
        /// <returns>UnitOfMeasure</returns>
        [EnableQuery, ODataRoute("({name})")]
        public override SingleResult<UnitOfMeasure> GetByName([FromODataUri] string name)
        {
            return base.GetByName(name);
        }

        // GET: odata/UnitsOfMeasure(5)/Phenomena
        [EnableQuery, ODataRoute("({id})/Phenomena")]
        public IQueryable<Phenomenon> GetPhenomena([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Phenomena, i => i.UnitsOfMeasure);
        }
    }
}
