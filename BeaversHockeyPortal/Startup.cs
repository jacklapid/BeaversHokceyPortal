using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System.Net;

[assembly: OwinStartupAttribute(typeof(BeaversHockeyPortal.Startup))]
namespace BeaversHockeyPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
