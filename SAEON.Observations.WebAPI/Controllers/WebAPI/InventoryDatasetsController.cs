using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class InventoryDatasetsController : WebApiController<InventoryDataset>
    {
        /// <summary>
        /// Inventory of Datasets
        /// </summary>
        /// <returns>ListOf(InventoryDataset)</returns>
        public override IQueryable<InventoryDataset> GetAll()
        {
            return base.GetAll();
        }
    }
}
