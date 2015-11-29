using DataModel;
using DataModel.Repositories;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;

namespace BeaversHockeyPortal.Controllers
{
    public static class ControllerHelper
    {
        public static IEnumerable<IdentityRole> GetRolesInScope(string userId, IRepository repo)
        {
            var person = repo.GetPersonByUserId(userId);

            if (person != null)
            {
                if (person.UserType_Id == (int)DataModel.Enums.UserTypeEnum.Admin)
                {
                    return repo.GetRoles().ToList();
                }
                else if (person.UserType_Id == (int)DataModel.Enums.UserTypeEnum.Manager)
                {
                    return repo.GetRoles().Where(r => r.Name == DataModel.Enums.UserTypeEnum.Player.ToString());
                }
            }

            return new List<IdentityRole>();
        }
        public static IEnumerable<Manager> GetManagersInScope(string userId, IRepository repo)
        {
            var managers = new List<Manager>();

            var person = repo.GetPersonByUserId(userId);

            if (person != null)
            {
                if (person.UserType_Id == (int)DataModel.Enums.UserTypeEnum.Admin)
                {
                    managers = repo.GetManagers().ToList();
                }
                else if (person.UserType_Id == (int)DataModel.Enums.UserTypeEnum.Manager)
                {
                    managers.Add(repo.GetManagerByUserId(userId));
                }
                else if (person.UserType_Id == (int)DataModel.Enums.UserTypeEnum.Player)
                {
                    var player = repo.GetPlayerByUserId(userId);
                    managers.Add(player.Manager);
                }
            }

            return managers;
        }

        public static IEnumerable<Team> GetTeamsInScope(string userId, IRepository repo)
        {
            var managers = GetManagersInScope(userId, repo);

            return managers
                .SelectMany(m => repo.GetTeamsForManager(m.Id))
                .Where(t => t != null)
                .Distinct()
                .ToList();
        }

        public static IEnumerable<Game> GetGamesInScope(string userId, IRepository repo)
        {
            var managers = GetManagersInScope(userId, repo);

            return managers
                .SelectMany(m => repo.GetGamesForManager(m.Id))
                .Where(g => g != null)
                .Distinct()
                .ToList();
        }

        public static IEnumerable<Player> GetPlayersInScope(string userId, IRepository repo)
        {
            var managers = GetManagersInScope(userId, repo);

            return managers
                .SelectMany(m => repo.GetPlayersForManager(m.Id))
                .Where(p => p != null)
                .Distinct()
                .ToList();
        }
    }
}
