using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SAEON.Logs;
using SAEON.Observations.Auth;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI
{
    public static class ODPMetadataHelper
    {
        public static async Task<string> CreateMetadata(ObservationsDbContext dbContext, IConfiguration config)
        {
            async Task GenerateODPMetadata(HttpClient client)
            {
                async Task GenerateODPMetadataForDOI(DigitalObjectIdentifier doi)
                {
                    AddLine($"{doi.DOIType} {doi.Name}");
                    var jObj = new JObject(
                        new JProperty("collection_key", "observations-database-dynamic-collection"),
                        new JProperty("schema_key", "saeon-odp-4-2"),
                        new JProperty("metadata", JObject.Parse(doi.MetadataJson)),
                        new JProperty("terms_conditions_accepted", true),
                        new JProperty("data_agreement_accepted", true),
                        new JProperty("data_agreement_url", "https://observations.saeon.ac.za/DataUsage"),
                        new JProperty("capture_method", "harvester")
                        );
                    var response = await client.PostAsync("metadata", new StringContent(jObj.ToString(), Encoding.UTF8, MediaTypeNames.Application.Json));
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
                    var errors = jODP["errors"];
                    doi.ODPMetadataId = new Guid(odpId);
                    doi.ODPMetadataIsValid = validated && errors.HasValues;
                    await dbContext.SaveChangesAsync();
                }

                AddLine("Generating metadata");
                var doiObservations = await dbContext.DigitalObjectIdentifiers.Include(i => i.Children).SingleAsync(i => i.DOIType == DOIType.ObservationsDb);
                await GenerateODPMetadataForDOI(doiObservations);
            }

            var sb = new StringBuilder();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(config["ODPMetadataUrl"]);
                client.SetBearerToken(await GetTokenAsync());
                await GenerateODPMetadata(client);
                AddLine("Done");
                return sb.ToString();
            }

            void AddLine(string line)
            {
                sb.AppendLine(line);
                SAEONLogs.Information(line);
            }

            async Task<string> GetTokenAsync()
            {
                using (SAEONLogs.MethodCall(typeof(ODPMetadataHelper)))
                {
                    try
                    {
                        using (var client = new HttpClient())
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
                                var response = await client.PostAsync(config["AuthenticationServerUrl"] + "/oauth2/token", formContent);
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
