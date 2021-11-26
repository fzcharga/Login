using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace login.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            //var isAdmin = User.IsInRole("Admin") || User.IsInRole("Manager");
            
            //if(isAdmin)
            //    return RedirectToAction("Index", "Alimentation");

            //return RedirectToAction("Index", "Depense");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ConnexionApps(string appName)
        {
            ViewBag.Message = "Patienter ...";
            ViewBag.appName = appName;
            return View();
        }
    }
}