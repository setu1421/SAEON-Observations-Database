using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAEON.AspNet.Auth;
using SAEON.Logs;
using SAEON.Observations.Auth;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    [Route("Internal/[controller]")]
    [EnableCors(ObsDBAuthenticationDefaults.CorsAllowQuerySitePolicy)]
    [Authorize(Policy = ObsDBAuthenticationDefaults.AllowedClientsPolicy)]
    [ApiExplorerSettings(IgnoreApi = true)]
#if ResponseCaching
    [ResponseCache(Duration = Defaults.InternalCacheDuration)]
    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
#endif
    public abstract class InternalApiController : BaseApiController
    {
    }

    [Route("Internal/[controller]")]
    [EnableCors(ObsDBAuthenticationDefaults.CorsAllowQuerySitePolicy)]
    [Authorize(Policy = ObsDBAuthenticationDefaults.AllowedClientsPolicy)]
    [ApiExplorerSettings(IgnoreApi = true)]
#if ResponseCaching
    [ResponseCache(Duration = Defaults.InternalCacheDuration)]
    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
#endif
    public abstract class InternalListController<TEntity> : BaseListController<TEntity> where TEntity : class
    {
    }

    [Route("Internal/[controller]")]
    [EnableCors(ObsDBAuthenticationDefaults.CorsAllowQuerySitePolicy)]
    [Authorize(Policy = ObsDBAuthenticationDefaults.AllowedClientsPolicy)]
    [ApiExplorerSettings(IgnoreApi = true)]
#if ResponseCaching
    [ResponseCache(Duration = Defaults.InternalCacheDuration)]
    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
#endif
    public abstract class InternalReadController<TEntity> : BaseIdedReadController<TEntity> where TEntity : IdedEntity
    {
        protected override void UpdateRequest()
        {
            EntityConfig.BaseUrl = Request.GetUri().GetLeftPart(UriPartial.Authority) + "/Internal";
        }
    }

    [Route("Internal/[controller]")]
    [EnableCors(ObsDBAuthenticationDefaults.CorsAllowQuerySitePolicy)]
    [Authorize(Policy = ObsDBAuthenticationDefaults.AllowedClientsPolicy)]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Policy = ODPAuthenticationDefaults.IdTokenPolicy)]
    public abstract class InternalWriteController<TEntity, TEntityPatch> : BaseIdedReadController<TEntity> where TEntity : NamedEntity where TEntityPatch : NamedEntity
    {
        //protected IMapper Mapper { get; private set; }

        public InternalWriteController() : base()
        {
            TrackChanges = true;
        }

        protected override void UpdateRequest()
        {
            EntityConfig.BaseUrl = Request.GetUri().GetLeftPart(UriPartial.Authority) + "/Internal";
        }

        protected abstract bool IsEntityOk(TEntity item, bool isPost);
        protected abstract bool IsEntityPatchOk(TEntityPatch item);
        protected abstract void SetEntity(ref TEntity item, bool isPost);
        protected abstract void UpdateEntity(ref TEntity item, TEntity updateItem);
        protected abstract void PatchEntity(ref TEntity item, TEntityPatch patchItem);

        public List<string> ModelStateErrors
        {
            get
            {
                return ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception).ToList();
            }
        }

        [HttpPost]
        public async Task<ActionResult<TEntity>> Post([FromBody] TEntity newItem)
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { nameof(newItem), newItem } }))
            {
                try
                {
                    UpdateRequest();
                    if (newItem is null)
                    {
                        SAEONLogs.Error($"{nameof(newItem)} cannot be null");
                        return BadRequest($"{nameof(newItem)} cannot be null");
                    }
                    SAEONLogs.Verbose("Adding {Name} {@item}", newItem.Name, newItem);
                    if (!ModelState.IsValid)
                    {
                        SAEONLogs.Error("ModelState.Invalid {ModelStateErrors}", ModelStateErrors);
                        return BadRequest(ModelState);
                    }
                    if (!IsEntityOk(newItem, true))
                    {
                        SAEONLogs.Error($"NewItem {newItem.Name} invalid");
                        return BadRequest($"NewItem {newItem.Name} invalid");
                    }
                    try
                    {
                        SetEntity(ref newItem, true);
                        SAEONLogs.Verbose("Add {@item}", newItem);
                        DbContext.Set<TEntity>().Add(newItem);
                        await DbContext.SaveChangesAsync();
                    }
                    catch (DbUpdateException ex)
                    {
                        if (await GetQuery().Where(i => i.Name == newItem.Name).AnyAsync())
                        {
                            SAEONLogs.Error("{Name} conflict", newItem.Name);
                            return Conflict();
                        }
                        else
                        {
                            SAEONLogs.Exception(ex, "Unable to add {Name}", newItem.Name);
                            return BadRequest(ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        SAEONLogs.Exception(ex, "Unable to add {Name}", newItem.Name);
                        return BadRequest($"Unable to add {newItem.Name}");
                    }
                    var location = $"{typeof(TEntity).Name}/{newItem.Id}";
                    SAEONLogs.Verbose("Location: {location} Id: {Id} Item: {@item}", location, newItem.Id, newItem);
                    return CreatedAtAction(nameof(GetById), new { id = newItem.Id }, newItem);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to add {Name}", newItem.Name);
                    throw;
                }
            }
        }

        [HttpPut("{id:guid}")]
        public virtual async Task<ActionResult> PutById(Guid id, [FromBody] TEntity updateItem)
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { nameof(id), id }, { nameof(updateItem), updateItem } }))
            {
                try
                {
                    UpdateRequest();
                    SAEONLogs.Information("Put: {id}", id);
                    if (updateItem is null)
                    {
                        SAEONLogs.Error($"{nameof(updateItem)} cannot be null");
                        return BadRequest($"{nameof(updateItem)} cannot be null");
                    }
                    SAEONLogs.Verbose("Updating {id} {@updateItem}", id, updateItem);
                    //if (!ModelState.IsValid)
                    //{
                    //    SAEONLogs.Error("ModelState.Invalid {ModelStateErrors}", ModelStateErrors);
                    //    return BadRequest(ModelState);
                    //}
                    if (id != updateItem.Id)
                    {
                        SAEONLogs.Error("{id} Id not same", id);
                        return BadRequest($"{id} Id not same");
                    }
                    var item = await GetQuery().Where(i => i.Id == id).FirstOrDefaultAsync();
                    if (item is null)
                    {
                        SAEONLogs.Error("{id} not found", id);
                        return NotFound();
                    }
                    if (!IsEntityOk(item, false))
                    {
                        SAEONLogs.Error($"Item  {item.Name} invalid");
                        return BadRequest($"Item {item.Name} invalid");
                    }
                    if (!IsEntityOk(updateItem, false))
                    {
                        SAEONLogs.Error($"UpdateItem {updateItem.Name} invalid");
                        return BadRequest($"UpdateItem {updateItem.Name} invalid");
                    }
                    try
                    {
                        SAEONLogs.Verbose("Loaded {@item} {@updateItem}", item, updateItem);
                        //Mapper.Map(delta, item);
                        //SAEONLogs.Verbose("Mapped delta {@item}", item);
                        UpdateEntity(ref item, updateItem);
                        SetEntity(ref item, false);
                        SAEONLogs.Verbose("Set {@item}", item);
                        await DbContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        SAEONLogs.Exception(ex, "Unable to update {id}", id);
                        return BadRequest(ex.Message);
                    }
                    return NoContent();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to update {id}", id);
                    throw;
                }
            }
        }

        [HttpPatch("{id:guid}")]
        public virtual async Task<ActionResult> PatchById(Guid id, [FromBody] TEntityPatch patchItem)
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { nameof(id), id }, { nameof(patchItem), patchItem } }))
            {
                try
                {
                    UpdateRequest();
                    SAEONLogs.Information("Patch: {id}", id);
                    if (patchItem is null)
                    {
                        SAEONLogs.Error($"{nameof(patchItem)} cannot be null");
                        return BadRequest($"{nameof(patchItem)} cannot be null");
                    }
                    SAEONLogs.Verbose("Updating {id} {@patchItem}", id, patchItem);
                    if (id != patchItem.Id)
                    {
                        SAEONLogs.Error("{id} Id not same", id);
                        return BadRequest($"{id} Id not same");
                    }

                    var item = await GetQuery().Where(i => i.Id == id).FirstOrDefaultAsync();
                    if (item is null)
                    {
                        SAEONLogs.Error("{id} not found", id);
                        return NotFound();
                    }
                    if (!IsEntityOk(item, false))
                    {
                        SAEONLogs.Error($"Item {item.Name} invalid");
                        return BadRequest($"Item {item.Name} invalid");
                    }
                    if (!IsEntityPatchOk(patchItem))
                    {
                        SAEONLogs.Error($"PatchItem {patchItem.Name} invalid");
                        return BadRequest($"PatchItem {patchItem.Name} invalid");
                    }
                    try
                    {
                        SAEONLogs.Verbose("Loaded {@oldItem} {@patchItem}", item, patchItem);
                        PatchEntity(ref item, patchItem);
                        SetEntity(ref item, false);
                        SAEONLogs.Verbose("Set {@item}", item);
                        await DbContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        SAEONLogs.Exception(ex, "Unable to patch {id}", id);
                        return BadRequest(ex.Message);
                    }

                    return NoContent();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to patch {id}", id);
                    throw;
                }
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteById(Guid id)
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "Id", id } }))
            {
                try
                {
                    UpdateRequest();
                    SAEONLogs.Verbose("Deleting {id}", id);
                    var item = await GetQuery().Where(i => i.Id == id).FirstOrDefaultAsync();
                    if (item is null)
                    {
                        SAEONLogs.Error("{id} not found", id);
                        return NotFound();
                    }
                    try
                    {
                        DbContext.Set<TEntity>().Remove(item);
                        SAEONLogs.Verbose("Delete {@item}", item);
                        await DbContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        SAEONLogs.Exception(ex, "Unable to delete {id}", id);
                        return BadRequest(ex.Message);
                    }
                    return NoContent();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to delete {id}", id);
                    throw;
                }
            }
        }
    }
}
