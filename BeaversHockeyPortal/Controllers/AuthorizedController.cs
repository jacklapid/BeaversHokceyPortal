using DataModel.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using DataModel;

namespace BeaversHockeyPortal.Controllers
{
    public class AuthorizedControllerWithDbContext : AuthorizedController
    {
        public DataModelContext DbContext { get; private set; }

        public AuthorizedControllerWithDbContext(IRepository repo, DataModelContext ctx) : base(repo)
        {
            this.DbContext = ctx;
        }
    }
}
