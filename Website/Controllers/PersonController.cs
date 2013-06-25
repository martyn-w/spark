using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Spark3.Models;
namespace Spark3.Controllers
{

    //NB: use Output Caching in the Controller, not the model!
    public class PersonController : Controller
    {

        // GET: /Departments/{group-id}/People/
        public ActionResult Index(PersonListModel personlist)
        {
            personlist.AfterInit();

            ViewData["IsValid"] = ModelState.IsValid;
            return View(personlist);
        }

        
        // GET: /Person/Details/person.name
        //[OutputCache(Duration = 10, VaryByParam = "none")]
        public ActionResult Details(PersonModel person)
        {
            person.AfterInit();

            ViewData["IsValid"] = ModelState.IsValid;
            return View(person);
        }
    }
}