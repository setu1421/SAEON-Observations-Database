using SAEON.Observations.Core.Entities;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    /// <summary>
    /// </summary>
    [RoutePrefix("Api/InventoryDataStreams")]
    public class InventoryDataStreamsController : BaseEntityController<InventoryDataStream>
    {
        /// <summary>
        /// Inventory of DataStreams
        /// </summary>
        /// <returns>ListOf(InventoryDataStream)</returns>
        [ResponseType(typeof(InventoryDataStream))]
        public override IQueryable<InventoryDataStream> GetAll()
        {
            return base.GetAll();
        }

    }
}
