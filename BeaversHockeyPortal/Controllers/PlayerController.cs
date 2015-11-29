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

namespace BeaversHockeyPortal.Controllers
{
    [Authorize]
    public class PlayerController : Controller
    {
        private IRepository _repo;

        public PlayerController(IRepository playerRepository)
        {
            _repo = playerRepository;
        }

        public ActionResult ConfirmGames()
        {
            return View(GetGamesOpenForConfirmation());
        }

        public ActionResult ConfirmGame(int id = 0, bool confirming = false)
        {
            var game = _repo.GetGameById(id);
            if (game == null)
            {
                return HttpNotFound();
            }

            var userId = this.User.Identity.GetUserId();
            var player = _repo.GetPersonByUserId(userId) as Player;

            if (player == null)
            {
                ModelState.AddModelError("", $"Only a player can confirm the game");
            }
            else
            {
                var isGameConfirmed = _repo.GetPlayerGameConfirmationStatus(player.Id, id);

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
                        _repo.ConfirmGame(id, player.Id);
                    }
                    else
                    {
                        _repo.UnconfirmGame(id, player.Id);
                    }
                    return RedirectToAction("ConfirmGames", "Player");
                }
            }

            return View(GetGamesOpenForConfirmation());
        }


        private IEnumerable<GameListViewModel> GetGamesOpenForConfirmation()
        {
            var userId = this.User.Identity.GetUserId();
            var person = _repo.GetPersonByUserId(userId);

            var isPlayer = person.UserType_Id == (int)DataModel.Enums.UserTypeEnum.Player;

            var games = ControllerHelper.GetGamesInScope(userId, _repo);

            Dictionary<int, bool> playerGameConfirmationStatuses = new Dictionary<int, bool>();

            if (isPlayer)
            {
                var daysBeforeCanConfirm = int.Parse(_repo.GetSettingValueByKey(Utilities.SettingKeys.DAYS_BEFORE_OPENNING_CONFIRMATIONS));
                var dateWhenConfirmationIsOpen = DateTime.Now.Date.AddDays(daysBeforeCanConfirm);
                games = games.Where(g => g.Date.Date <= dateWhenConfirmationIsOpen);

                playerGameConfirmationStatuses = _repo.GetPlayerGameConfirmationStatuses(person.Id, games.Select(g => g.Id));
            }

            var gamesVM = games
                .OrderByDescending(g => g.Date)
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

        // GET: Player
        public ActionResult Players()
        {
            var userID = User.Identity.GetUserId();

            var players = ControllerHelper.GetPlayersInScope(userID, _repo).ToList()
        .OrderBy(p => p.PlayerStatus_Id == (int)PlayerStatusEnum.Regular)
                .ThenBy(p => p.FullName)
        .Select(p => new PlayerViewModels
        {
            FirstName = p.FirstName,
            LastName = p.LastName,
            Position = Enum.GetName(typeof(DataModel.Enums.PlayerPositionEnum), p.PlayerPosition_Id),
            PositionId = p.PlayerPosition_Id,
            Team = p.Team != null ? p.Team.Name : string.Empty,
            Manageer = p.Manager != null ? p.Manager.FullName : string.Empty
        })
        .ToList();

            return View(players);
        }

        [Authorize(Roles = "Manager, Admin")]
        public ActionResult CreatePlayer()
        {
            return View();
        }
    }
}