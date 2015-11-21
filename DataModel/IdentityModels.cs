using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Migrations;

namespace DataModel
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public partial class DataModelContext : IdentityDbContext<ApplicationUser>
    {
        //public ApplicationDbContext()
        //    : base("DefaultConnection", throwIfV1Schema: false)
        //{
        //    Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, ApplicationMigrationConfiguration>());
        //}

        public static DataModelContext Create()
        {
            return new DataModelContext();
        }
    }

    //public class ApplicationMigrationConfiguration : DbMigrationsConfiguration<ApplicationDbContext>
    //{
    //    public ApplicationMigrationConfiguration()
    //    {
    //        this.AutomaticMigrationDataLossAllowed = true;
    //        this.AutomaticMigrationsEnabled = true;
    //    }

    //    protected override void Seed(ApplicationDbContext context)
    //    {
    //        base.Seed(context);

            
    //    }
    //}
}