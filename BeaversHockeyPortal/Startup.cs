using Microsoft.Owin;
using Owin;

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
