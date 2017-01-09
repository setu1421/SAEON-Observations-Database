using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SAEON.Observations.QuerySite.Startup))]
namespace SAEON.Observations.QuerySite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
