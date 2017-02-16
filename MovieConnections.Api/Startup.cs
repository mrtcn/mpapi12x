using System.Web.Http;
using AutoMapper;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using MovieConnections.Api;
using MovieConnections.Api.ViewModel;
using MovieConnections.Core.EntityParams;
using MovieConnections.Data.Entities;
using MovieConnections.Data.Models;
using MovieConnections.Framework.Extensions;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace MovieConnections.Api
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {

            Mapper.Initialize(cfg =>
                cfg.CreateMap<ConnectionPath, ConnectionPathParams>());
            Mapper.Initialize(cfg =>
                cfg.CreateMap<RegisterViewModel, ApplicationUser>().Ignore());

            Mapper.Configuration.AssertConfigurationIsValid();
            HttpConfiguration config = new HttpConfiguration();

            ConfigureOAuth(app);

            WebApiConfig.Register(config);
            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(config);
        }
    }
}