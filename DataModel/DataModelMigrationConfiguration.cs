using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Utilities;

namespace DataModel
{
    public class DataModelMigrationConfiguration : DbMigrationsConfiguration<DataModelContext>
    {

        public DataModelMigrationConfiguration()
        {
            this.AutomaticMigrationDataLossAllowed = true;
            this.AutomaticMigrationsEnabled = true;

        }

        protected override void Seed(DataModelContext context)
        {
            base.Seed(context);

            if (context.Persons.Count() > 0)
            {
                return;
            }

            var adminUser = new Person
            {
                UserName = Utilities.Constants.ADMIN_EMAIL,
                Email = Utilities.Constants.ADMIN_EMAIL,
                FirstName = "Jack",
                LastName = "Lapid",
            };

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            userManager.Create(adminUser, "Abc_123");

            foreach (var kvp in EnumHelper.ToDictionary(typeof(Enums.PlayerStatusEnum)))
            {
                context.PlayerStatuses.Add(new PlayerStatus { Id = kvp.Key, Name = kvp.Value });
            }

            foreach (var kvp in EnumHelper.ToDictionary(typeof(Enums.PlayerPositionEnum)))
            {
                context.PlayerPositions.Add(new PlayerPosition { Id = kvp.Key, Name = kvp.Value });
            }

            foreach (var kvp in EnumHelper.ToDictionary(typeof(Enums.UserTypeEnum)))
            {
                context.UserTypes.Add(new UserType { Id = kvp.Key, Name = kvp.Value });
            }

            foreach (var kvp in EnumHelper.ToDictionary(typeof(Enums.RoleEnum)))
            {
                var role = context.Roles.Add(new IdentityRole { Name = kvp.Value });

                if (kvp.Key == (int)Enums.RoleEnum.Admin || kvp.Key == (int)Enums.RoleEnum.Manager)
                {
                    adminUser.Roles.Add(new IdentityUserRole
                    {
                        RoleId = role.Id,
                        UserId = adminUser.Id
                    });
                }
            }

            adminUser.UserType_Id = (int)Enums.UserTypeEnum.Admin;

            context.Teams.Add(new Team { Name = "Beavers" });

            context.Arenas.Add(new Arena { Name = "Concordia" });

            context.SaveChanges();
        }
    }
}
