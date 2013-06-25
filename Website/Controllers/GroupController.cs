using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Spark3.Models;

namespace Spark3.Controllers
{
    public class GroupController : Controller
    {
        //
        // GET: /Group/

        public ActionResult Index(GroupListModel grouplist)
        {
            grouplist.AfterInit();
            return View(grouplist);
        }

        // GET: /Group/Details/7
        //[OutputCache(Duration = 10, VaryByParam = "none")]
        public ActionResult Details(GroupModel group)
        {
            group.AfterInit();

            ViewData["IsValid"] = ModelState.IsValid;
            return View(group);
        }

    }
}
