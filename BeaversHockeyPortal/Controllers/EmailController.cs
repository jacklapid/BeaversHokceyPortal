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
            if (!model.ToPlayerStatusIds.IsAny() && !model.ToUserIds.IsAny() && !model.ToUserTypeIds.IsAny())
            {
                ModelState.AddModelError("", "No recipients selected");
            }
            else
            {
                bool success = _Repo.CreateEmailTemplate(
                     model.ToUserIds,
                     model.ToPlayerStatusIds,
                     model.ToUserTypeIds,
                     model.From,
                     model.Subject,
                     model.Body,
                     model.ManagerId);
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
        }
    }
}