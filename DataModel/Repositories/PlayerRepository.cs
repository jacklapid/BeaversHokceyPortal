using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DataModel.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private DataModelContext _ctx;

        public PlayerRepository(DataModelContext ctx)
        {
            _ctx = ctx;
        }

        public IQueryable<Manager> GetManagers()
        {
            return _ctx.Persons.OfType<Manager>();
        }

        public Manager GetManagerById(string managerId)
        {
            return _ctx.Persons.OfType<Manager>().FirstOrDefault(m => m.Id == managerId);
        }

        public IQueryable<Player> GetPlayersForManager(string managerId)
        {

            return _ctx.Persons.OfType<Player>()
                               .Where(p => p.Manager.Id == managerId);
        }

        public IQueryable<IdentityRole> GetRoles()
        {
            return _ctx.Roles;
        }

        public Person GetPersonById(string userId)
        {
            return this._ctx.Persons.FirstOrDefault(person => person.Id == userId);
        }

        public Team GeTeamById(int teamId)
        {
            return _ctx.Teams.FirstOrDefault(t => t.Id == teamId);
        }
    }
}
