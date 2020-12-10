using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class InventorySensorsController : WebApiController<InventorySensor>
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
