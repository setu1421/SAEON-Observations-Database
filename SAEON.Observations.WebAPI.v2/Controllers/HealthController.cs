using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using SAEON.Observations.WebAPI.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.v2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Policy = Constants.TenantPolicy)]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;
        private readonly IConfiguration _config;
        private readonly ObservationsDbContext _dbContext;
        private readonly IHttpClientFactory _clientFactory;

        public HealthController(ILogger<HealthController> logger, IConfiguration config, ObservationsDbContext dbContext, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _config = config;
            _dbContext = dbContext;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<HealthModel> Get()
        {
            _logger.LogInformation("Open Data Platform - Url: {ODPUrl} Key: {ODPApiKey}", _config["ODPUrl"], _config["ODPApiKey"]);
            var model = new HealthModel();
            try
            {
                if (!_dbContext.Database.CanConnect())
                {
                    model.Database = "Not exist";
                    model.Healthy = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                model.Database = ex.Message;
                model.Healthy = false;
            }
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "Health");
                var client = _clientFactory.CreateClient("IdentityServer");
                _logger.LogDebug("Calling: {url}", client.BaseAddress+request.RequestUri.ToString());
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                model.IdentityService = ex.Message;
                model.Healthy = false;
            }

            return model;
        }
    }
}