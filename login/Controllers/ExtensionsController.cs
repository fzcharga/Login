using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using login.Models;


namespace login.Controllers
{
    public class ExtensionsController : Controller
    {
        private loginEntities db = new loginEntities();

        // GET: Extensions
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read()
        {
            var res = from e in db.Extension
                      select new
                      {
                          id = e.ext,
                          type = e.TypeFichier.Libelle,
                      };

            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}