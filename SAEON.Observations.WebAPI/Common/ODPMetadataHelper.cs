using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Auth;
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
        public static async Task<string> CreateMetadata(ObservationsDbContext dbContext, IHubContext<AdminHub> adminHub, IConfiguration config)
        {
            async Task GenerateODPMetadata(HttpClient client)
            {
                async Task GenerateODPMetadataForDOI(DigitalObjectIdentifier doi, string collection)
                {
                    var needsUpdate = doi.ODPMetadataNeedsUpdate ?? false;
                    var needsPublish = !doi.ODPMetadataIsPublished ?? true;
                    if (!needsUpdate && !needsPublish) return;
                    if (needsUpdate)
                    {
                        await AddLineAsync($"Updating {doi.DOIType} {doi.Code}, {doi.Name}");
                        var jObj = new JObject(
                            new JProperty("doi", doi.DOI),
                            new JProperty("collection_key", collection),
                            //new JProperty("schema_key", "saeon-odp-4-2"),
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
                    if (needsPublish)
                    {
                        await AddLineAsync($"Publishing {doi.DOIType} {doi.Code}, {doi.Name}");
                        doi.ODPMetadataIsPublished = null;
                        doi.ODPMetadataPublishErrors = null;
                        var response = await client.PostAsync($"metadata/workflow/{doi.ODPMetadataId}?state=published", null);
                        response.EnsureSuccessStatusCode();
                        var jsonODP = await response.Content.ReadAsStringAsync();
                        SAEONLogs.Verbose("jsonODP: {jsonODP}", jsonODP);
                        var jODP = JObject.Parse(jsonODP);
                        doi.ODPMetadataIsPublished = jODP["success"].ToString().IsTrue();
                        if (!doi.ODPMetadataIsPublished ?? false)
                        {
                            doi.ODPMetadataPublishErrors = jODP["errors"].ToString();
                        }
                    }
                    await dbContext.SaveChangesAsync();
                }

                async Task GenerateODPMetadataForDynamicDOI(DigitalObjectIdentifier doi)
                {
                    if (!MetadataHelper.DynamicDOITypes.Contains(doi.DOIType))
                        throw new InvalidOperationException("Invalid DOI type");
                    await GenerateODPMetadataForDOI(doi, "observations-database-dynamic-collection");
                }

                //async Task GenerateODPMetadataForPeriodicDOI(DigitalObjectIdentifier doi)
                //{
                //    if (doi.DOIType != DOIType.Periodic)
                //        throw new InvalidOperationException("Invalid DOI type");
                //    await GenerateODPMetadataForDOI(doi, "observations-database-periodic-collection");
                //}

                await AddLineAsync("Generating ODP metadata");
                foreach (var doiDataset in await dbContext.DigitalObjectIdentifiers.Where(i => i.DOIType == DOIType.Dataset).ToListAsync())
                {
                    await GenerateODPMetadataForDynamicDOI(doiDataset);
                }
                //var doiObservations = await dbContext.DigitalObjectIdentifiers.SingleAsync(i => i.DOIType == DOIType.ObservationsDb);
                //await GenerateODPMetadataForDynamicDOI(doiObservations);
                //foreach (var doiOrganisation in await dbContext
                //   .DigitalObjectIdentifiers
                //   .Where(i => i.DOIType == DOIType.Organisation && i.Code == "SAEON")
                //   .OrderBy(i => i.Name)
                //   .ToListAsync())
                //{
                //    await GenerateODPMetadataForDynamicDOI(doiOrganisation);
                //    foreach (var doiProgramme in await dbContext
                //        .DigitalObjectIdentifiers
                //        .Where(i => i.ParentId == doiOrganisation.Id)
                //        .OrderBy(i => i.Name)
                //        .ToListAsync())
                //    {
                //        await GenerateODPMetadataForDynamicDOI(doiProgramme);
                //        foreach (var doiProject in await dbContext
                //            .DigitalObjectIdentifiers
                //            .Where(i => i.ParentId == doiProgramme.Id)
                //            .OrderBy(i => i.Name)
                //            .ToListAsync())
                //        {
                //            await GenerateODPMetadataForDynamicDOI(doiProject);
                //            foreach (var doiSite in await dbContext
                //                .DigitalObjectIdentifiers
                //                .Where(i => i.ParentId == doiProject.Id)
                //                .OrderBy(i => i.Name)
                //                .ToListAsync())
                //            {
                //                await GenerateODPMetadataForDynamicDOI(doiSite);
                //                foreach (var doiStation in await dbContext
                //                    .DigitalObjectIdentifiers
                //                    .Where(i => i.ParentId == doiSite.Id)
                //                    .OrderBy(i => i.Name)
                //                    .ToListAsync())
                //                {
                //                    await GenerateODPMetadataForDynamicDOI(doiStation);
                //                    foreach (var doiDataset in await dbContext
                //                        .DigitalObjectIdentifiers
                //                        .Where(i => i.ParentId == doiStation.Id)
                //                        .OrderBy(i => i.Name)
                //                        .ToListAsync())
                //                    {
                //                        await GenerateODPMetadataForDynamicDOI(doiDataset);
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
            }

            var sb = new StringBuilder();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(config["ODPMetadataUrl"].AddTrailingForwardSlash());
                client.SetBearerToken(await GetTokenAsync());
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

            async Task<string> GetTokenAsync()
            {
                using (SAEONLogs.MethodCall(typeof(ODPMetadataHelper)))
                {
                    try
                    {
                        using (var client = new HttpClient() { BaseAddress = new Uri(config["AuthenticationServerUrl"].AddTrailingForwardSlash()) })
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
