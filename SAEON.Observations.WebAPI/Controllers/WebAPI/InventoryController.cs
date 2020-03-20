using SAEON.Observations.Core.Entities;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    /// <summary>
    /// </summary>
    [RoutePrefix("Api/Inventory")]
    public class InventoryController : BaseEntityController<Inventory>
    {
        /// <summary>
        /// Inventory of sensors
        /// </summary>
        /// <returns>ListOf(Inventory)</returns>
        [ResponseType(typeof(Inventory))]
        public override IQueryable<Inventory> GetAll()
        {
            return base.GetAll();
        }

    }
}
