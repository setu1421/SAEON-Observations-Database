#if NETCOREAPP3_1
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SAEON.Logs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SAEON.Observations.Core.Authentication
{
    public static class ODPAuthenticationDefaults
    {
        public const string AuthenticationScheme = "ODP";
        public const string ConfigKeyIntrospectionUrl = "IntrospectionUrl";
    }

    public class ODPAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string IntrospectionUrl { get; set; }
        public bool RequireLogin { get; set; }
    }

    public class ODPAuthenticationPostConfigureOptions : IPostConfigureOptions<ODPAuthenticationOptions>
    {
        public void PostConfigure(string name, ODPAuthenticationOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrEmpty(options.IntrospectionUrl))
            {
                throw new InvalidOperationException("IntrospectionUrl must be provided in options");
            }
        }
    }

    public class ODPAuthenticationHandler : AuthenticationHandler<ODPAuthenticationOptions>
    {

        public ODPAuthenticationHandler(
            IOptionsMonitor<ODPAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    SAEONLogs.Debug("IntrospectionUrl: {IntrospectionUrl}", Options.IntrospectionUrl);
                    var token = Request.GetBearerToken();
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        SAEONLogs.Error("ODP Authorization Failed, no token");
                        return AuthenticateResult.Fail("No token");
                    }
                    SAEONLogs.Debug("Token: {Token}", token);
                    // Validate token
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
                        using (var formContent = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("token", token) }))
                        {
                            var response = await client.PostAsync(new Uri(Options.IntrospectionUrl), formContent);
                            if (!response.IsSuccessStatusCode)
                            {
                                SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                                SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync());
                            }
                            response.EnsureSuccessStatusCode();
                            var json = await response.Content.ReadAsStringAsync();
                            SAEONLogs.Information("Response: {Response}", json);
                            var jObject = JObject.Parse(json);
                            var isActive = jObject.Value<bool>("active");
                            if (!isActive)
                            {
                                SAEONLogs.Error("ODP Authorization Failed, invalid token {Token}", token);
                                return AuthenticateResult.Fail("Invalid token");
                            }
                            // If requireLogin make sure token is authentication token
                            if (Options.RequireLogin)
                            {

                            }
                        }
                    }
                    var claims = new[] { new Claim("ODPToken", token) };
                    var identity = new ClaimsIdentity(claims, ODPAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, ODPAuthenticationDefaults.AuthenticationScheme);
                    SAEONLogs.Debug("ODP authentication succeeded");
                    return AuthenticateResult.Success(ticket);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }
    }

    public static class ODPAuthenticationExtensions
    {
        public static AuthenticationBuilder AddODP(this AuthenticationBuilder builder)
        {
            return AddODP(builder, ODPAuthenticationDefaults.AuthenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddODP(this AuthenticationBuilder builder, string authenticationScheme)
        {
            return AddODP(builder, authenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddODP(this AuthenticationBuilder builder, Action<ODPAuthenticationOptions> configureOptions)
        {
            return AddODP(builder, ODPAuthenticationDefaults.AuthenticationScheme, configureOptions);
        }

        public static AuthenticationBuilder AddODP(this AuthenticationBuilder builder, string authenticationScheme, Action<ODPAuthenticationOptions> configureOptions) =>
            builder.AddScheme<ODPAuthenticationOptions, ODPAuthenticationHandler>(authenticationScheme, configureOptions);
    }
}
#endif