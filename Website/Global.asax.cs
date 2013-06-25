using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Spark3
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute("Departments-Index",
                "departments",
                new { controller = "Group", action = "Index", groupid = UrlParameter.Optional });


            routes.MapRoute("Departments-Details",
                "departments/{groupid}",
                new { controller = "Group", action = "Details", groupid = UrlParameter.Optional });

            routes.MapRoute("Departments-People-Index",
                "departments/{groupid}/people",
                new { controller = "Person", action = "Index", groupid = UrlParameter.Optional });

            routes.MapRoute("People-Details",
                "departments/{groupid}/people/{personid}",
                new { controller = "Person", action = "Details", groupid = UrlParameter.Optional, personid = UrlParameter.Optional });

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();

            

            AreaRegistration.RegisterAllAreas();

            
            RegisterRoutes(RouteTable.Routes);
        }
    }
}