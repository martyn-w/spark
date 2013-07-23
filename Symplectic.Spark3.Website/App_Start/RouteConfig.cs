using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Symplectic.Spark3.Website
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            

            

           // routes.MapRoute(
           //     name: "Default",
           //     url: "{controller}/{action}/{id}",
           //     defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
           // );


            routes.MapRoute(
                name: "Index",
                url: "people/index",
                defaults: new { controller = "People", action = "Index" }
                );

            routes.MapRoute(
                name: "Details",
                url: "people/{id}/{*anything}",
                defaults: new { controller = "People", action = "Details"}
//                constraints: new { id = @"\d+" }
                );


            routes.MapRoute(
                name: "Blank",
                url: "{controller}/{*anything}",
                defaults: new { controller = "People", action = "Blank" }
                );

            


        }
    }
}