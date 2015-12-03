using DataModel.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace BeaversHockeyPortal.Controllers
{
    [Authorize(Roles = Utilities.Constants.ADMIN_ROLE + ", " + Utilities.Constants.MANAGER_ROLE)]
    public class AuthorizedController : Controller
    {
        public IRepository _Repo { get; private set; }



        public AuthorizedController(IRepository repo)
        {
            this._Repo = repo;
        }

        public string UserId
        {
            get
            {
                return this.User.Identity.GetUserId();
            }
        }
    }
}
