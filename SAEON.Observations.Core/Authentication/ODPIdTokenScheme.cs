#if NETCOREAPP3_1
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SAEON.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SAEON.Observations.Core.Authentication
{
    public static class ODPIdTokenAuthenticationDefaults
    {
        public const string AuthenticationScheme = "ODPIdToken";
    }

    public class ODPIdTokenAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string IntrospectionUrl { get; set; }
    }

    public class ODPIdTokenAuthenticationPostConfigureOptions : IPostConfigureOptions<ODPIdTokenAuthenticationOptions>
    {
        public void PostConfigure(string name, ODPIdTokenAuthenticationOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrEmpty(options.IntrospectionUrl))
            {
                throw new InvalidOperationException("IntrospectionUrl must be provided in options");
            }
        }
    }

    public class ODPIdTokenAuthenticationHandler : AuthenticationHandler<ODPIdTokenAuthenticationOptions>
    {

        public ODPIdTokenAuthenticationHandler(
            IOptionsMonitor<ODPIdTokenAuthenticationOptions> options,
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
                        SAEONLogs.Error("ODPIdTokenAuthorization Failed, no Id token");
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
                            if ((!isActive) || (jObj["ext"] == null))
                            {
                                SAEONLogs.Error("ODPIdTokenAuthorization, invalid Id token {Token}", token);
                                return AuthenticateResult.Fail("Invalid Id token");
                            }
                            var clientId = jObj.Value<string>("client_id");
                            var userId = jObj["ext"].Value<string>("user_id");
                            var userEmail = jObj["ext"].Value<string>("email");
                            var userRoles = from r in jObj["ext"]["access_rights"] select (string)r["role_name"];
                            SAEONLogs.Debug("User Id: {Id} Email: {Email}, Roles: {Role}", userId, userEmail, userRoles);
                            var claims = new List<Claim> {
                                new Claim("ClientId",clientId),
                                new Claim("ODPIdToken", token),
                                new Claim(ClaimTypes.NameIdentifier,userId),
                                new Claim(ClaimTypes.Email,userId)
                            };
                            foreach (var userRole in userRoles)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, userRole));
                            }
                            var identity = new ClaimsIdentity(claims, ODPIdTokenAuthenticationDefaults.AuthenticationScheme);
                            var principal = new ClaimsPrincipal(identity);
                            var ticket = new AuthenticationTicket(principal, ODPIdTokenAuthenticationDefaults.AuthenticationScheme);
                            SAEONLogs.Debug("ODPIdTokenAuthentication succeeded");
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

    public static class ODPIdTokenAuthenticationExtensions
    {
        public static AuthenticationBuilder AddODPIdToken(this AuthenticationBuilder builder)
        {
            return AddODPIdToken(builder, ODPIdTokenAuthenticationDefaults.AuthenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddODPIdToken(this AuthenticationBuilder builder, string authenticationScheme)
        {
            return AddODPIdToken(builder, authenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddODPIdToken(this AuthenticationBuilder builder, Action<ODPIdTokenAuthenticationOptions> configureOptions)
        {
            return AddODPIdToken(builder, ODPIdTokenAuthenticationDefaults.AuthenticationScheme, configureOptions);
        }

        public static AuthenticationBuilder AddODPIdToken(this AuthenticationBuilder builder, string authenticationScheme, Action<ODPIdTokenAuthenticationOptions> configureOptions)
        {
            builder.Services.AddSingleton<IPostConfigureOptions<ODPIdTokenAuthenticationOptions>, ODPIdTokenAuthenticationPostConfigureOptions>();
            //builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<ODPIdTokenAuthenticationOptions>, ODPIdTokenAuthenticationPostConfigureOptions>());
            return builder.AddScheme<ODPIdTokenAuthenticationOptions, ODPIdTokenAuthenticationHandler>(authenticationScheme, configureOptions);
        }
    }
}
#endif