using SAEON.Logs;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    //[ClientAuthorization("SAEON.Observations.QuerySite")] Uncomment when going live
    public class BaseController<TEntity> : ApiController where TEntity : BaseEntity
    {
        protected readonly ObservationsDbContext db = null;

        public BaseController() : base()
        {
            using (Logging.MethodCall(GetType()))
            {
                db = new ObservationsDbContext();
            }
        }

        protected override void Dispose(bool disposing)
        {
            using (Logging.MethodCall(GetType()))
            {
                if (disposing)
                {
                    if (db != null)
                    {
                        db.Dispose();
                    }
                }
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// query for items
        /// </summary>
        /// <returns></returns>
        protected virtual List<TEntity> GetList()
        {
            var result = new List<TEntity>();
            return result;
        }

        /// <summary>
        /// Get all TEntity
        /// </summary>
        /// <returns>ListOf(TEntity)</returns>
        [HttpGet]
        [Route]
        public IQueryable<TEntity> GetAll()
        {
            using (Logging.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    return GetList().AsQueryable();
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
