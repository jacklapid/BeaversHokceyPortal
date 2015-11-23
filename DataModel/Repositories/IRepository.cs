using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Repositories
{
    public interface IRepository
    {
        IQueryable<Player> GetPlayersForManager(string managerId);

        IQueryable<Manager> GetManagers();

        Manager GetManagerById(string managerId);


        Team GetTeamById(int teamId);

        IQueryable<Team> GetTeamsForManager(string managerId);

        IQueryable<Team> GetTeams();

        bool CreateTeam(string teamName, int? fileAttachmentId, Manager manager);


        Game GetGameById(int gameId);

        IQueryable<Game> GetGames();

        IQueryable<Game> GetGamesForManager(string managerId);

        bool CreateGame(Arena arena, DateTime date, Manager manager, Team us, Team them);


        IQueryable<Arena> GetArenas();

        Arena GetArenaById(int ArenaId);

        IQueryable<IdentityRole> GetRoles();

        Person GetPersonById(string userId);

        string GetSettingValueByKey(string key);

        bool ConfirmGame(int gameId, string userId);

        bool UnconfirmGame(int gameId, string userId);
    }
}
