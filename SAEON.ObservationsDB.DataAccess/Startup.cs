using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SAEON.ObservationsDB.DataAccess.Startup))]
namespace SAEON.ObservationsDB.DataAccess
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
