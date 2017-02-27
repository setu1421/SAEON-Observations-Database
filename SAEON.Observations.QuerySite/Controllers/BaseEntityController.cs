using AutoMapper;
using SAEON.Observations.Core;
using SAEON.Observations.Data;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    [Authorize]
    //[Authorize(Roles = "Administrators, DataReaders")]
    public class BaseEntityController<TEntity> : Controller where TEntity : BaseEntity
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
        protected virtual bool IsEntityOk(TEntity item, bool isInsert = false)
        {
            return true;
        }

        /// <summary>
        /// Overwrite to do additional checks before Post or Put
        /// </summary>
        /// <param name="item">TEntity</param>
        protected virtual void SetEntity(ref TEntity item)
        { }

        /// <summary>
        /// Return all TEntity
        /// </summary>
        /// <returns>List of TEntity</returns>
        public virtual async Task<ActionResult> Index()
        {
            using (LogContext.PushProperty("Method", "Index"))
            {
                try
                {
                    var filter = EntityFilter();
                    if (filter == null)
                        return View(await db.Set<TEntity>().OrderBy(i => i.Name).ToListAsync());
                    else
                        return View(await db.Set<TEntity>().Where(filter).OrderBy(i => i.Name).ToListAsync());
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get all");
                    throw;
                }
            }
        }

        /// <summary>
        /// Return an TEntity by Id
        /// </summary>
        /// <param name="id">The Id of the TEntity</param>
        /// <returns>View(TEntity)</returns>
        [Route("{id:guid}")]
        public virtual async Task<ActionResult> Details(Guid? id)
        {
            using (LogContext.PushProperty("Method", "Details"))
            {
                try
                {
                    if ((id == null) || !id.HasValue || (id == Guid.Empty))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest,"id cannot be null");
                    }
                    var filter = EntityFilter();
                    TEntity item;
                    if (filter == null)
                        item = await db.Set<TEntity>().FirstOrDefaultAsync(i => (i.Id == id));
                    else
                        item = await db.Set<TEntity>().Where(filter).FirstOrDefaultAsync(i => (i.Id == id));
                    if (item == null)
                    {
                        Log.Error("{id} not found", id);
                        return HttpNotFound($"{id} not found");
                    }
                    return View(item);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        /// <summary>
        /// Create a TEntity
        /// </summary>
        /// <returns>View()</returns>
        [Authorize]
        //[Authorize(Roles = "Administrators, DataWriters")]
        public virtual ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create a TEntity
        /// </summary>
        /// <param name="item">TEntity to create</param>
        /// <returns>View(TEntity)</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        //[Authorize(Roles = "Administrators, DataWriters")]
        public virtual async Task<ActionResult> Create(TEntity item)
        {
            using (LogContext.PushProperty("Method", "Create"))
            {
                try
                {
                    Log.Verbose("Adding {Name} {@item}", item.Name, item);
                    if (item == null)
                    {
                        Log.Error("item cannot be null");
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "item cannot be null");

                    }
                    if (ModelState.IsValid)
                    {
                        if (!IsEntityOk(item, true))
                        {
                            Log.Error("{Name} invalid", item.Name);
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"{item.Name} invalid");
                        }
                        try
                        {
                            SetEntity(ref item);
                            item = Mapper.Map<TEntity, TEntity>(item);
                            db.Set<TEntity>().Add(item);
                            await db.SaveChangesAsync();
                        }
                        catch (DbUpdateException)
                        {
                            var filter = EntityFilter();
                            IQueryable<TEntity> query;
                            query = db.Set<TEntity>().Where(i => i.Id == item.Id);
                            if (filter != null)
                                query = query.Where(filter);
                            if (await query.AnyAsync())
                            {
                                Log.Error("{Name} conflict", item.Name);
                                return new HttpStatusCodeResult(HttpStatusCode.Conflict,$"{item.Name} conflict");
                            }
                            else
                            {
                                throw;
                            }
                        }
                        catch (DbEntityValidationException ex)
                        {
                            var validationErrors = ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors.Select(m => m.PropertyName + ": " + m.ErrorMessage)).ToList();
                            Log.Error(ex, "Unable to add {Name} {EntityValidationErrors}", item.Name, validationErrors);
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Unable to add {item.Name} EntityValidationErrors:  {string.Join("; ", validationErrors)}");

                        }
                        return RedirectToAction("Index");
                    }
                    return View(item);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to add {Name}", item.Name);
                    throw;
                }
            }

        }

        /// <summary>
        /// Edit a TEntity
        /// </summary>
        /// <param name="id">id of TEntity</param>
        /// <returns>View(TEntity)</returns>
        [Route("{id:guid}")]
        [Authorize]
        //[Authorize(Roles = "Administrators, DataWriters")]
        public virtual async Task<ActionResult> Edit(Guid? id)
        {
            using (LogContext.PushProperty("Method", "Edit"))
            {
                try
                {
                    if ((id == null) || !id.HasValue || (id == Guid.Empty))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "id cannot be null");
                    }
                    var filter = EntityFilter();
                    TEntity item;
                    if (filter == null)
                        item = await db.Set<TEntity>().FirstOrDefaultAsync(i => (i.Id == id));
                    else
                        item = await db.Set<TEntity>().Where(filter).FirstOrDefaultAsync(i => (i.Id == id));
                    if (item == null)
                    {
                        Log.Error("{id} not found", id);
                        return HttpNotFound($"{id} not found");
                    }
                    return View(item);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        /// <summary>
        /// Edit a TEntity
        /// </summary>
        /// <param name="delta">TEntity to edit</param>
        /// <returns>View(TEntity)</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        //[Authorize(Roles = "Administrators, DataWriters")]
        public virtual async Task<ActionResult> Edit(TEntity delta)
        {
            using (LogContext.PushProperty("Method", "Edit"))
            {
                try
                {
                    Log.Verbose("Updating {@delta}", delta);
                    if (delta == null)
                    {
                        Log.Error("delta cannot be null");
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "delta cannot be null");

                    }
                    if (ModelState.IsValid)
                    {
                        if (!IsEntityOk(delta))
                        {
                            Log.Error("{Name} invalid", delta.Name);
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"{delta.Name} invalid");
                        }
                        var filter = EntityFilter();
                        IQueryable<TEntity> query;
                        query = db.Set<TEntity>().Where(i => i.Id == delta.Id);
                        if (filter != null)
                        {
                            query = query.Where(filter);
                        }
                        var item = await query.FirstOrDefaultAsync();
                        if (item == null)
                        {
                            Log.Error("{id} not found", delta.Id);
                            return HttpNotFound($"{delta.Id} not found");
                        }
                        SetEntity(ref delta);
                        item = Mapper.Map<TEntity, TEntity>(delta, item);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    return View(delta);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to edit {id}", delta?.Id);
                    throw;
                }
            }

        }

        /// <summary>
        /// Delete a TEntity
        /// </summary>
        /// <param name="id">Id of the TEntity</param>
        /// <returns>View(item)</returns>
        [Route("{id:guid}")]
        [Authorize]
        //[Authorize(Roles = "Administrators, DataWriters")]
        public virtual async Task<ActionResult> Delete(Guid? id)
        {
            using (LogContext.PushProperty("Method", "Delete"))
            {
                try
                {
                    Log.Verbose("Deleting {id}", id);
                    if ((id == null) || !id.HasValue || (id == Guid.Empty))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "id cannot be null");
                    }
                    var filter = EntityFilter();
                    TEntity item;
                    if (filter == null)
                        item = await db.Set<TEntity>().FirstOrDefaultAsync(i => (i.Id == id));
                    else
                        item = await db.Set<TEntity>().Where(filter).FirstOrDefaultAsync(i => (i.Id == id));
                    if (item == null)
                    {
                        Log.Error("{id} not found", id);
                        return HttpNotFound($"{id} not found");
                    }
                    return View(item);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to delete {id}", id);
                    throw;
                }
            }

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        //[Authorize(Roles = "Administrators, DataWriters")]
        public virtual async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            using (LogContext.PushProperty("Method", "DeleteConfirmed"))
            {
                try
                {
                    Log.Verbose("Deleting {id}", id);
                    var filter = EntityFilter();
                    IQueryable<TEntity> query;
                    query = db.Set<TEntity>().Where(i => i.Id == id);
                    if (filter != null)
                    {
                        query = query.Where(filter);
                    }
                    var item = await query.FirstOrDefaultAsync();
                    if (item == null)
                    {
                        Log.Error("{id} not found", id);
                        return HttpNotFound($"{id} not found");
                    }
                    db.Set<TEntity>().Remove(item);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to delete {id}", id);
                    throw;
                }
            }
        }
    }
}