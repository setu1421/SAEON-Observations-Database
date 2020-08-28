using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SAEON.Logs;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    [Route("Api/[controller]")]
    [Authorize(Policy = TenantAuthenticationDefaults.TenantPolicy)]
    //[Authorize(Policy = ODPAuthenticationDefaults.AccessTokenPolicy)]
    [ApiController]
    public abstract class BaseController<TEntity> : ControllerBase where TEntity : BaseEntity
    {
        private ObservationsDbContext _dbContext;
        protected ObservationsDbContext DbContext => _dbContext ??= HttpContext.RequestServices.GetRequiredService<ObservationsDbContext>();

        [NotMapped]
        public string EntitySetName { get; protected set; }
        [NotMapped]
        public List<string> NavigationLinks { get; } = new List<string>();

        protected void UpdateRequest()
        {
            EntityConfig.BaseUrl = Request.GetUri().GetLeftPart(UriPartial.Authority) + "/Api";
        }

        /// <summary>
        /// Overwrite to order of entities
        /// </summary>
        /// <returns>ListOf(PredicateOf(TEntity))</returns>
        protected virtual List<Expression<Func<TEntity, object>>> GetOrderBys()
        {
            return new List<Expression<Func<TEntity, object>>>();
        }

        /// <summary>
        /// query for items
        /// </summary>
        /// <returns></returns>
        protected virtual IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> extraWhere = null)
        {
            var query = DbContext.Set<TEntity>().AsNoTracking().AsQueryable();
            if (extraWhere != null)
            {
                query = query.Where(extraWhere);
            }
            foreach (var orderBy in GetOrderBys())
            {
                query = query.OrderBy(orderBy);
            }
            return query;
        }

        /// <summary>
        /// Get all TEntity
        /// </summary>
        /// <returns>ListOf(TEntity)</returns>
        [HttpGet]
        //[Route]
        public virtual IQueryable<TEntity> GetAll()
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    UpdateRequest();
                    SAEONLogs.Verbose("uri: {uri}", Request.GetUri());
                    return GetQuery();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }
    }
}
