using SAEON.Observations.Core;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.OData;

namespace SAEON.Observations.WebAPI.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    //[Authorize(Roles = "Administrators, DataReaders")]
    public abstract class BaseODataController<TEntity> : ODataController where TEntity : BaseEntity
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

        /// <summary>
        /// Overwrite to filter entities
        /// </summary>
        /// <returns>PredicateOf(TEntity)</returns>
        protected virtual Expression<Func<TEntity, bool>> EntityFilter()
        {
            return null;
        }

        /// <summary>
        /// Overwrite to do additional checks before Post or Put
        /// </summary>
        /// <param name="item">TEntity</param>
        /// <returns>True if TEntity is Ok else False</returns>
        protected virtual bool IsEntityOk(TEntity item)
        {
            return true;
        }

        /// <summary>
        /// Overwrite to do additional checks before Post or Put
        /// </summary>
        /// <param name="item">TEntity</param>
        protected virtual void SetEntity(ref TEntity item)
        { }


        [EnableQuery]
        public virtual IQueryable<TEntity> GetAll()
        {
            using (LogContext.PushProperty("Method", "GetAll"))
            {
                try
                {
                    var filter = EntityFilter();
                    if (filter == null)
                        return db.Set<TEntity>().OrderBy(i => i.Name);
                    else
                        return db.Set<TEntity>().Where(filter).OrderBy(i => i.Name);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get all");
                    throw;
                }
            }
        }

        [EnableQuery]
        public virtual SingleResult<TEntity> GetById([FromODataUri] Guid id)
        {
            using (LogContext.PushProperty("Method", "GetById"))
            {
                try
                {
                    var filter = EntityFilter();
                    if (filter == null)
                        return SingleResult.Create(db.Set<TEntity>().Where(i => (i.Id == id)));
                    else
                        return SingleResult.Create(db.Set<TEntity>().Where(filter).Where(i => (i.Id == id)));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        [EnableQuery]
        public virtual SingleResult<TEntity> GetByName([FromODataUri] string name)
        {
            using (LogContext.PushProperty("Method", "GetByName"))
            {
                try
                {
                    var filter = EntityFilter();
                    if (filter == null)
                        return SingleResult.Create(db.Set<TEntity>().Where(i => (i.Name == name)));
                    else
                        return SingleResult.Create(db.Set<TEntity>().Where(filter).Where(i => (i.Name == name)));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get {name}", name);
                    throw;
                }
            }
        }

    }


}