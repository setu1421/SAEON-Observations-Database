using AutoMapper;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [EnableCors(SAEONAuthenticationDefaults.CorsAllowQuerySitePolicy)]
    //[Authorize(Policy = ODPAuthenticationDefaults.AllowedClientsPolicy)]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public abstract class InternalApiController : BaseApiController
    {
    }

    [Route("Internal/[controller]")]
    [EnableCors(SAEONAuthenticationDefaults.CorsAllowQuerySitePolicy)]
    //[Authorize(Policy = ODPAuthenticationDefaults.AllowedClientsPolicy)]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public abstract class InternalListController<TEntity> : BaseListController<TEntity> where TEntity : class
    {
    }

    [Route("Internal/[controller]")]
    [EnableCors(SAEONAuthenticationDefaults.CorsAllowQuerySitePolicy)]
    //[Authorize(Policy = ODPAuthenticationDefaults.AllowedClientsPolicy)]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public abstract class InternalReadController<TEntity> : BaseIdedReadController<TEntity> where TEntity : IdedEntity
    {
        protected override void UpdateRequest()
        {
            EntityConfig.BaseUrl = Request.GetUri().GetLeftPart(UriPartial.Authority) + "/Internal";
        }
    }

    [Route("Internal/[controller]")]
    [EnableCors(SAEONAuthenticationDefaults.CorsAllowQuerySitePolicy)]
    //[Authorize(Policy = ODPAuthenticationDefaults.AllowedClientsPolicy)]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Policy = ODPAuthenticationDefaults.IdTokenPolicy)]
    public abstract class InternalWriteController<TEntity> : BaseIdedReadController<TEntity> where TEntity : NamedEntity
    {
        protected IMapper Mapper { get; private set; }

        public InternalWriteController() : base()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserDownload, UserDownload>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
                    .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
                cfg.CreateMap<UserQuery, UserQuery>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
                    .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
            });
            Mapper = config.CreateMapper();
            TrackChanges = true;
        }

        protected override void UpdateRequest()
        {
            EntityConfig.BaseUrl = Request.GetUri().GetLeftPart(UriPartial.Authority) + "/Internal";
        }

        protected abstract bool IsEntityOk(TEntity item, bool isPost);

        protected abstract void SetEntity(ref TEntity item, bool isPost);

        public List<string> ModelStateErrors
        {
            get
            {
                return ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception).ToList();
            }
        }

        [HttpPost]
        public async Task<ActionResult<TEntity>> Post([FromBody] TEntity item)
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "item", item } }))
            {
                try
                {
                    UpdateRequest();
                    if (item == null)
                    {
                        SAEONLogs.Error("item cannot be null");
                        return BadRequest("item cannot be null");
                    }
                    SAEONLogs.Verbose("Adding {Name} {@item}", item.Name, item);
                    if (!ModelState.IsValid)
                    {
                        SAEONLogs.Error("ModelState.Invalid {ModelStateErrors}", ModelStateErrors);
                        return BadRequest(ModelState);
                    }
                    if (!IsEntityOk(item, true))
                    {
                        SAEONLogs.Error("{Name} invalid", item.Name);
                        return BadRequest($"{item.Name} invalid");
                    }
                    try
                    {
                        SetEntity(ref item, true);
                        SAEONLogs.Verbose("Add {@item}", item);
                        DbContext.Set<TEntity>().Add(item);
                        await DbContext.SaveChangesAsync();
                    }
                    catch (DbUpdateException ex)
                    {
                        if (await GetQuery().Where(i => i.Name == item.Name).AnyAsync())
                        {
                            SAEONLogs.Error("{Name} conflict", item.Name);
                            return Conflict();
                        }
                        else
                        {
                            SAEONLogs.Exception(ex, "Unable to add {Name}", item.Name);
                            return BadRequest(ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        SAEONLogs.Exception(ex, "Unable to add {Name}", item.Name);
                        return BadRequest($"Unable to add {item.Name}");
                    }
                    var location = $"{typeof(TEntity).Name}/{item.Id}";
                    SAEONLogs.Verbose("Location: {location} Id: {Id} Item: {@item}", location, item.Id, item);
                    return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to add {Name}", item.Name);
                    throw;
                }
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> PutById(Guid id, [FromBody] TEntity delta)
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "id", id }, { "delta", delta } }))
            {
                try
                {
                    UpdateRequest();
                    SAEONLogs.Information("Put: {id}", id);
                    if (delta == null)
                    {
                        SAEONLogs.Error("delta cannot be null");
                        return BadRequest("delta cannot be null");
                    }
                    SAEONLogs.Verbose("Updating {id} {@delta}", id, delta);
                    if (!ModelState.IsValid)
                    {
                        SAEONLogs.Error("ModelState.Invalid {ModelStateErrors}", ModelStateErrors);
                        return BadRequest(ModelState);
                    }
                    if (id != delta.Id)
                    {
                        SAEONLogs.Error("{id} Id not same", id);
                        return BadRequest($"{id} Id not same");
                    }
                    if (!IsEntityOk(delta, false))
                    {
                        SAEONLogs.Error("{delta.Name} invalid", delta);
                        return BadRequest($"{delta.Name} invalid");
                    }
                    var item = await GetQuery().Where(i => i.Id == id).FirstOrDefaultAsync();
                    if (item == null)
                    {
                        SAEONLogs.Error("{id} not found", id);
                        return NotFound();
                    }
                    try
                    {
                        //SAEONLogs.Verbose("Loaded {@item}", item);
                        Mapper.Map(delta, item);
                        //SAEONLogs.Verbose("Mapped delta {@item}", item);
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
                    if (item == null)
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
