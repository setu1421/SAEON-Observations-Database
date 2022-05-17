using SAEON.Observations.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class InventoryDatasetsController : ApiReadController<VDatasetExpansion>
    {
        /// <summary>
        /// All Datasets
        /// </summary>
        /// <returns></returns>
        public override Task<IEnumerable<VDatasetExpansion>> GetAll()
        {
            return base.GetAll();
        }
    }
}
