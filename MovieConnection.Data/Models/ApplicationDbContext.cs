using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MovieConnections.Data.Models {

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public static ApplicationDbContext Create() 
        {
            return DependencyResolver.Current.GetService<ApplicationDbContext>();
        }

        public ApplicationDbContext() : base("ApplicationDbContext") {
            Configuration.ProxyCreationEnabled = true;
            Configuration.LazyLoadingEnabled = true;
            Configuration.AutoDetectChangesEnabled = false;
            Configuration.ValidateOnSaveEnabled = false;
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<ApplicationUser>().Property(x => x.FirstName).HasMaxLength(255).IsRequired();
            modelBuilder.Entity<ApplicationUser>().Property(x => x.LastName).HasMaxLength(255).IsRequired();

            //modelBuilder.Entity<ApplicationRole>().Property(r => r.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            //var entityTypes = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.BaseType != null
            //&& x.BaseType.IsGenericType
            //&& x.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>)
            //&& x.BaseType.GenericTypeArguments.Any(y => y.BaseType.IsAssignableFrom(typeof(IEntity))));

            //foreach (var entityType in entityTypes)
            //{
            //    modelBuilder.Configurations.Add((dynamic)Activator.CreateInstance(entityType));
            //}

            modelBuilder.Configurations.AddFromAssembly(GetType().Assembly);

            //base.OnModelCreating(modelBuilder);
        }
    }
}