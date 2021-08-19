using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class InventoryDatasetsController : ApiReadController<InventoryDataset>
    {
        protected override List<Expression<Func<InventoryDataset, bool>>> GetWheres()
        {
            var result = base.GetWheres();
            result.Add(i => i.VerifiedCount > 0);
            return result;
        }

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
