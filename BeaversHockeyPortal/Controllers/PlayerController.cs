using BeaversHockeyPortal.Models;
using DataModel;
using DataModel.Enums;
using DataModel.Repositories;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BeaversHockeyPortal.WebUtilities;
using Utilities;
using EmailModule;
using System.Transactions;

namespace BeaversHockeyPortal.Controllers
{
    [Authorize]
    public class PlayerController : AuthorizedController
    {
        private const string SESSION_FILTER_STATUS = "statusFilter";
        private const string SESSION_FILTER_POSITION = "positionFilter";
        private const string SESSION_FILTER_DISPLAYENTRIES = "displayEntriesFilter";
        private const string SESSION_FILTER_PAGE = "pageFilter";

        public PlayerController(IRepository playerRepository) : base(playerRepository)
        {
        }

        public ActionResult ConfirmGames()
        {
            return View(GetGamesOpenForConfirmation());
        }

        public ActionResult ConfirmGame(int id = 0, bool confirming = false)
        {
            var game = this._Repo.GetGameById(id);
            if (game == null)
            {
                return HttpNotFound();
            }

            var userId = this.User.Identity.GetUserId();
            var player = this._Repo.GetPersonByUserId(userId) as Player;

            if (player == null)
            {
                ModelState.AddModelError("", $"Only a player can confirm the game");
            }
            else
            {
                var isGameConfirmed = this._Repo.GetPlayerGameConfirmationStatus(player.Id, id);

                if (confirming && isGameConfirmed)
                {
                    ModelState.AddModelError("", $"Error Confirming: Player '{player.FullName}' already confirmed this game");
                }
                else if (!confirming && !isGameConfirmed)
                {
                    ModelState.AddModelError("", $"Error Unconfirming: Player '{player.FullName}' not registered for this game");
                }
                else
                {
                    if (confirming)
                    {
                        this._Repo.ConfirmGame(id, player.Id);
                    }
                    else
                    {
                        this._Repo.UnconfirmGame(id, player.Id);
                    }
                    return RedirectToAction("ConfirmGames", "Player");
                }
            }

            return View(GetGamesOpenForConfirmation());
        }

        // GET: Player
        public ActionResult Players(PlayerModels theModel)
        {
            //http://localhost:59665/Player/Players?displayEntries=1&status=1&position=2&page=0

            int status = this.GetFilterValue<int>(theModel.PlayerStatusFilter, SESSION_FILTER_STATUS);
            int position = this.GetFilterValue<int>(theModel.PlayerPositionFilter, SESSION_FILTER_POSITION);

            var displayEntries = 0;
            var page = 0;

            Session[SESSION_FILTER_STATUS] = status;
            Session[SESSION_FILTER_POSITION] = position;
            Session[SESSION_FILTER_DISPLAYENTRIES] = displayEntries;
            Session[SESSION_FILTER_PAGE] = page;

            var userID = User.Identity.GetUserId();

            var playersInScope = ControllerHelper.GetPlayersInScope(userID, this._Repo);

            if (status > 0)
            {
                playersInScope = playersInScope.Where(player => player.PlayerStatus_Id == status);
            }

            if (position > 0)
            {
                playersInScope = playersInScope.Where(player => player.PlayerPosition_Id == position);
            }

            if (displayEntries > 0)
            {
                if ((page - 1) > 0)
                {
                    playersInScope = playersInScope.Skip(displayEntries * page);
                }

                playersInScope = playersInScope.Take(displayEntries);
            }

            var playerUserIds = playersInScope.Select(p => p.ApplicationUser_Id).ToList();

            var playerUsers = _Repo.GetAllRegistredUsers()
                .Where(user => playerUserIds.Contains(user.Id))
                .Select(user => new
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                })
                .ToList();

            var players = playersInScope
                .ToList()
                .OrderBy(p => p.FullName)
                .Select(p => new PlayerViewModels
                {
                    Username = playerUsers.First(u => u.Id == p.ApplicationUser_Id).UserName,
                    Email = playerUsers.First(u => u.Id == p.ApplicationUser_Id).Email,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Position = Enum.GetName(typeof(PlayerPositionEnum), p.PlayerPosition_Id),
                    Status = Enum.GetName(typeof(PlayerStatusEnum), p.PlayerStatus_Id),
                    PositionId = p.PlayerPosition_Id,
                    Team = p.Team != null ? p.Team.Name : string.Empty,
                    Manageer = p.Manager != null ? p.Manager.FullName : string.Empty
                })
                .ToList();

            var model = new PlayerModels
            {
                Players = players,
                PlayerStatusFilter = status.ToString(),
                PlayerPositionFilter = position.ToString(),
            };

            return View(model);
        }

        [Authorize(Roles = "Manager, Admin")]
        public ActionResult CreatePlayerRegistration()
        {
            var model = new PlayerRegistrationViewModel();

            this.PopulateOptions(model);

            return View(model);
        }

        [Authorize(Roles = "Manager, Admin")]
        [HttpPost]
        public ActionResult CreatePlayerRegistration(PlayerRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var success = false;

                var token = this._Repo.CreatePlayerRegistration(
model.Email,
model.ManagerId,
model.TeamId);

                using (TransactionScope transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    success = !string.IsNullOrWhiteSpace(token);

                    if (success)
                    {
                        #region SEND EMAIL
                        var emailSender = new EmailSender();

                        var link = $"{WebSiteSettings.GetSettingValue<string>(SettingKeys.PHYSICAL_SITE_ADDRESS)}/Account/Register?token={token}";

                        var body = "<html><head></head><body>" +
                            "<p> Please follow this " +
                            $"<a target=\"_blank\" href=\"{link}\">link</a>" +
                            " to register for your <b>Beavers Hockey Portal</b></p>" +
                            "</body></html>";

                        try
                        {
                            success = emailSender.SendEmail(
                                        from: WebSiteSettings.GetSettingValue<string>(SettingKeys.FROM_EMAIL_ADDRESS),
                                        to: model.Email,
                                        subject: "Beavers Hockey Portal Registration",
                                        body: body
                                        );
                        }
                        catch (Exception)
                        {
                            transaction.Dispose();
                            throw;
                        }

                        #endregion SEND EMAIL

                        if (success)
                        {
                            transaction.Complete();
                        }
                        else
                        {
                            transaction.Dispose();
                        }
                    }
                }

                if (success)
                {
                    ViewData["Message"] = "Successfully Created Player Registration. An Email has been sent to the Player";

                    ModelState.Clear();
                    model = new PlayerRegistrationViewModel();
                    this.PopulateOptions(model);
                }
                else
                {
                    ModelState.AddModelError("", "Failed creating new Player Registration");
                }
            }

            return View(model);
        }

        private void PopulateOptions(PlayerRegistrationViewModel model)
        {
            model.AvailableManagers = ControllerHelper.GetManagersInScope(this.UserId, this._Repo).ToSelectListItems();

            model.AvailableTeams = ControllerHelper.GetTeamsInScope(this.UserId, this._Repo).ToSelectListItems();
        }

        private IEnumerable<GameListViewModel> GetGamesOpenForConfirmation()
        {
            var userId = this.User.Identity.GetUserId();
            var person = this._Repo.GetPersonByUserId(userId);

            var isPlayer = person.UserType_Id == (int)DataModel.Enums.UserTypeEnum.Player;

            var games = ControllerHelper.GetGamesInScope(userId, this._Repo);

            Dictionary<int, bool> playerGameConfirmationStatuses = new Dictionary<int, bool>();

            if (isPlayer)
            {
                var daysBeforeCanConfirm = WebSiteSettings.GetSettingValue<int>(SettingKeys.DAYS_BEFORE_OPENNING_CONFIRMATIONS);
                var dateWhenConfirmationIsOpen = DateTime.Now.Date.AddDays(daysBeforeCanConfirm);
                games = games.Where(g => g.Date.Date <= dateWhenConfirmationIsOpen);

                playerGameConfirmationStatuses = this._Repo.GetPlayerGameConfirmationStatuses(person.Id, games.Select(g => g.Id));
            }

            var gamesVM = games
                .OrderBy(g => g.Date)
                .Select(g => new GameListViewModel
                {
                    Id = g.Id,
                    Date = g.Date,
                    Time = g.Date,
                    Arena = g.Arena != null ? g.Arena.Name : string.Empty,
                    Manager = g.Manager.FullName,
                    IsConfirmed = isPlayer ? playerGameConfirmationStatuses[g.Id] : (bool?)null,
                    TheirTeam = g.Them != null ? g.Them.Name : string.Empty,
                });

            return gamesVM;
        }
        
        private T GetFilterValue<T>(string modelValue, string sessionKey)
        {
            T filterValue;

            if (modelValue != null && modelValue.TryConvert<T>(out filterValue))
            {
                Session[sessionKey] = filterValue;

                return filterValue;
            }

            if (Session[sessionKey] != null && Session[sessionKey].ToString().TryConvert<T>(out filterValue))
            {
                return filterValue;
            }
            else
            {
                return default(T);
            }
        }
    }
}