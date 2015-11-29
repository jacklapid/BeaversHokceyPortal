using DataModel.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace BeaversHockeyPortal.Controllers
{
    [Authorize(Roles = Utilities.Constants.ADMIN_ROLE + ", " + Utilities.Constants.MANAGER_ROLE)]
    public class TeamController : Controller
    {
        private IRepository _repo;

        public TeamController(IRepository repo)
        {
            _repo = repo;
        }

        public ActionResult Teams()
        {

            var model = new Models.TeamViewModel
            {
                ListTeamViewModels = this.GetTeams(),
                CreateTeamViewModel = new Models.CreateTeamViewModel()
            };

            this.PopulateOptions(model.CreateTeamViewModel);
            return View(model);
        }

        private IEnumerable<BeaversHockeyPortal.Models.ListTeamViewModel> GetTeams()
        {
            var teams = _repo.GetTeams().ToList()
                .Select(t => new BeaversHockeyPortal.Models.ListTeamViewModel
                {
                    Name = t.Name,
                    ImageUrl = BeaversHockeyPortal.WebUtilities.ImageUtilities.GetImageUrl(t.ImageIds.FirstOrDefault()),
                    ManagerFullName = t.Manager != null ? t.Manager.FullName : string.Empty
                });

            return teams;
        }

        private void PopulateOptions(Models.CreateTeamViewModel model)
        {
            model.AvailableManagers = ControllerHelper.GetManagersInScope(this.User.Identity.GetUserId(), this._repo)
    .Select(m => new SelectListItem
    {
        Text = m.FullName,
        Value = m.Id.ToString()
    })
    .ToList();
        }

        public ActionResult Team()
        {

            var model = new Models.TeamViewModel
            {
                CreateTeamViewModel = new Models.CreateTeamViewModel(),
                ListTeamViewModels = new List<Models.ListTeamViewModel>()
            };

            this.PopulateOptions(model.CreateTeamViewModel);

            return View(model);
        }

        [HttpPost]
        public ActionResult Team(BeaversHockeyPortal.Models.CreateTeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                var manager = _repo.GetManagerById(model.ManagerId);

                if (manager == null)
                {
                    ModelState.AddModelError("", "Team must be created with a manager");
                }
                else
                {
                    int? fileAttachmentId = null;

                    var successfullyCreated = _repo.CreateTeam(model.Name, fileAttachmentId, manager);

                    if (successfullyCreated)
                    {
                        ModelState.Clear();

                        ViewBag.Message = $"Team {model.Name} successfully created";
                    }
                }
            }

            var fullModel = new Models.TeamViewModel
            {
                ListTeamViewModels = this.GetTeams(),
                CreateTeamViewModel = model
            };


//            return View(fullModel);

            return RedirectToAction("Teams", "Team");

        }


    }
}