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
        IQueryable<Player> GetPlayersForManager(int managerId);

        IQueryable<Manager> GetManagers();

        Manager GetManagerById(int managerId);

        Manager GetManagerByUserId(string userId);


        Team GetTeamById(int teamId);

        IQueryable<Team> GetTeamsForManager(int managerId);

        IQueryable<Team> GetTeams();

        bool CreateTeam(string teamName, int? fileAttachmentId, Manager manager);


        Game GetGameById(int gameId);

        IQueryable<Game> GetGames();

        IQueryable<Game> GetGamesForManager(int managerId);

        bool CreateGame(int arenaId, DateTime date, Manager manager, int usId, int themId, int seasonId);


        IQueryable<Arena> GetArenas();

        Arena GetArenaById(int ArenaId);

        IQueryable<IdentityRole> GetRoles();

        Person GetPersonByUserId(string userId);

        Player GetPlayerByUserId(string userId);

        string GetSettingValueByKey(string key);

        bool ConfirmGame(int gameId, int playerId);

        bool UnconfirmGame(int gameId, int playerId);

        bool SavePerson(Person person);

        IQueryable<Season> GetSeasons();

        Dictionary<int, bool> GetPlayerGameConfirmationStatuses(int playerId, IEnumerable<int> gameIds);

        bool GetPlayerGameConfirmationStatus(int playerId, int gameId);
    }
}
