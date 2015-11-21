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
        private IPlayerRepository _playerRepository;

        public PlayerController(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        // GET: Player
        public ActionResult Players()
        {
            var userID = User.Identity.GetUserId();
            var person = _playerRepository.GetPersonById(userID);

            IEnumerable<string> managerIds = new List<string>();

            if (person == null)
            {
                return View(new List<PlayerViewModels>());
            }

            switch (person.UserTypeId)
            {
                case (int)UserTypeEnum.Admin:
                    managerIds = _playerRepository.GetManagers().Select(m => m.Id);
                    break;
                case (int)UserTypeEnum.Manager:
                    managerIds = new List<string> { person.Id };
                    break;
                case (int)UserTypeEnum.Player:
                    var player = person as Player;
                    managerIds = new List<string> { player.ManagerId };
                    break;
                default:
                    throw new Exception($"Unable to view player for current user: {person.FullName}");

            }

            var players = managerIds.ToList()
                .SelectMany(managerId => _playerRepository.GetPlayersForManager(managerId))
                    .OrderBy(p => p.PlayerStatusId == (int)PlayerStatusEnum.Regular)
                    .Select(p => new PlayerViewModels
                    {
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        Position = Enum.GetName(typeof(DataModel.Enums.PlayerPositionEnum), p.PlayerPositionId),
                        PositionId = p.PlayerPositionId
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