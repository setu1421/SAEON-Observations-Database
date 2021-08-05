using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using SAEON.AspNet.Auth;
using SAEON.Core;
using SAEON.Logs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SAEON.Observations.Service
{
    public class Worker : BackgroundService
    {
        private const int MinutesBetweenRuns = 1;
        private const int SecondsDelay = 1;
        private DateTime lastRun = DateTime.MinValue;

        public IConfiguration Config { get; }

        public Worker(IConfiguration config)
        {
            Config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var currentTime = DateTime.Now;
                        var elapsedMinutes = (currentTime - lastRun).TotalMinutes;
                        //SAEONLogs.Verbose("Last: {Last} Current: {Current} Elapsed: {ElapsedMinutes}", lastRun, currentTime, elapsedMinutes);
                        if (elapsedMinutes >= MinutesBetweenRuns)
                        {
                            SAEONLogs.Information("Worker running at: {time}", currentTime);
                            lastRun = currentTime.Date.AddHours(currentTime.Hour).AddMinutes(currentTime.Minute);
                            using (var client = await GetWebAPIClientAsync())
                            {
                                var url = Config["WebAPIHealthCheckUrl"];
                                var response = await client.GetAsync(url, stoppingToken);
                                SAEONLogs.Verbose("Response: {response}", response);
                                response.EnsureSuccessStatusCode();
                            }
                        }
                        await Task.Delay(1000 * SecondsDelay, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get all");
                    throw;
                }
            }
        }

        private async Task<string> GetAccessTokenAsync()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    using (var client = new HttpClient() { BaseAddress = new Uri(Config["AuthenticationServerUrl"].AddTrailingForwardSlash()) })
                    {
                        using (var formContent = new FormUrlEncodedContent(new[] {
                                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                                new KeyValuePair<string, string>("scope", Config["WebAPIClientId"]),
                                new KeyValuePair<string, string>("client_id", Config["QuerySiteClientId"]),
                                new KeyValuePair<string, string>("client_secret", Config["QuerySiteClientSecret"]),
                                }))
                        {
                            //SAEONLogs.Information("scope: {scope} client_id: {client_id} client_secret: {client_secret}", ODPAuthenticationDefaults.WebAPIClientId, ODPAuthenticationDefaults.QuerySiteClientId), config["QuerySiteClientSecret"]);
                            SAEONLogs.Verbose("Requesting ODP token");
                            var response = await client.PostAsync("oauth2/token", formContent);
                            if (!response.IsSuccessStatusCode)
                            {
                                SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                                SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync());
                            }
                            response.EnsureSuccessStatusCode();
                            var odpToken = await response.Content.ReadAsStringAsync();
                            SAEONLogs.Verbose("ODPToken: {ODPToken}", odpToken);
                            if (string.IsNullOrWhiteSpace(odpToken))
                            {
                                SAEONLogs.Error("ODPToken is invalid");
                                throw new InvalidOperationException("ODPToken is invalid");
                            }
                            var jObj = JObject.Parse(odpToken);
                            var token = jObj.Value<string>("access_token");
                            SAEONLogs.Verbose("AccessToken: {AccessToken}", token);
                            return token;
                        }
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        protected async Task<string> GetAuthorizationAsync()
        {
            var token = await GetAccessTokenAsync();
            return $"Bearer {token}";
        }

        private HttpClient GetWebAPIClientBase()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var client = new HttpClient
                    {
                        //client.DefaultRequestHeaders.Add(Constants.HeaderKeyTenant, Tenant);
                        BaseAddress = new Uri(Config["WebAPIUrl"].AddTrailingForwardSlash()),
                        Timeout = TimeSpan.FromMinutes(Convert.ToInt32(Config["WebAPITimeoutMins"] ?? "15"))
                    };
                    return client;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        protected async Task<HttpClient> GetWebAPIClientAsync()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var client = GetWebAPIClientBase();
                    var token = await GetAccessTokenAsync();
                    client.SetBearerToken(token);
                    return client;
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
