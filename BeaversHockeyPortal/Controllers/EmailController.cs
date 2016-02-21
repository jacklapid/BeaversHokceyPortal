using BeaversHockeyPortal.Models;
using DataModel.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BeaversHockeyPortal.WebUtilities;
using Utilities;

namespace BeaversHockeyPortal.Controllers
{
    [Authorize(Roles = Utilities.Constants.ADMIN_ROLE + ", " + Utilities.Constants.MANAGER_ROLE)]
    public class EmailController : AuthorizedController
    {
        public EmailController(IRepository repo) : base(repo)
        { }

        // GET: Email
        public ActionResult Templates()
        {
            var emailTemplates = ControllerHelper.GetEmailTemplatesInScope(this.UserId, this._Repo);

            var emailTemplatesVM = emailTemplates.Select(et => new EmailTemplateViewModel
            {
                Body = et.Body,
                From = et.From,
                Subject = et.Subject,
                To = et.To,
                Context = et.Context,
            });

            return View(emailTemplatesVM);
        }


        public ActionResult Create()
        {
            var model = new EmailTemplateViewModel();

            this.PopulateOptions(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmailTemplateViewModel model)
        {
            if (model.ToSelection != ToSelection.Specific && model.ToSelection != ToSelection.Text)
            {
                ModelState.AddModelError("", "'TO' field is NOT SPECIFIED");
            }
            else if (model.ToSelection == ToSelection.Specific && !model.ToPlayerStatusIds.IsAny() && !model.ToUserIds.IsAny() && !model.ToUserTypeIds.IsAny())
            {
                ModelState.AddModelError("", "No Specific recipients selected");
            }
            else if (model.ToSelection == ToSelection.Text && string.IsNullOrWhiteSpace(model.To))
            {
                ModelState.AddModelError("", "No Text recipients selected");
            }
            else
            {
                var success = false;

                if (model.ToSelection == ToSelection.Specific)
                {
                    success = _Repo.CreateEmailTemplateToPredefiniedUsers(
                     model.ToUserIds,
                     model.ToPlayerStatusIds,
                     model.ToUserTypeIds,
                     model.From,
                     model.Subject,
                     model.Body,
                     model.Context,
                     model.ManagerId);
                }
                else if (model.ToSelection == ToSelection.Text)
                {
                    success = _Repo.CreateEmailTemplate(
                     model.To,
                     model.From,
                     model.Subject,
                     model.Body,
                     model.Context,
                     model.ManagerId);
                }
                if (success)
                {
                    ModelState.Clear();
                    ViewData["Message"] = "Successfully Created Email Template";
                }
                else
                {
                    ModelState.AddModelError("", "Error saving email templates");
                }
            }
            model = new EmailTemplateViewModel();
            this.PopulateOptions(model);

            return View(model);
        }

        private void PopulateOptions(EmailTemplateViewModel model)
        {
            model.AvailableManagers = ControllerHelper.GetManagersInScope(this.UserId, this._Repo).ToSelectListItems();

            model.AvailableUsers = ControllerHelper.GetUsersInScope(this.UserId, this._Repo).ToSelectListItems();

            model.AvailablePlayerStatuses = EnumHelper<DataModel.Enums.PlayerStatusEnum>.ToDictionary().ToSelectListItems();

            model.AvailableUserTypes = EnumHelper<DataModel.Enums.UserTypeEnum>.ToDictionary().ToSelectListItems();

            model.ToSelection = ToSelection.Text;

            model.From = Utilities.Constants.ADMIN_EMAIL;
        }
    }
}