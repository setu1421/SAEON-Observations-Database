using Microsoft.AspNetCore.Mvc;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class InventorySensorsController : ApiReadController<VInventorySensor>
    {
        protected override List<Expression<Func<VInventorySensor, bool>>> GetWheres()
        {
            var result = base.GetWheres();
            result.Add(i => i.IsValid ?? false);
            return result;
        }

        /// <summary>
        /// All Sensors
        /// </summary>
        /// <returns></returns>
        public override Task<IEnumerable<VInventorySensor>> GetAll()
        {
            return base.GetAll();
        }
    }
}
