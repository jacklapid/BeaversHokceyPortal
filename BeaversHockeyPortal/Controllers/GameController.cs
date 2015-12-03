using DataModel.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BeaversHockeyPortal.WebUtilities;

namespace BeaversHockeyPortal.Controllers
{
    [Authorize(Roles = Utilities.Constants.ADMIN_ROLE + ", " + Utilities.Constants.MANAGER_ROLE)]
    public class GameController : AuthorizedController
    {
        // GET: Game
        public GameController(IRepository repo) : base(repo)
        { }

        public ActionResult Games()
        {
            var model = new Models.GameViewModel
            {
                GameCreateViewModel = new Models.GameCreateViewModel
                {
                    Date = DateTime.Now.Date,
                    Time = DateTime.Now
                },
                GameListViewModel = GetGames()
            };

            this.PopulateOptions(model.GameCreateViewModel);

            return View(model);
        }

        private IEnumerable<Models.GameListViewModel> GetGames()
        {
            var games = ControllerHelper.GetGamesInScope(this.UserId, this._Repo)
                .Select(g => new Models.GameListViewModel
                {
                    Id = g.Id,
                    Date = g.Date.Date,
                    Time = g.Date,
                    Arena = g.Arena != null ? g.Arena.Name : string.Empty,
                    OurScore = g.ScoreUs,
                    TheirScore = g.ScoreThem,
                    OurTeam = g.Us.Name,
                    TheirTeam = g.Them != null ? g.Them.Name : string.Empty,
                    GameNotes = g.GameNotes.Select(n => new Models.NoteViewModel
                    {
                        Creator = n.Creator != null ? n.Creator.FullName : string.Empty,
                        TimeStamp = n.TimeStamp,
                        Text = n.NoteDescription
                    }).ToList(),
                    GameStatistics = g.GameStatistics.Select(s => s.Description).ToList(),
                    Manager = g.Manager != null ? g.Manager.FullName : string.Empty,
                    Season = g.Season != null ? g.Season.Name : string.Empty
                })
                .OrderByDescending(g => g.Date);

            return games;
        }

        public ActionResult Game()
        {
            var model = new Models.GameViewModel
            {
                GameCreateViewModel = new Models.GameCreateViewModel(),
                GameListViewModel = new List<Models.GameListViewModel>()
            };

            this.PopulateOptions(model.GameCreateViewModel);

            return View(model);
        }

        [HttpPost]
        public ActionResult Game(Models.GameCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var manager = _Repo.GetManagerById(model.ManagerId);

                if (manager == null)
                {
                    ModelState.AddModelError("", "A Game must be created for a manager");
                }
                else
                {
                    var successfullyCreated = _Repo.CreateGame(
                        model.ArenaId,
                        new DateTime(model.Date.Year, model.Date.Month, model.Date.Day, model.Time.Hour, model.Time.Minute, 0),
                        manager,
                        model.UsId,
                        model.ThemId,
                        model.SeasonId);

                    if (successfullyCreated)
                    {
                        ModelState.Clear();

                        ViewBag.Message = $"Game successfully created";
                    }
                }
            }

            var fullModel = new Models.GameViewModel
            {
                GameCreateViewModel = model,
                GameListViewModel = GetGames()
            };


            //return View(fullModel);

            return RedirectToAction("Games", "Game");
        }

        private void PopulateOptions(Models.GameCreateViewModel model)
        {
            model.AvailableManagers = ControllerHelper.GetManagersInScope(this.UserId, this._Repo).ToSelectListItems();

            model.AllAvailableTeams = this._Repo.GetTeams().ToSelectListItems();

            model.AvailableArenas = this._Repo.GetArenas()
                .OrderBy(x => x.Name)
                    .Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    })
    .ToList();

            model.AvailableTeamsOwnedByManager = ControllerHelper.GetTeamsInScope(this.UserId, this._Repo)
                .OrderBy(x => x.Name)
                                    .Select(x => new SelectListItem
                                    {
                                        Text = x.Name,
                                        Value = x.Id.ToString()
                                    })
    .ToList();

            model.AvailableSeasons = _Repo.GetSeasons()
                                .OrderBy(x => x.StartDate)
                                    .Select(x => new SelectListItem
                                    {
                                        Text = x.Name,
                                        Value = x.Id.ToString()
                                    })
    .ToList();
        }

    }
}