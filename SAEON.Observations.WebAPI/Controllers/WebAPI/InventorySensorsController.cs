using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class InventorySensorsController : ApiReadController<VInventorySensor>
    {
        protected override List<Expression<Func<VInventorySensor, bool>>> GetWheres()
        {
            var result = base.GetWheres();
            result.Add(i => i.VerifiedCount > 0 && i.LatitudeNorth.HasValue && i.LatitudeSouth.HasValue && i.LongitudeEast.HasValue && i.LongitudeWest.HasValue);
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
