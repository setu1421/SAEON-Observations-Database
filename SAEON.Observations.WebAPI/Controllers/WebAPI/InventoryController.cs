using SAEON.Observations.Core.Entities;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    /// <summary>
    /// </summary>
    [RoutePrefix("Api/Inventory")]
    public class InventoryController : BaseEntityController<Inventory>
    {
        /// <summary>
        /// All Inventory
        /// </summary>
        /// <returns>ListOf(Inventory)</returns>
        public override IQueryable<Inventory> GetAll()
        {
            return base.GetAll();
        }

    }
}
