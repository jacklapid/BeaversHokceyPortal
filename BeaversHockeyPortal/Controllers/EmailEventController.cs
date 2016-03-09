using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using DataModel;
using BeaversHockeyPortal.Models;
using BeaversHockeyPortal.WebUtilities;
using System.Data.Entity;
using DataModel.Enums;

namespace BeaversHockeyPortal.Controllers
{
    [Authorize(Roles = Utilities.Constants.ADMIN_ROLE + ", " + Utilities.Constants.MANAGER_ROLE)]
    public class EmailEventController : Controller
    {
        private DataModelContext _ctx;

        private string UserId
        {
            get
            {
                return this.User.Identity.GetUserId();
            }
        }

        public EmailEventController(DataModelContext ctx)
        {
            this._ctx = ctx;
        }

        // GET: EmailEvent
        public ActionResult Events()
        {
            var managers = ControllerHelper.GetManagersInScope(this.UserId, this._ctx);

            var emailEvents = managers
                .SelectMany(m => _ctx.EmailEvents
                                    .Include(ee => ee.EmailEventTypes)
                                    .Include(ee => ee.EmailTemplates)
                                    .Where(ee => ee.Manager != null && ee.Manager.Id == m.Id))
                .Distinct()
                .ToList();

            var model =
                (from ee in emailEvents
                 let gameEE = ee as GameEmailEvent
                 select
                 new EmailEventViewModel
                 {
                     DaysBeforeGame = gameEE != null ? gameEE.DaysBeforeGame : 0,
                     ReoccurrenceDays = gameEE != null ? gameEE.DaysForReoccurrence : null, //.HasValue ? ee.DaysForReoccurrence.ToString() : string.Empty,
                     ManagerName = ee.Manager.FullName,
                     EventTypesString = gameEE != null ? string.Join(",", ee.EmailEventTypes.Select(eet => eet.Name)) : "N/A",
                     EmailTemplatesString = string.Join(",", ee.EmailTemplates.Select(eet => eet.Subject)),
                     EmailEventType = gameEE != null ? Models.EmailEventType.Game : Models.EmailEventType.Other
                 })
            .ToList();

            return View(model);
        }

        public ActionResult Create()
        {
            var model = new EmailEventViewModel();

            PopulateOptions(model);

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(EmailEventViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.EmailEventType == Models.EmailEventType.Game)
                    {
                        if (model.EventTypes.Any())
                        {
                            _ctx.EmailEvents.Add(new GameEmailEvent
                            {
                                DaysBeforeGame = model.DaysBeforeGame,
                                DaysForReoccurrence = model.ReoccurrenceDays,
                                Manager = _ctx.Persons.FirstOrDefault(p => p.Id == model.ManagerId) as Manager,
                                EmailEventTypes = _ctx.EmailEventTypes.Where(eet => model.EventTypes.Contains(eet.Id)).ToList(),
                                EmailTemplates = _ctx.EmailTemplates.Where(eet => model.EmailTemplates.Contains(eet.Id)).ToList(),
                            });
                        }
                        else
                        {
                            ModelState.AddModelError("", "Error saving email event: Event TYPE is not provided");
                        }
                    }
                    else if (model.EmailEventType == Models.EmailEventType.Other)
                    {
                        var emailEventType = (int)EmailEventTypeEnum.Other;

                        _ctx.EmailEvents.Add(new EmailEvent
                        {
                            Manager = _ctx.Persons.FirstOrDefault(p => p.Id == model.ManagerId) as Manager,
                            EmailEventTypes = _ctx.EmailEventTypes.Where(eet => eet.Id == emailEventType).ToList(),
                            EmailTemplates = _ctx.EmailTemplates.Where(eet => model.EmailTemplates.Contains(eet.Id)).ToList(),
                        });
                    }

                    _ctx.SaveChanges();

                    ModelState.Clear();
                    ViewData["Message"] = "Successfully Created Email Event";
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Error saving email event");
                }
            }

            model = new EmailEventViewModel();
            this.PopulateOptions(model);

            return View(model);
        }

        private void PopulateOptions(EmailEventViewModel model)
        {
            var managers = ControllerHelper.GetManagersInScope(this.UserId, this._ctx);

            model.AvailableManagers = managers.ToSelectListItems();

            model.AvailableEmailTemplates = managers
                .SelectMany(m => _ctx.EmailTemplates.Where(et => et.Manager != null && et.Manager.Id == m.Id))
                .Distinct()
                .ToList()
                .ToSelectListItems();

            model.AvailableEventTypes = System.Web.Mvc.Html.EnumHelper.GetSelectList(typeof(DataModel.Enums.EmailEventTypeEnum));

            model.EmailEventType = Models.EmailEventType.Game;
        }
    }
}