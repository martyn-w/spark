using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Symplectic.Spark3.Website.Controllers
{
    public class PeopleController : Controller
    {
        //
        
        // GET: /People/
        public ActionResult Index(Models.PersonIndex personIndex)
        {
            //special hack to send a blank page for /people//index
            if (Request.ServerVariables["UNENCODED_URL"].ToLower().Contains("people//index"))
                return View("Blank");
            else
                return View(personIndex);
        }

        // GET: /People/0123456789
        public ActionResult Details(Models.Person person)
        {
            return View(person);
        }

        public ActionResult Blank()
        {
            return View();
        }

    }
}
