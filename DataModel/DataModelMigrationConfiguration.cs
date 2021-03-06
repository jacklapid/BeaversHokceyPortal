﻿using Microsoft.AspNet.Identity;
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

            var adminUser = new ApplicationUser
            {
                UserName = Utilities.Constants.ADMIN,
                Email = Utilities.Constants.ADMIN_EMAIL,
            };

            var adminPerson = new Person
            {
                ApplicationUser_Id = adminUser.Id,
                FirstName = "Jack",
                LastName = "Lapid",
            };

            var managerUser = new ApplicationUser
            {
                UserName = "Manager",
                Email = "manager@yahoo.ca",
            };

            var managerPerson = new Manager
            {
                ApplicationUser_Id = managerUser.Id,
                FirstName = "Jesse",
                LastName = "Manager",
            };

            context.Persons.Add(adminPerson);

            context.Persons.Add(managerPerson);

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            userManager.Create(adminUser, "Qwe_123");

            userManager.Create(managerUser, "Qwe_123");

            foreach (var kvp in EnumHelper<Enums.PlayerStatusEnum>.ToDictionary())
            {
                context.PlayerStatuses.Add(new PlayerStatus { Id = kvp.Key, Name = kvp.Value });
            }

            foreach (var kvp in EnumHelper<Enums.PlayerPositionEnum>.ToDictionary())
            {
                context.PlayerPositions.Add(new PlayerPosition { Id = kvp.Key, Name = kvp.Value });
            }

            foreach (var kvp in EnumHelper<Enums.UserTypeEnum>.ToDictionary())
            {
                context.UserTypes.Add(new UserType { Id = kvp.Key, Name = kvp.Value });
            }

            foreach (var kvp in EnumHelper<Enums.EmailEventTypeEnum>.ToDictionary())
            {
                context.EmailEventTypes.Add(new EmailEventType { Id = kvp.Key, Name = kvp.Value });
            }

            foreach (var kvp in EnumHelper<Enums.RoleEnum>.ToDictionary())
            {
                var role = context.Roles.Add(new IdentityRole { Name = kvp.Value });

                if (kvp.Key == (int)Enums.RoleEnum.Admin || kvp.Key == (int)Enums.RoleEnum.Manager)
                {
                    adminUser.Roles.Add(new IdentityUserRole
                    {
                        RoleId = role.Id,
                        UserId = adminUser.Id
                    });

                    if (kvp.Key == (int)Enums.RoleEnum.Manager)
                    {
                        managerUser.Roles.Add(new IdentityUserRole
                        {
                            RoleId = role.Id,
                            UserId = managerUser.Id
                        });
                    }
                }
            }

            context.Arenas.Add(new Arena { Name = "Concordia", Address = "123 Sherbrooke" });

            adminPerson.UserType_Id = (int)Enums.UserTypeEnum.Admin;
            managerPerson.UserType_Id = (int)Enums.UserTypeEnum.Manager;

            context.Seasons.Add(new Season
            {
                Name = "2015 - 2016",
                StartDate = new DateTime(2015, 09, 01),
                EndDate = new DateTime(2016, 05, 01)
            });

            var beavers = new Team
            {
                Manager = managerPerson,
                Name = "Beavers"
            };
            context.Teams.Add(beavers);

            context.SaveChanges();
        }
    }
}
