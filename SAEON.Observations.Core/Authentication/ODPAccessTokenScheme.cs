#if NETCOREAPP3_1
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
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
    public static class ODPAccessTokenAuthenticationDefaults
    {
        public const string AuthenticationScheme = "ODPAccessToken";
        public const string AllowedClientsPolicy = "ODPAllowedClientsAccessToken";
        public const string DeniedClientsPolicy = "ODPDeniedClientsAccessToken";
    }

    public class ODPAccessTokenAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string IntrospectionUrl { get; set; }
    }

    public class ODPAccessTokenAuthenticationPostConfigureOptions : IPostConfigureOptions<ODPAccessTokenAuthenticationOptions>
    {
        public void PostConfigure(string name, ODPAccessTokenAuthenticationOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrEmpty(options.IntrospectionUrl))
            {
                throw new InvalidOperationException("IntrospectionUrl must be provided in options");
            }
        }
    }

    public class ODPAccessTokenAuthenticationHandler : AuthenticationHandler<ODPAccessTokenAuthenticationOptions>
    {

        public ODPAccessTokenAuthenticationHandler(
            IOptionsMonitor<ODPAccessTokenAuthenticationOptions> options,
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
                        SAEONLogs.Error("ODPAccessTokenAuthorization Failed, no Access token");
                        return AuthenticateResult.Fail("No token");
                    }
                    SAEONLogs.Debug("Token: {Token}", token);
                    // Validate token
                    using (var client = new HttpClient())
                    {
                        client.SetBearerToken(token);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
                        using (var formContent = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("token", token) }))
                        {
                            var response = await client.PostAsync(new Uri(Options.IntrospectionUrl), formContent).ConfigureAwait(false);
                            if (!response.IsSuccessStatusCode)
                            {
                                SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                                SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                            }
                            response.EnsureSuccessStatusCode();
                            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            SAEONLogs.Information("Response: {Response}", json);
                            var jObj = JObject.Parse(json);
                            var isActive = jObj.Value<bool>("active");
                            if (!isActive)
                            {
                                SAEONLogs.Error("ODPAccessTokenAuthorization, invalid Access token {Token}", token);
                                return AuthenticateResult.Fail("Invalid Access token");
                            }
                            var clientId = jObj.Value<string>("client_id");
                            var claims = new List<Claim> { new Claim("ClientId", clientId), new Claim("ODPAccessToken", token) };
                            var identity = new ClaimsIdentity(claims, ODPAccessTokenAuthenticationDefaults.AuthenticationScheme);
                            var principal = new ClaimsPrincipal(identity);
                            var ticket = new AuthenticationTicket(principal, ODPAccessTokenAuthenticationDefaults.AuthenticationScheme);
                            SAEONLogs.Debug("ODPAccessTokenAuthentication succeeded");
                            return AuthenticateResult.Success(ticket);
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

    public static class ODPAccessTokenAuthenticationExtensions
    {
        public static AuthenticationBuilder AddODPAccessToken(this AuthenticationBuilder builder)
        {
            return AddODPAccessToken(builder, ODPAccessTokenAuthenticationDefaults.AuthenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddODPAccessToken(this AuthenticationBuilder builder, string authenticationScheme)
        {
            return AddODPAccessToken(builder, authenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddODPAccessToken(this AuthenticationBuilder builder, Action<ODPAccessTokenAuthenticationOptions> configureOptions)
        {
            return AddODPAccessToken(builder, ODPAccessTokenAuthenticationDefaults.AuthenticationScheme, configureOptions);
        }

        public static AuthenticationBuilder AddODPAccessToken(this AuthenticationBuilder builder, string authenticationScheme, Action<ODPAccessTokenAuthenticationOptions> configureOptions)
        {
            builder.Services.AddSingleton<IPostConfigureOptions<ODPAccessTokenAuthenticationOptions>, ODPAccessTokenAuthenticationPostConfigureOptions>();
            //builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<ODPAccessTokenAuthenticationOptions>, ODPAccessTokenAuthenticationPostConfigureOptions>());
            return builder.AddScheme<ODPAccessTokenAuthenticationOptions, ODPAccessTokenAuthenticationHandler>(authenticationScheme, configureOptions);
        }
    }
}
#endif