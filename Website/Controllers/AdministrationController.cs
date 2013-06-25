using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Spark3.Models;


namespace Spark3.Controllers
{
    public class AdministrationController : Controller
    {

        #region Logger
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        // GET: /Administration/
        public ActionResult Index(AdministrationModel administration)
        {
            administration.AfterInit();
            return View(administration);
        }

        public ActionResult LogOff()
        {
            //FormsService.SignOut();
            
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Save(AdministrationModel administration)
        {
            if (ModelState.IsValid)
            {
                administration.UpdateAdministrationSettings(Request.Form);
                
                return View(administration);

                //// Attempt to register the user
                //MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                //if (createStatus == MembershipCreateStatus.Success)
                //{
                //    FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                //    return RedirectToAction("Index", "Home");
                //}
                //else
                //{
                //    ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                //}
            }
            return View(administration);

            //// If we got this far, something failed, redisplay form
            //ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            //return View(model);
        }


    }
}
