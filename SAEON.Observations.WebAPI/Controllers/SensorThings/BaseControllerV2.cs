/*
using AutoQueryable.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAEON.AspNet.WebApi;
using SAEON.Logs;
using SAEON.Observations.Core.Entities;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [TenantAuthorization]
    public abstract class BaseControllerV2<TEntity> : ApiController where TEntity : SensorThingEntity
    {
        private ObservationsDbContext dbContext = null;
        protected ObservationsDbContext DbContext
        {
            get
            {
                if (dbContext == null) dbContext = new ObservationsDbContext(TenantAuthorizationAttribute.GetTenantFromHeaders(Request));
                return dbContext;
            }
            private set => dbContext = value;
        }

        public BaseControllerV2()
        {
            SensorThingsConfig.BaseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority) + "/SensorThings";
        }

        ~BaseControllerV2()
        {
            DbContext = null;
        }

        protected abstract IQueryable<TEntity> GetEntities();

        [HttpGet]
        [Route]
        public virtual JToken GetAll()
        {
            using (Logging.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    var queryString = Request.RequestUri.Query.Replace("$", "");
                    Logging.Verbose("QueryString: {QueryString}", queryString);
                    var entities = GetEntities().AutoQueryable(queryString);
                    string json = JsonConvert.SerializeObject(entities);
                    //Logging.Verbose("json: {json}", json);
                    var arr = JArray.Parse(json);
                    Logging.Verbose("List: {count} {@list}", arr.Count(), arr.ToString());
                    var result = new JObject
                    {
                        new JProperty("@iot.count", arr.Count()),
                        new JProperty("value", arr)
                    };
                    return result;
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        //[Route("Entity({id:guid})")]
        public virtual JToken GetById([FromUri]Guid id)
        {
            using (Logging.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    //var queryString = Request.RequestUri.Query.Replace("$", "");
                    //Logging.Verbose("QueryString: {QueryString}", queryString);
                    //var entities = GetEntities().AutoQueryable(queryString);
                    var entity = GetEntities().FirstOrDefault(i => i.Id == id);
                    if (entity == null)
                    {
                        return null;
                    }
                    else
                    {
                        return entity.AsJSON;
                    }
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

    }
}
*/