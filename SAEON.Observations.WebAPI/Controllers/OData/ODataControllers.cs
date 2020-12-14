using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Cors;
using SAEON.Logs;
using SAEON.Observations.Auth;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    [EnableCors(SAEONAuthenticationDefaults.CorsAllowAllPolicy)]
    public abstract class ODataController<TEntity> : BaseODataController<TEntity> where TEntity : BaseEntity
    {
    }

    public abstract class IDEntityODataController<TEntity> : ODataController<TEntity> where TEntity : GuidIdEntity
    {
        /// <summary>
        /// Get a TEntity by Id
        /// </summary>
        /// <param name="id">The Id of the TEntity</param>
        /// <returns>TEntity</returns>
        [EnableQuery(PageSize = ODataDefaults.PageSize, MaxTop = ODataDefaults.MaxTop)]
        public virtual SingleResult<TEntity> GetById(Guid id)
        {
            using (SAEONLogs.MethodCall<SingleResult<TEntity>>(GetType(), new MethodCallParameters { { "Id", id } }))
            {
                try
                {
                    UpdateRequest();
                    return SingleResult.Create(GetQuery(i => (i.Id == id)));
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }
    }

    public abstract class CodedODataController<TEntity> : IDEntityODataController<TEntity> where TEntity : CodedEntity
    {
        protected override List<Expression<Func<TEntity, object>>> GetOrderBys()
        {
            var result = base.GetOrderBys();
            result.Insert(0, i => i.Code);
            return result;
        }
    }

    public abstract class NamedODataController<TEntity> : CodedODataController<TEntity> where TEntity : NamedEntity
    {
        protected override List<Expression<Func<TEntity, object>>> GetOrderBys()
        {
            var result = base.GetOrderBys();
            result.Insert(0, i => i.Name);
            return result;
        }
    }

}
