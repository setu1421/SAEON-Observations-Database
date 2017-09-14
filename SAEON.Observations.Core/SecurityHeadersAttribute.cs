using SAEON.Logs;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;

namespace SAEON.Observations.Core
{
    public class SecurityHeadersAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var result = context.Result;
            if (result is ViewResult)
            {
                if (!context.HttpContext.Response.Headers.AllKeys.Contains("X-Content-Type-Options"))
                //if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Type-Options"))
                {
                    context.HttpContext.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                }
                if (!context.HttpContext.Response.Headers.AllKeys.Contains("X-Frame-Options"))
                {
                    context.HttpContext.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                }

                var csp = ConfigurationManager.AppSettings["ContentSecurityPolicy"];
                //Logging.Verbose("ContentSecurityPolicy: {csp}", csp);
                // once for standards compliant browsers
                if (!context.HttpContext.Response.Headers.AllKeys.Contains("Content-Security-Policy"))
                {
                    context.HttpContext.Response.Headers.Add("Content-Security-Policy", csp);
                }
                // and once again for IE
                if (!context.HttpContext.Response.Headers.AllKeys.Contains("X-Content-Security-Policy"))
                {
                    context.HttpContext.Response.Headers.Add("X-Content-Security-Policy", csp);
                }
            }
        }
    }
}
