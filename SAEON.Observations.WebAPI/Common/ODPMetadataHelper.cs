using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SAEON.AspNet.Auth;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.WebAPI.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI
{
    public static class ODPMetadataHelper
    {
        public static async Task<string> CreateODPMetadata(ObservationsDbContext dbContext, IHubContext<AdminHub> adminHub, HttpContext httpContext, IConfiguration config)
        {
            async Task GenerateODPMetadata(HttpClient client)
            {
                async Task GenerateODPMetadataForDOI(DigitalObjectIdentifier doi, string collection)
                {
                    // No MetadataJson
                    if (doi.MetadataJson == DOIHelper.BlankJson)
                    {
                        SAEONLogs.Warning($"Ignoring (Bad MetadataJson) {doi.DOIType} {doi.Code}, {doi.Name}");
                        await AddLineAsync($"Ignoring (Bad MetadataJson) {doi.DOIType} {doi.Code}, {doi.Name}");
                        return;
                    }
                    var needsUpdate = doi.ODPMetadataNeedsUpdate ?? false;
                    var needsPublish = !doi.ODPMetadataIsPublished ?? true;
                    //SAEONLogs.Verbose("NeedsUpdate: {NeedsUpdate} NeedsPublish: {NeedsPublish}", needsUpdate, needsPublish);
                    if (!needsUpdate && !needsPublish) return;
                    if (needsUpdate)
                    {
                        await AddLineAsync($"Updating {doi.DOIType} {doi.Code}, {doi.Name}");
                        var jObj = new JObject(
                            new JProperty("doi", doi.DOI),
                            new JProperty("collection_key", collection),
                            new JProperty("schema_key", "saeon-datacite-4-3"),
                            new JProperty("metadata", JObject.Parse(doi.MetadataJson)),
                            new JProperty("terms_conditions_accepted", true),
                            new JProperty("data_agreement_accepted", true),
                            new JProperty("data_agreement_url", "https://observations.saeon.ac.za/DataUsage"),
                            new JProperty("capture_method", "harvester")
                            );
                        var response = await client.PostAsync("metadata/", new StringContent(jObj.ToString(), Encoding.UTF8, MediaTypeNames.Application.Json));
                        if (!response.IsSuccessStatusCode)
                        {
                            SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                            SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync());
                        }
                        response.EnsureSuccessStatusCode();
                        var jsonODP = await response.Content.ReadAsStringAsync();
                        SAEONLogs.Verbose("jsonODP: {jsonODP}", jsonODP);
                        var jODP = JObject.Parse(jsonODP);
                        var odpId = jODP.Value<string>("id");
                        var validated = jODP.Value<bool>("validated");
                        doi.ODPMetadataId = new Guid(odpId);
                        var errors = jODP["errors"];
                        doi.ODPMetadataErrors = errors.ToString();
                        doi.ODPMetadataIsValid = validated && !errors.HasValues;
                        doi.ODPMetadataNeedsUpdate = !doi.ODPMetadataIsValid;
                    }
                    if (needsPublish && (doi.ODPMetadataIsValid ?? false))
                    {
                        await AddLineAsync($"Publishing {doi.DOIType} {doi.Code}, {doi.Name}");
                        doi.ODPMetadataIsPublished = null;
                        doi.ODPMetadataPublishErrors = null;
                        var response = await client.PostAsync($"metadata/workflow/{doi.ODPMetadataId}?state=published", null);
                        if (!response.IsSuccessStatusCode)
                        {
                            SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                            SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync());
                        }
                        response.EnsureSuccessStatusCode();
                        var jsonODP = await response.Content.ReadAsStringAsync();
                        SAEONLogs.Verbose("jsonODP: {jsonODP}", jsonODP);
                        var jODP = JObject.Parse(jsonODP);
                        var errors = jODP["errors"];
                        doi.ODPMetadataPublishErrors = errors.ToString();
                        doi.ODPMetadataIsPublished = jODP["success"].ToString().IsTrue() && !errors.HasValues;
                    }
                    if (dbContext.Entry(doi).State != EntityState.Unchanged)
                    {
                        doi.UpdatedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString();
                    }
                    await dbContext.SaveChangesAsync();
                }

                async Task GenerateODPMetadataForDynamicDOI(DigitalObjectIdentifier doi)
                {
                    if (!MetadataHelper.DynamicDOITypes.Contains(doi.DOIType))
                        throw new InvalidOperationException("Invalid DOI type");
                    await GenerateODPMetadataForDOI(doi, "observations-database-dynamic-collection");
                }

                foreach (var doiDataset in await dbContext.DigitalObjectIdentifiers.Where(i => i.DOIType == DOIType.Dataset).OrderBy(i => i.Id).ToListAsync())
                {
                    await GenerateODPMetadataForDynamicDOI(doiDataset);
                }
            }

            var sb = new StringBuilder();
            using (var client = await GetClient(config))
            {
                await AddLineAsync("Creating ODP Metadata");
                await GenerateODPMetadata(client);
                await AddLineAsync("Done");
                return sb.ToString();
            }

            async Task AddLineAsync(string line)
            {
                sb.AppendLine(line);
                SAEONLogs.Information(line);
                await adminHub.Clients.All.SendAsync(SignalRDefaults.CreateODPMetadataStatusUpdate, line);
            }
        }

        public static async Task<HttpClient> GetClient(IConfiguration config)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(config["ODPMetadataUrl"].AddEndForwardSlash())
            };
            client.SetBearerToken(await GetTokenAsync());
            return client;

            async Task<string> GetTokenAsync()
            {
                using (SAEONLogs.MethodCall(typeof(ODPMetadataHelper)))
                {
                    try
                    {
                        using (var client = new HttpClient() { BaseAddress = new Uri(config["AuthenticationServerUrl"].AddEndForwardSlash()) })
                        {
                            using (var formContent = new FormUrlEncodedContent(new[] {
                                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                                new KeyValuePair<string, string>("scope", "ODP.Metadata"),
                                new KeyValuePair<string, string>("client_id", "observations-metadata-harvester"),
                                new KeyValuePair<string, string>("client_secret", config["ODPMetadataClientSecret"]),
                                }))
                            {
                                //SAEONLogs.Information("scope: {scope} client_id: {client_id} client_secret: {client_secret}", ODPAuthenticationDefaults.WebAPIClientId, ODPAuthenticationDefaults.QuerySiteClientId), config["QuerySiteClientSecret"]);
                                SAEONLogs.Verbose("Requesting access token");
                                var response = await client.PostAsync("oauth2/token", formContent);
                                if (!response.IsSuccessStatusCode)
                                {
                                    SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                                    SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync());
                                }
                                response.EnsureSuccessStatusCode();
                                var accessToken = await response.Content.ReadAsStringAsync();
                                if (string.IsNullOrWhiteSpace(accessToken))
                                {
                                    SAEONLogs.Error("AccessToken is invalid");
                                    if (string.IsNullOrWhiteSpace(accessToken)) throw new InvalidOperationException("AccessToken is invalid");
                                }
                                SAEONLogs.Verbose("AccessToken: {AccessToken}", accessToken);
                                var jObj = JObject.Parse(accessToken);
                                var token = jObj.Value<string>("access_token");
                                SAEONLogs.Verbose("BearerToken: {BearerToken}", token);
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
        }

    }
}
