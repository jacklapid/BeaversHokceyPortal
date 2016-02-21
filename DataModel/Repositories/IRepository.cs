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

        IQueryable<Person> GetPersonsByUserType(Enums.UserTypeEnum userType);

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

        //string GetSettingValueByKey(string key);

        bool ConfirmGame(int gameId, int playerId);

        bool UnconfirmGame(int gameId, int playerId);

        bool SavePerson(Person person);

        IQueryable<Season> GetSeasons();

        Dictionary<int, bool> GetPlayerGameConfirmationStatuses(int playerId, IEnumerable<int> gameIds);

        bool CreateEmailTemplateToPredefiniedUsers(int[] toUserIds, int[] toPlayerStatusIds, int[] toUserTypeIds, string from, string subject, string body, string context, int managerId);

        bool CreateEmailTemplate(string to, string from, string subject, string body, string context, int managerId);
        bool GetPlayerGameConfirmationStatus(int playerId, int gameId);

        IQueryable<EmailTemplate> GetEmailTemplatesForManager(int managerId);

        PlayerRegistration GetPlayerToRegisterByToken(string token);
        string CreatePlayerRegistration(string email, int managerId, int teamId);
        IQueryable<ApplicationUser> GetAllRegistredUsers();
    }
}
