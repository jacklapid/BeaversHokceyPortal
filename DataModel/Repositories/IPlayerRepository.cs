using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Repositories
{
    public interface IPlayerRepository
    {
        IQueryable<Player> GetPlayersForManager(string managerId);

        IQueryable<Manager> GetManagers();

        IQueryable<IdentityRole> GetRoles();

        Person GetPersonById(string userId);
    }
}
