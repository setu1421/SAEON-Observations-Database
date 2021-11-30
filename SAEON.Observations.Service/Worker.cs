using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using SAEON.AspNet.Auth;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Auth;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SAEON.Observations.Service
{
    public class Worker : BackgroundService
    {
        private DateTime lastRun = DateTime.Now.Date;
        private readonly IConfiguration config;
        private readonly HubConnection adminHubConnection;

        public Worker(IConfiguration config) : base()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    SAEONLogs.Information("Starting worker");
                    this.config = config;
                    adminHubConnection = new HubConnectionBuilder()
                        .WithUrl(config["WebAPIUrl"].AddTrailingForwardSlash() + "AdminHub")
                        .Build();
                    adminHubConnection.On<string>(SignalRDefaults.CreateDOIsStatusUpdate, CreateDOIsStatusUpdate);
                    adminHubConnection.On<string>(SignalRDefaults.CreateMetadataStatusUpdate, CreateMetadataStatusUpdate);
                    adminHubConnection.On<string>(SignalRDefaults.CreateODPMetadataStatusUpdate, CreateODPMetadataStatusUpdate);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        private void CreateDOIsStatusUpdate(string status)
        {
            SAEONLogs.Verbose("CreateDOIs: {Status}", status);
        }

        private void CreateMetadataStatusUpdate(string status)
        {
            SAEONLogs.Verbose("CreateMetadata: {Status}", status);
        }

        private void CreateODPMetadataStatusUpdate(string status)
        {
            SAEONLogs.Verbose("CreateODPMetadata: {Status}", status);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    SAEONLogs.Information("Starting WebAPI AdminHub");
                    while (true)
                    {
                        try
                        {
                            await adminHubConnection.StartAsync(cancellationToken);
                            break;
                        }
                        catch
                        {
                            await Task.Delay(1000, cancellationToken);
                        }
                    }
                    await base.StartAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    //throw;
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    await adminHubConnection.DisposeAsync();
                    await base.StopAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    //throw;
                }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var currentTime = DateTime.Now;
                        var elapsedMinutes = (currentTime - lastRun).TotalMinutes;
                        //SAEONLogs.Verbose("Last: {Last} Current: {Current} Elapsed: {ElapsedMinutes}", lastRun, currentTime, elapsedMinutes);
                        var runEveryMins = Convert.ToInt32(config["RunEveryMins"] ?? "5");
                        if (elapsedMinutes >= runEveryMins)
                        {
                            SAEONLogs.Information("Worker running at: {time}", currentTime);
                            var newDay = (currentTime.Date != lastRun.Date);
                            var newHour = (currentTime.Hour != lastRun.Hour) || newDay;
                            if (config["CreateImportBatchSummaries"].IsTrue()) await CreateImportBatchSummaries();
                            if (newHour)
                            {
                                SAEONLogs.Information("New Hour: {Hour}", currentTime.Hour);
                                await CreateSnapshots();
                            }
                            if (newDay)
                            {
                                SAEONLogs.Information("New Day: {Date}", currentTime.Date);
                                await CreateImportBatchSummaries();
                                await UpdateODP();
                            }
                            var minute = (int)(Math.Floor(currentTime.Minute * 1.0 / runEveryMins) * runEveryMins);
                            lastRun = currentTime.Date.AddHours(currentTime.Hour).AddMinutes(minute);
                        }
                        await Task.Delay(1000 * Convert.ToInt32(config["DelaySecs"] ?? "15"), cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }

            async Task CreateSnapshots()
            {
                try
                {
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();
                    using (var client = await GetWebAPIClientAsync(cancellationToken))
                    {
                        SAEONLogs.Information("Creating snapshots");
                        var response = await client.PostAsync("Internal/Admin/CreateSnapshots", null, cancellationToken);
                        if (!response.IsSuccessStatusCode)
                        {
                            SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                            SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync(cancellationToken));
                        }
                        response.EnsureSuccessStatusCode();
                        var result = await response.Content.ReadAsStringAsync(cancellationToken);
                        SAEONLogs.Verbose("Snapshots: {Result}", result);
                        stopWatch.Stop();
                        SAEONLogs.Information("Done: {Elapsed}", stopWatch.Elapsed.TimeStr());
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                }
            }


            async Task CreateImportBatchSummaries()
            {
                try
                {
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();
                    using (var client = await GetWebAPIClientAsync(cancellationToken))
                    {
                        SAEONLogs.Information("Creating ImportBatchSummaries");
                        var response = await client.PostAsync("Internal/Admin/CreateImportBatchSummaries", null, cancellationToken);
                        if (!response.IsSuccessStatusCode)
                        {
                            SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                            SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync(cancellationToken));
                        }
                        response.EnsureSuccessStatusCode();
                        var result = await response.Content.ReadAsStringAsync(cancellationToken);
                        //SAEONLogs.Verbose("Snapshots: {Result}", result);
                        stopWatch.Stop();
                        SAEONLogs.Information("Done: {Elapsed}", stopWatch.Elapsed.TimeStr());
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                }
            }

            async Task UpdateODP()
            {
                try
                {
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();
                    using (var client = await GetWebAPIClientAsync(cancellationToken))
                    {
                        SAEONLogs.Information("Checking WebAPI health");
                        var response = await client.GetAsync(config["WebAPIHealthCheckUrl"], cancellationToken);
                        if (!response.IsSuccessStatusCode)
                        {
                            SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                            SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync(cancellationToken));
                        }
                        //SAEONLogs.Verbose("Response: {response}", response);
                        response.EnsureSuccessStatusCode();
                        var health = await response.Content.ReadAsStringAsync(cancellationToken);
                        SAEONLogs.Verbose("WebAPI Health: {Health}", health);
                        SAEONLogs.Information("Updating the ODP");
                        response = await client.PostAsync("Internal/Admin/UpdateODP", null, cancellationToken);
                        if (!response.IsSuccessStatusCode)
                        {
                            SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                            SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync(cancellationToken));
                        }
                        response.EnsureSuccessStatusCode();
                        var result = await response.Content.ReadAsStringAsync(cancellationToken);
                        //SAEONLogs.Verbose("Result: {Result}", result);
                        stopWatch.Stop();
                        SAEONLogs.Information("Done: {Elapsed}", stopWatch.Elapsed.TimeStr());
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                }
            }
        }

        private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    using (var client = new HttpClient() { BaseAddress = new Uri(config["AuthenticationServerUrl"].AddTrailingForwardSlash()) })
                    {
                        using (var formContent = new FormUrlEncodedContent(new[] {
                                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                                new KeyValuePair<string, string>("scope", config["WebAPIClientId"]),
                                new KeyValuePair<string, string>("client_id", config["QuerySiteClientId"]),
                                new KeyValuePair<string, string>("client_secret", config["QuerySiteClientSecret"]),
                                }))
                        {
                            //SAEONLogs.Information("scope: {scope} client_id: {client_id} client_secret: {client_secret}", ODPAuthenticationDefaults.WebAPIClientId, ODPAuthenticationDefaults.QuerySiteClientId), config["QuerySiteClientSecret"]);
                            SAEONLogs.Verbose("Requesting ODP token");
                            var response = await client.PostAsync("oauth2/token", formContent, cancellationToken);
                            if (!response.IsSuccessStatusCode)
                            {
                                SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                                SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync(cancellationToken));
                            }
                            response.EnsureSuccessStatusCode();
                            var odpToken = await response.Content.ReadAsStringAsync(cancellationToken);
                            //SAEONLogs.Verbose("ODPToken: {ODPToken}", odpToken);
                            if (string.IsNullOrWhiteSpace(odpToken))
                            {
                                SAEONLogs.Error("ODPToken is invalid");
                                throw new InvalidOperationException("ODPToken is invalid");
                            }
                            var jObj = JObject.Parse(odpToken);
                            var token = jObj.Value<string>("access_token");
                            //SAEONLogs.Verbose("AccessToken: {AccessToken}", token);
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

        private async Task<HttpClient> GetWebAPIClientAsync(CancellationToken cancellationToken)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var client = new HttpClient
                    {
                        //client.DefaultRequestHeaders.Add(Constants.HeaderKeyTenant, Tenant);
                        BaseAddress = new Uri(config["WebAPIUrl"].AddTrailingForwardSlash()),
                        Timeout = TimeSpan.FromMinutes(Convert.ToInt32(config["WebAPITimeoutMins"] ?? "15"))
                    };
                    var token = await GetAccessTokenAsync(cancellationToken);
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
