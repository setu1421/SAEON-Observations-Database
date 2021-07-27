using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SAEON.Logs;
using SAEON.Observations.Auth;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    [EnableCors(ObsDBAuthenticationDefaults.CorsAllowAllPolicy)]
    public abstract class ODataController<TEntity> : BaseODataController<TEntity> where TEntity : BaseEntity
    {
        protected override void UpdateRequest()
        {
            BaseUrl = Request.GetUri().GetLeftPart(UriPartial.Authority) + "/OData";
        }
    }

    public abstract class IDEntityODataController<TEntity> : ODataController<TEntity> where TEntity : GuidIdEntity
    {
        /// <summary>
        /// Get a TEntity by Id
        /// </summary>
        /// <param name="id">The Id of the TEntity</param>
        /// <returns>TEntity</returns>
        [HttpGet]
        [ODataRoute("({id})")]
        [EnableQuery(PageSize = ODataDefaults.PageSize, MaxTop = ODataDefaults.MaxTop)]
#if ResponseCaching
        [ResponseCache(Duration = Defaults.CacheDuration, VaryByQueryKeys = new[] { "id" })]
#endif

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

        [HttpGet]
        [EnableQuery(PageSize = ODataDefaults.PageSize, MaxTop = ODataDefaults.MaxTop)]
        //[ODataRoute("({id})/TRelated")] required on calling class
#if ResponseCaching
        [ResponseCache(Duration = Defaults.CacheDuration, VaryByQueryKeys = new[] { "id" })]
#endif
        protected TRelated GetSingle<TRelated>(Guid id, Expression<Func<TEntity, TRelated>> select) where TRelated : GuidIdEntity
        {
            using (SAEONLogs.MethodCall<SingleResult<TRelated>>(GetType()))
            {
                try
                {
                    UpdateRequest();
                    SAEONLogs.Verbose("uri: {uri}", Request.GetUri());
                    return GetQuery(i => i.Id == id).Select(select).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }


        [HttpGet]
        [EnableQuery(PageSize = ODataDefaults.PageSize, MaxTop = ODataDefaults.MaxTop)]
        //[ODataRoute("({id})/TRelated")] required on calling class
#if ResponseCaching
        [ResponseCache(Duration = Defaults.CacheDuration, VaryByQueryKeys = new[] { "id" })]
#endif
        protected IQueryable<TRelated> GetManyWithGuidId<TRelated>(Guid id, Expression<Func<TEntity, IEnumerable<TRelated>>> select) where TRelated : GuidIdEntity
        {
            using (SAEONLogs.MethodCall<TRelated>(GetType()))
            {
                try
                {
                    UpdateRequest();
                    SAEONLogs.Verbose("uri: {uri}", Request.GetUri());
                    return GetQuery(i => i.Id == id).SelectMany(select).Take(ODataDefaults.MaxAll);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        [HttpGet]
        [EnableQuery(PageSize = ODataDefaults.PageSize, MaxTop = ODataDefaults.MaxTop)]
        //[ODataRoute("({id})/TRelated")] required on calling class
#if ResponseCaching
        [ResponseCache(Duration = Defaults.CacheDuration, VaryByQueryKeys = new[] { "id" })]
#endif
        protected IQueryable<TRelated> GetManyWithIntId<TRelated>(Guid id, Expression<Func<TEntity, IEnumerable<TRelated>>> select) where TRelated : IntIdEntity
        {
            using (SAEONLogs.MethodCall<TRelated>(GetType()))
            {
                try
                {
                    UpdateRequest();
                    SAEONLogs.Verbose("uri: {uri}", Request.GetUri());
                    return GetQuery(i => i.Id == id).SelectMany(select).Take(ODataDefaults.MaxAll);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        [HttpGet]
        [EnableQuery(PageSize = ODataDefaults.PageSize, MaxTop = ODataDefaults.MaxTop)]
        //[ODataRoute("({id})/TRelated")] required on calling class
#if ResponseCaching
        [ResponseCache(Duration = Defaults.CacheDuration, VaryByQueryKeys = new[] { "id" })]
#endif
        protected IQueryable<TRelated> GetManyWithLongId<TRelated>(Guid id, Expression<Func<TEntity, IEnumerable<TRelated>>> select) where TRelated : LongIdEntity
        {
            using (SAEONLogs.MethodCall<TRelated>(GetType()))
            {
                try
                {
                    UpdateRequest();
                    SAEONLogs.Verbose("uri: {uri}", Request.GetUri());
                    return GetQuery(i => i.Id == id).SelectMany(select).Take(ODataDefaults.MaxAll);
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

    public abstract class NamedODataController<TEntity> : IDEntityODataController<TEntity> where TEntity : NamedEntity
    {
        protected override List<Expression<Func<TEntity, object>>> GetOrderBys()
        {
            var result = base.GetOrderBys();
            result.Insert(0, i => i.Name);
            return result;
        }
    }

    public abstract class CodedNamedODataController<TEntity> : IDEntityODataController<TEntity> where TEntity : CodedNamedEntity
    {
        protected override List<Expression<Func<TEntity, object>>> GetOrderBys()
        {
            var result = base.GetOrderBys();
            result.Insert(0, i => i.Code);
            result.Insert(0, i => i.Name);
            return result;
        }
    }

}
