using SAEON.Observations.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class InventoryDatasetsController : ApiReadController<InventoryDataset>
    {
        /// <summary>
        /// All Datasets
        /// </summary>
        /// <returns></returns>
        public override Task<IEnumerable<InventoryDataset>> GetAll()
        {
            return base.GetAll();
        }
    }
}
