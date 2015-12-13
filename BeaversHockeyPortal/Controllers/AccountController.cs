using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BeaversHockeyPortal.Models;
using DataModel;
using System.Transactions;

namespace BeaversHockeyPortal.Controllers
{
    [Authorize]
    public class AccountController : AuthorizedControllerWithDbContext
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController(DataModel.Repositories.IRepository repo, DataModelContext dbContext) : base(repo, dbContext)
        { }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, DataModel.Repositories.IRepository repo, DataModelContext dbContext) : base(repo, dbContext)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string email)
        {
            ViewBag.ReturnUrl = returnUrl;

            var model = new LoginViewModel();

            if (!string.IsNullOrWhiteSpace(email))
            {
                model.Email = email;
            }

            return View(model);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var userName = model.Email;
            var result = await SignInManager.PasswordSignInAsync(userName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }


        private bool HasTokenExpired(PlayerRegistration personToRegister)
        {
            var experiationDate = personToRegister.TokenGeneratedOn.AddDays(-Utilities.Constants.REGISTRATION_TOKEN_EXPIRATION_DAYS);

            return experiationDate > DateTime.Now;
        }

        private void PopulateOptionsForRegisteringPlayer(RegisterViewModel model, PlayerRegistration playerRegistration)
        {
            //ROles
            model.AvailableRoles = this._Repo.GetRoles().Where(role => role.Name == DataModel.Enums.RoleEnum.Player.ToString()).
                Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Id,
                    Selected = true
                })
                    .ToList();

            model.RoleId = this._Repo.GetRoles().FirstOrDefault(role => role.Name == DataModel.Enums.RoleEnum.Player.ToString()).Id;

            //Manager
            model.AvailableManagers.Add(new SelectListItem
            {
                Text = playerRegistration.Manager.FullName,
                Value = playerRegistration.Manager.Id.ToString()
            });
            model.ManagerId = playerRegistration.Manager.Id;

            //Team
            model.AvailableTeams.Add(new SelectListItem
            {
                Text = playerRegistration.Team.Name,
                Value = playerRegistration.Team.Id.ToString()
            });
            model.TeamId = playerRegistration.Team.Id;
        }

        private void PopulateOptionsForLoggedUser(RegisterViewModel model)
        {
            var userId = this.UserId;

            model.AvailableRoles = ControllerHelper.GetRolesInScope(userId, this._Repo)
                    .Select(r => new SelectListItem
                    {
                        Text = r.Name,
                        Value = r.Id
                    })
                    .ToList();

            model.AvailableManagers = ControllerHelper.GetManagersInScope(userId, this._Repo)
                .Select(m => new SelectListItem
                {
                    Text = m.FullName,
                    Value = m.Id.ToString()
                })
                .ToList();

            model.AvailableTeams = ControllerHelper.GetTeamsInScope(userId, this._Repo)
                .Select(t => new SelectListItem
                {
                    Text = t.Name,
                    Value = t.Id.ToString()
                })
                .ToList();
        }

        [AllowAnonymous]
        public ActionResult Register(string token)
        {
            var model = new RegisterViewModel();

            if (this.User.Identity.IsAuthenticated)
            {
                this.PopulateOptionsForLoggedUser(model);
            }
            else
            {

                var playerToRegister = this._Repo.GetPlayerToRegisterByToken(token);
                var isTokenValid = ValidateToken(playerToRegister);

                if (isTokenValid)
                {
                    this.PopulateOptionsForRegisteringPlayer(model, playerToRegister);

                    model.Email = playerToRegister.PlayerEmail;
                }
            }

            return View(model);
        }

        private bool ValidateToken(PlayerRegistration playerToRegister)
        {
            var isTokenValid = false;

            if (playerToRegister == null)
            {
                ModelState.AddModelError("", "You may only register from a provided LINK with a valid token");
            }
            else if (HasTokenExpired(playerToRegister))
            {
                ModelState.AddModelError("", "You may only register token has expired. Contact admin.");
            }
            else if (playerToRegister.TokenAlreadyUsed)
            {
                ModelState.AddModelError("", "You cannot register with this token as it has already been used.");
            }
            else
            {
                isTokenValid = true;
            }

            return isTokenValid;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, string token)
        {
            var canRegister = false;
            var success = false;
            PlayerRegistration playerToRegister = null;

            if (this.User.Identity.IsAuthenticated)
            {
                canRegister = true;
            }
            else
            {
                playerToRegister = this._Repo.GetPlayerToRegisterByToken(token);
                canRegister = ValidateToken(playerToRegister);
            }

            if (ModelState.IsValid && canRegister)
            {
                using (TransactionScope transaction = new TransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        var user = new ApplicationUser
                        {
                            UserName = model.Email,
                            Email = model.Email,
                        };

                        // Create USER Account
                        var result = await UserManager.CreateAsync(user, model.Password);
                        success = result.Succeeded;

                        if (result.Succeeded)
                        {
                            // Add role
                            user.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole
                            {
                                RoleId = model.RoleId,
                                UserId = user.Id
                            });

                            // Create PERSON
                            var roleName = this._Repo.GetRoles().First(r => r.Id == model.RoleId).Name;
                            Person person = null;

                            #region Create Person
                            switch (roleName)
                            {
                                case Utilities.Constants.PLAYER_ROLE:

                                    var manager = this._Repo.GetManagerById(model.ManagerId);
                                    if (manager == null)
                                    {
                                        AddErrors("Player must be attached to a manager");
                                        success = false;
                                    }
                                    else
                                    {
                                        person = new Player
                                        {
                                            PlayerPosition_Id = (int)model.Position,
                                            PlayerStatus_Id = (int)model.Status,
                                            Manager = manager,
                                            Team = this._Repo.GetTeamById(model.TeamId),
                                        };
                                    }
                                    break;
                                case Utilities.Constants.MANAGER_ROLE:
                                    person = new Manager
                                    {

                                    };
                                    break;
                            }
                            #endregion Create Person

                            if (success)
                            {
                                person.ApplicationUser_Id = user.Id;
                                person.FirstName = model.FirstName;
                                person.LastName = model.LastName;

                                success = this._Repo.SavePerson(person);
                            }
                        }
                        else //await UserManager.CreateAsync
                        {
                            AddErrors(result);
                        }

                        if (success)
                        {
                            // Mark TOKEN as already used
                            if (playerToRegister != null)
                            {
                                playerToRegister.TokenAlreadyUsed = true;

                                this.DbContext.SaveChanges();
                            }

                            transaction.Complete();
                        }
                        else
                        {
                            transaction.Dispose();
                        }
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException e)
                    {
                        transaction.Dispose();

                        var newException = new FormattedDbEntityValidationException(e);
                        throw newException;
                    }
                }
            }

            if (success && playerToRegister != null)
            {
                return RedirectToAction("Login", "Account", new { email = playerToRegister.PlayerEmail });
            }

            if (success)
            {
                ModelState.Clear();
                model = new RegisterViewModel();
                @ViewData["Message"] = $"Successully created user for Email: {model.Email}";
            }

            this.PopulateOptionsForLoggedUser(model);
            return View(model);
        }

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register2(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        ApplicationUser user = null;
        //        var roleName = _repo.GetRoles().First(r => r.Id == model.RoleId).Name;

        //        switch (roleName)
        //        {
        //            case Utilities.Constants.PLAYER_ROLE:

        //                var manager = _repo.GetManagerById(model.ManagerId);
        //                if (manager == null)
        //                {
        //                    AddErrors("Player must be attached to a manager");
        //                }
        //                else
        //                {
        //                    user = new Player
        //                    {
        //                        PlayerPosition_Id = (int)model.Position,
        //                        PlayerStatus_Id = (int)model.Status,
        //                        Manager = manager,
        //                        Team = _repo.GetTeamById(model.TeamId)
        //                    };
        //                }
        //                break;
        //            case Utilities.Constants.MANAGER_ROLE:
        //                user = new Manager
        //                {

        //                };
        //                break;
        //            default:
        //                AddErrors($"Cannot create user for Role {model.RoleId}");
        //                break;
        //        }

        //        if (user != null)
        //        {
        //            var person = user as Person;
        //            person.FirstName = model.FirstName;
        //            person.LastName = model.LastName;

        //            user.UserName = model.Email;
        //            user.Email = model.Email;

        //            user.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole
        //            {
        //                RoleId = model.RoleId,
        //                UserId = user.Id
        //            });
        //            try
        //            {


        //                var result = await UserManager.CreateAsync(user, model.Password);
        //                if (result.Succeeded)
        //                {
        //                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

        //                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //                    // Send an email with this link
        //                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

        //                    return RedirectToAction("Register", "Account");
        //                }
        //                AddErrors(result);
        //            }
        //            catch (System.Data.Entity.Validation.DbEntityValidationException e)
        //            {
        //                var newException = new FormattedDbEntityValidationException(e);
        //                throw newException;
        //            }
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    this.PopulateOptions(model);
        //    return View(model);
        //}

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private void AddErrors(string error)
        {
            ModelState.AddModelError("", error);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}