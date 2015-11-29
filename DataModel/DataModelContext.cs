using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public partial class DataModelContext : IdentityDbContext<ApplicationUser>
    {
        public DataModelContext() : base("DefaultConnection")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled= false;

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataModelContext, DataModelMigrationConfiguration>());
        }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Game>().HasOptional(e => e.Arena).WithMany();
            //modelBuilder.Entity<Player>().HasOptional(e => e.Manager).WithMany();

            //modelBuilder.Entity<Person>()
            //    .HasMany(p => p.Roles)
            //    .WithMany()
            //    .Map(m => {
            //        m.ToTable("PersonRoles");
            //        m.MapLeftKey("PersonId");
            //        m.MapRightKey("RoleId");
            //    });
        }

        public DbSet<Person> Persons { get; set; }

        public DbSet<PlayerStatus> PlayerStatuses { get; set; }

        public DbSet<PlayerPosition> PlayerPositions { get; set; }

        public DbSet<UserType> UserTypes { get; set; }

        public DbSet<FileAttachment> FileAttachments { get; set; }

        public DbSet<Arena> Arenas { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<GameStatistic> GameStatistics { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<Note> Notes { get; set; }

        public DbSet<Season> Seasons{ get; set; }

        public DbSet<Setting> Settings{ get; set; }

        public DbSet<GameConfirmation> GameConfirmations { get; set; }
    }

}
