using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using SAEON.Observations.Core.SensorThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.OData;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
{
    [ODataRouteName("SensorThings")]
    public class BaseSensorThingsControllers<TEntity> : ODataController where TEntity : BaseSensorThingEntity
    {
        protected ObservationsDbContext db = new ObservationsDbContext();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        protected virtual List<TEntity> GetList()
        {
            return new List<TEntity>();
        }

        //[EnableQuery, ODataRoute] Required in derived class
        public virtual IQueryable<TEntity> GetAll()
        {
            using (Logging.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    var list = GetList();
                    list.ForEach(i => i.GenerateSensorThingsProperties());
                    Logging.Verbose("List: {@list}", list);
                    return list.AsQueryable();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get all");
                    throw;
                }
            }
        }
    }
}