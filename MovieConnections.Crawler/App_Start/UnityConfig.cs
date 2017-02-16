using System;
using System.Linq;
using System.Reflection;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Practices.Unity;
using MovieConnections.Core.BaseServices;
using MovieConnections.Core.Model;
using MovieConnections.Data.IdentityServices;
using MovieConnections.Data.Models;
using MovieConnections.Framework.Repository;

namespace MovieConnections.Crawler
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
            // container.RegisterType<IProductRepository, ProductRepository>();

            container.RegisterType<IAuthenticationManager>(
                new InjectionFactory(x => HttpContext.Current.GetOwinContext().Authentication));

            container.RegisterType<ApplicationDbContext>("ApplicationDbContext", new TransientLifetimeManager());

            container.RegisterType<IUnitOfWork, UnitOfWork>(
                new InjectionConstructor(container.Resolve<ApplicationDbContext>("ApplicationDbContext")));

            container.RegisterType<IObjectSetFactory, ObjectSetFactory>(
                new InjectionConstructor(container.Resolve<ApplicationDbContext>("ApplicationDbContext")));

            var repositories =
                Assembly.Load("MovieConnections.Framework")
                    .GetTypes()
                    .Where(x => !x.IsInterface && !x.IsAbstract && x.Name == "Repository`1")
                    .ToList();

            container.RegisterTypes(
                repositories,
                WithMappings.FromMatchingInterface,
                WithName.Default,
                WithLifetime.Transient
                );

            container.RegisterType<ApplicationUserStore>(
                new InjectionConstructor(container.Resolve<ApplicationDbContext>("ApplicationDbContext")));
            container.RegisterType<ApplicationRoleStore>(
                new InjectionConstructor(container.Resolve<ApplicationDbContext>("ApplicationDbContext")));

            container.RegisterType<IUserStore<ApplicationUser, int>, ApplicationUserStore>(
                new InjectionConstructor(container.Resolve<ApplicationDbContext>("ApplicationDbContext")));
            container.RegisterType<IRoleStore<ApplicationRole, int>, ApplicationRoleStore>(
                new InjectionConstructor(container.Resolve<ApplicationDbContext>("ApplicationDbContext")));

            var types = Assembly.Load("MovieConnections.Core").GetTypes()
                .Where(x => !x.IsInterface && x.GetInterfaces().Contains(typeof(IDependency))).ToList();

            container.RegisterTypes(
                types,
                WithMappings.FromMatchingInterface,
                WithName.Default,
                WithLifetime.Transient);

            var culturedServices = Assembly.Load("MovieConnections.Core").GetTypes()
                .Where(x => !x.IsInterface && !x.IsAbstract && x.GetInterfaces().Contains(typeof(ICulturedDependency))).ToList();

            foreach (var type in culturedServices)
            {
                container.RegisterType(typeof(CulturedBaseService<DependencyEntity, DependencyCulturedEntity, ICulturedEntityParams>), type);

                container.RegisterType(typeof(ICulturedBaseService<,,>), type);
            }
        }
    }
}
