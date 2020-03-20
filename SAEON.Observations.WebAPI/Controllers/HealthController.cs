﻿using SAEON.AspNet.Common;
using SAEON.Logs;
using SAEON.Observations.Core.Entities;
using SAEON.Observations.WebAPI.Models;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers
{
    [RoutePrefix("Health")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HealthController : ApiController
    {
        protected ObservationsDbContext db = null;

        public HealthController()
        {
            db = new ObservationsDbContext(ConfigurationManager.AppSettings[AspNetConstants.TenantDefault] ?? "SAEON");
        }

        [HttpGet]
        [Route]
        public async Task<IHttpActionResult> GetAsync()
        {
            using (Logging.MethodCall(GetType()))
            {
                Logging.Information("Open Data Platform - Url: {ODPUrl} Key: {ODPApiKey}", Properties.Settings.Default.ODPUrl, ConfigurationManager.AppSettings["ODPApiKey"]);
                var model = new HealthModel();
                try
                {
                    if (!db.Database.Exists())
                    {
                        model.Database = "Not exist";
                        model.Healthy = false;
                    }
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    model.Database = ex.Message;
                    model.Healthy = false;
                }
                try
                {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var url = Properties.Settings.Default.IdentityServerUrl + "/Health";
                    Logging.Verbose("Calling: {url}", url);
                    var response = await client.GetAsync(url);
                    Logging.Verbose("Response: {response}", response);
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    model.IdentityService = ex.Message;
                    model.Healthy = false;
                }
                return Ok(model);
            }
        }
    }
}
