using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SAEON.ObservationsDB.WebSite.Startup))]
namespace SAEON.ObservationsDB.WebSite
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
