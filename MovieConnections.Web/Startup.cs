using Microsoft.Owin;
using MovieConnections.Web;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace MovieConnections.Web
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}