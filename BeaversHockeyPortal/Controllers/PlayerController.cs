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

        public ActionResult ConfirmGame(int gameId = 0)
        {
            var game = _repo.GetGameById(gameId);
            if (game == null)
            {
                return HttpNotFound();
            }

            var userId = this.User.Identity.GetUserId();
            var player = _repo.GetPersonById(userId) as Player;

            if (game.ConfirmedPlayers.Any(p => p.Id == userId))
            {
                ModelState.AddModelError("", $"Player '{player.FullName}' already registered for this game");
            }
            else
            {
                game.ConfirmedPlayers.Add(player);
            }

            _repo.ConfirmGame(gameId, userId);

            return View(GetGamesOpenForConfirmation());
        }

        public ActionResult UnConfirmGame(int gameId = 0)
        {
            var game = _repo.GetGameById(gameId);
            if (game == null)
            {
                return HttpNotFound();
            }

            var userId = this.User.Identity.GetUserId();
            var player = _repo.GetPersonById(userId) as Player;

            if (game.ConfirmedPlayers.All(p => p.Id != userId))
            {
                ModelState.AddModelError("", $"Player '{player.FullName}' is not registered for this game. Cannot UNCONFIRM!");
            }
            else
            {
                game.ConfirmedPlayers.Remove(player);
            }

            _repo.UnconfirmGame(gameId, userId);

            return View(GetGamesOpenForConfirmation());
        }


        private IEnumerable<GameListViewModel> GetGamesOpenForConfirmation()
        {
            var daysBeforeCanConfirm = int.Parse(_repo.GetSettingValueByKey(Utilities.SettingKeys.DAYS_BEFORE_OPENNING_CONFIRMATIONS));
            var dateWhenConfirmationIsOpen = DateTime.Now.Date.AddDays(-daysBeforeCanConfirm);

            var userId = this.User.Identity.GetUserId();

            var games = ControllerHelper.GetGamesInScope(userId, _repo)
                .Where(g => g.Date.Date >= dateWhenConfirmationIsOpen)
                .OrderByDescending(g => g.Date)
                .Select(g => new GameListViewModel
                {
                    Id = g.Id,
                    Date = g.Date,
                    Time = g.Date,
                    Arena = g.Arena != null ? g.Arena.Name : string.Empty,
                    Manager = g.Manager.FullName,
                    IsConfirmed = g.ConfirmedPlayers.Any(p => p.Id == userId) ? "YES" : "NO"
                });

            return games;
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