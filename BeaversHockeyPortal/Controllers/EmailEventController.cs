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

            var model = emailEvents.Select(ee => new EmailEventViewModel
            {
                DaysBeforeGame = ee.DaysBeforeGame,
                ReoccurrenceDays = ee.DaysForReoccurrence, //.HasValue ? ee.DaysForReoccurrence.ToString() : string.Empty,
                ManagerName = ee.Manager.FullName,
                EventTypesString = string.Join(",", ee.EmailEventTypes.Select(eet => eet.Name)),
                EmailTemplatesString = string.Join(",", ee.EmailTemplates.Select(eet => eet.Subject)),
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
                    _ctx.EmailEvents.Add(new GameEmailEvent
                    {
                        DaysBeforeGame = model.DaysBeforeGame,
                        DaysForReoccurrence = model.ReoccurrenceDays,
                        Manager = _ctx.Persons.FirstOrDefault(p => p.Id == model.ManagerId) as Manager,
                        EmailEventTypes = _ctx.EmailEventTypes.Where(eet => model.EventTypes.Contains(eet.Id)).ToList(),
                        EmailTemplates = _ctx.EmailTemplates.Where(eet => model.EmailTemplates.Contains(eet.Id)).ToList(),
                    });

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
        }
    }
}