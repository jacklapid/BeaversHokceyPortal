using DataModel.Language;
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
            this.Configuration.ProxyCreationEnabled = false;

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataModelContext, DataModelMigrationConfiguration>());
        }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EmailEvent>()
                .HasMany(et => et.EmailTemplates)
                .WithMany()
                .Map(m =>
                {
                    m.ToTable("EmailEventEmailTemplate");
                    m.MapLeftKey("EmailEvent_Id");
                    m.MapRightKey("EmailTemplate_Id");
                });

            modelBuilder.Entity<EmailEvent>()
    .HasMany(et => et.EmailEventTypes)
    .WithMany()
    .Map(m =>
    {
        m.ToTable("EmailEventEmailEventType");
        m.MapLeftKey("EmailEvent_Id");
        m.MapRightKey("EmailEventType_Id");
    });

            modelBuilder.Entity<EmailTemplate>()
                .HasMany(et => et.ToPersons)
                .WithMany()
                .Map(m =>
                {
                    m.ToTable("EmailTemplateToPerson");
                    m.MapLeftKey("EmailTemplate_Id");
                    m.MapRightKey("ToPerson_Id");
                });

            modelBuilder.Entity<EmailTemplate>()
    .HasMany(et => et.ToPlayerStatuses)
    .WithMany()
    .Map(m =>
    {
        m.ToTable("EmailTemplateToPlayerStatus");
        m.MapLeftKey("EmailTemplate_Id");
        m.MapRightKey("ToPlayerStatus_Id");
    });

            modelBuilder.Entity<EmailTemplate>()
.HasMany(et => et.ToUserTypes)
.WithMany()
.Map(m =>
{
    m.ToTable("EmailTemplateToUserType");
    m.MapLeftKey("EmailTemplate_Id");
    m.MapRightKey("ToUserType_Id");
});
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

        public DbSet<Season> Seasons { get; set; }

        //public DbSet<Setting> Settings { get; set; }

        public DbSet<GameConfirmation> GameConfirmations { get; set; }

        public DbSet<EmailTemplate> EmailTemplates { get; set; }

        public DbSet<EmailEvent> EmailEvents { get; set; }

        public DbSet<EmailEventType> EmailEventTypes { get; set; }

        public DbSet<PlayerRegistration> PlayerRegistrations { get; set; }
    }
}
