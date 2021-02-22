using SAEON.Observations.Core;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class InventoryDatasetsController : WebApiController<InventoryDataset>
    {
        /// <summary>
        /// All Datasets
        /// </summary>
        /// <returns></returns>
        public override IQueryable<InventoryDataset> GetAll()
        {
            return base.GetAll();
        }
    }
}
