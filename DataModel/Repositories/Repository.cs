using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace DataModel.Repositories
{
    public class Repository : IRepository
    {
        private DataModelContext _ctx;

        public Repository(DataModelContext ctx)
        {
            _ctx = ctx;
        }

        public IQueryable<Manager> GetManagers()
        {
            return _ctx.Persons.OfType<Manager>();
        }

        public IQueryable<Person> GetPersonsByUserType(Enums.UserTypeEnum userType)
        {
            return _ctx.Persons.Where(p => p.UserType_Id == (int)userType);
        }

        public Manager GetManagerById(int managerId)
        {
            return _ctx.Persons.OfType<Manager>().FirstOrDefault(m => m.Id == managerId);
        }

        public Manager GetManagerByUserId(string userId)
        {
            return _ctx.Persons.OfType<Manager>().FirstOrDefault(m => m.ApplicationUser_Id == userId);
        }

        public IQueryable<Player> GetPlayersForManager(int managerId)
        {
            return _ctx.Persons.OfType<Player>()
    .Include(p => p.Team)
                   .Where(p => p.Manager.Id == managerId);
        }

        public IQueryable<IdentityRole> GetRoles()
        {
            return _ctx.Roles;
        }

        public Player GetPlayerByUserId(string userId)
        {
            return this._ctx.Persons.OfType<Player>().Include(p => p.Manager).FirstOrDefault(p => p.ApplicationUser_Id == userId);
        }


        public Person GetPersonByUserId(string userId)
        {
            return this._ctx.Persons.FirstOrDefault(person => person.ApplicationUser_Id == userId);
        }

        public Team GetTeamById(int teamId)
        {
            return _ctx.Teams.FirstOrDefault(t => t.Id == teamId);
        }

        public IQueryable<Team> GetTeams()
        {
            return _ctx.Teams;
        }

        public IQueryable<Team> GetTeamsForManager(int managerId)
        {
            return _ctx.Teams.Where(t => t.Manager != null && t.Manager.Id == managerId);
        }

        public bool CreateTeam(string teamName, int? fileAttachmentId, Manager manager)
        {
            try
            {
                var newTeam = new Team
                {
                    Name = teamName,
                    Manager = manager,
                };

                if (fileAttachmentId.HasValue)
                {
                    newTeam.ImageIds.Add(fileAttachmentId.Value);
                }

                _ctx.Teams.Add(newTeam);

                _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }

            return true;
        }

        public Game GetGameById(int gameId)
        {
            return _ctx.Games.FirstOrDefault(x => x.Id == gameId);
        }

        public IQueryable<Game> GetGames()
        {
            return _ctx.Games;
        }

        public IQueryable<Game> GetGamesForManager(int managerId)
        {
            return _ctx.Games.Include(g => g.Arena)
                .Include(g => g.Season)
                .Include(g => g.Them)
                .Where(x => x.Manager != null && x.Manager.Id == managerId);
        }

        public bool CreateGame(int arenaId, DateTime date, Manager manager, int usId, int themId, int seasonId)
        {
            try
            {
                var game = new Game
                {
                    Arena = this.GetArenaById(arenaId),
                    Date = date,
                    Manager = manager,
                    Us = this.GetTeamById(usId),
                    Them = this.GetTeamById(themId),
                    Season = _ctx.Seasons.FirstOrDefault(s => s.Id == seasonId)
                };

                _ctx.Games.Add(game);

                _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }

            return true;
        }

        public IQueryable<Arena> GetArenas()
        {
            return _ctx.Arenas;
        }

        public Arena GetArenaById(int ArenaId)
        {
            return _ctx.Arenas.FirstOrDefault(a => a.Id == ArenaId);
        }

        public string GetSettingValueByKey(string key)
        {
            return _ctx.Settings.First(s => s.Key == key).Value;
        }

        public bool ConfirmGame(int gameId, int playerId)
        {
            return this.ConfirmOrUnconfirmPlayer(gameId, playerId, true);
        }

        public bool UnconfirmGame(int gameId, int playerId)
        {
            return this.ConfirmOrUnconfirmPlayer(gameId, playerId, false);

        }
        private bool ConfirmOrUnconfirmPlayer(int gameId, int playerId, bool confirming)
        {
            var confirmedGame = _ctx.GameConfirmations.FirstOrDefault(gc => gc.Game_Id == gameId && gc.Player_Id == playerId);

            if (confirming && confirmedGame == null)
            {
                _ctx.GameConfirmations.Add(new GameConfirmation
                {
                    Game_Id = gameId,
                    Player_Id = playerId
                });
            }
            else if (!confirming && confirmedGame != null)
            {
                _ctx.GameConfirmations.Remove(confirmedGame);
            }
            else
            {
                return false;
            }

            _ctx.SaveChanges();

            return true;
        }

        public bool SavePerson(Person person)
        {
            _ctx.Persons.Add(person);

            _ctx.SaveChanges();

            return true;
        }

        public IQueryable<Season> GetSeasons()
        {
            return _ctx.Seasons;
        }

        public Dictionary<int, bool> GetPlayerGameConfirmationStatuses(int playerId, IEnumerable<int> gameIds)
        {
            var dic = new Dictionary<int, bool>();

            var gamesConfirmedByPlayer = _ctx.GameConfirmations.Where(gc => gc.Player_Id == playerId && gameIds.Contains(gc.Game_Id)).ToList();

            foreach (var gameId in gameIds)
            {
                dic[gameId] = gamesConfirmedByPlayer.Any(gc => gc.Game_Id == gameId);
            }

            return dic;
        }

        public bool GetPlayerGameConfirmationStatus(int playerId, int gameId)
        {
            return _ctx.GameConfirmations.Any(gc => gc.Player_Id == playerId && gc.Game_Id == gameId);
        }

        public IQueryable<EmailTemplate> GetEmailTemplatesForManager(int managerId)
        {
            return _ctx.EmailTemplates.Where(et => et.Manager.Id == managerId);
        }

        public bool CreateEmailTemplate(int[] toUserIds, int[] toPlayerStatusIds, int[] toUserTypeIds, string from, string subject, string body, int managerId)
        {
            var emailTemplate = new EmailTemplate
            {
                ToPersons = toUserIds != null ? _ctx.Persons.Where(p => toUserIds.Contains(p.Id)).ToList() : new List<Person>(),
                ToPlayerStatuses = toPlayerStatusIds != null ? _ctx.PlayerStatuses.Where(ps => toPlayerStatusIds.Contains(ps.Id)).ToList() : new List<PlayerStatus>(),
                ToUserTypes = toUserTypeIds != null ? _ctx.UserTypes.Where(ut => toUserTypeIds.Contains(ut.Id)).ToList() : new List<UserType>(),
                From = from,
                Subject = subject,
                Body = body,
                Manager = this.GetManagerById(managerId)
            };

            _ctx.EmailTemplates.Add(emailTemplate);

            _ctx.SaveChanges();

            return true;
        }

        public PlayerRegistration GetPlayerToRegisterByToken(string token)
        {
            return _ctx.PlayerRegistrations.Include(p => p.Manager).Include(p => p.Team).FirstOrDefault(x => x.Token == token);
        }

        public bool CreatePlayerRegistration(string email, int managerId, int teamId)
        {
            var newPlayerRegistration = new PlayerRegistration
            {
                PlayerEmail = email,
                Token = Guid.NewGuid().ToString(),
                TokenAlreadyUsed = false,
                TokenGeneratedOn = DateTime.Now,
                Manager = this.GetManagerById(managerId),
                Team = this.GetTeamById(teamId)
            };

            _ctx.PlayerRegistrations.Add(newPlayerRegistration);

            _ctx.SaveChanges();

            return true;
        }
    }
}