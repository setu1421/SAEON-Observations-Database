using SAEON.Observations.Core;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class InventoryDatasetsController : BaseController<InventoryDataset>
    {
        /// <summary>
        /// Inventory of Datasets
        /// </summary>
        /// <returns>ListOf(InventoryDataStream)</returns>
        public override IQueryable<InventoryDataset> GetAll()
        {
            return base.GetAll();
        }
    }
}
