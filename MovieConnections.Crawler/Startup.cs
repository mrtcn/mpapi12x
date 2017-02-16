using Microsoft.Owin;
using MovieConnections.Crawler;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace MovieConnections.Crawler
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
