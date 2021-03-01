using SAEON.Observations.Core;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class InventorySensorsController : ApiReadController<InventorySensor>
    {
        /// <summary>
        /// All Sensors
        /// </summary>
        /// <returns></returns>
        public override IQueryable<InventorySensor> GetAll()
        {
            return base.GetAll();
        }
    }
}
