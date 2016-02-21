using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BeaversHockeyPortal
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
        name: "Default",
        url: "{controller}/{action}/{id}",
        defaults: new
        {
            controller = "Home",
            action = "Index",
            id = UrlParameter.Optional
        }
    );

            routes.MapRoute(
                name: "Register",
url: "Account/Register/{token}",
defaults: new
{
    controller = "Account",
    action = "Register",
});

            //            routes.MapRoute(
            //                name: "Register",
            //url: "{controller}/{action}/{token}",
            //defaults: new
            //{
            //    controller = "Account",
            //    action = "Register",
            //    token = UrlParameter.Optional
            //});

            //            routes.MapRoute(
            //    name: "LoginWithEmail",
            //url: "{controller}/{action}/{email}",
            //defaults: new
            //{
            //    controller = "Account",
            //    action = "Login",
            //    email = UrlParameter.Optional
            //});

        }
    }
}
