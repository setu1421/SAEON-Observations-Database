using SAEON.Logs;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI
{
    public class LoggingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using (Logging.MethodCall(GetType()))
            {
                Logging.Verbose("Url: {Url} Headers: {Headers}", request.RequestUri.PathAndQuery, request.Headers.ToString());
                return await base.SendAsync(request, cancellationToken);
            }
        }
    }
}