
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication1
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {


            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //reset pass without email
            routes.MapRoute(
            name: "ResetWithoutEmail",
            url: "Account/ResetPasswordWithoutEmail",
             defaults: new { controller = "Account", action = "ResetPasswordWithoutEmail" }
);

            // show all comments
            routes.MapRoute(
            name: "ShowAllComments",
            url: "Guestbook/Show",
            defaults: new { controller = "Guestbook", action = "ShowAll" }
        );
            //show by date
            routes.MapRoute(
                name: "CommentsByDate",
                url: "Guestbook/CommentsByDate/{userDate}",
                defaults: new { controller = "Guestbook", action = "CommentsByDate", userDate = UrlParameter.Optional }
                );

            // show  personal comments
            routes.MapRoute(
            name: "MyComments",
            url: "Comments/{userName}",
            defaults: new { controller = "Guestbook", action = "Comments", userName = UrlParameter.Optional }
            );

            //default
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Guestbook", action = "Index", id = UrlParameter.Optional }
            
            
);
        }
    }
}
