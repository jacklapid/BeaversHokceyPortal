using DataModel;
using DataModel.Repositories;

using System.Collections.Generic;
using System.Linq;

namespace BeaversHockeyPortal.Controllers
{
    public static class ControllerHelper
    {
        public static IEnumerable<Manager> GetManagersInScope(string userId, IRepository repo)
        {
            var managers = new List<Manager>();

            var person = repo.GetPersonById(userId);

            if (person != null)
            {
                if (person.UserType_Id == (int)DataModel.Enums.UserTypeEnum.Admin)
                {
                    managers = repo.GetManagers().ToList();
                }
                else if (person.UserType_Id == (int)DataModel.Enums.UserTypeEnum.Manager)
                {
                    managers.Add(repo.GetManagerById(userId));
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
    }
}
