using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SAEON.Observations.WebSite.Query.Startup))]
namespace SAEON.Observations.WebSite.Query
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
