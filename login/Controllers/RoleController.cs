using login.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;


namespace login.Controllers
{
    public class RoleController : Controller
    {
        loginEntities db = new loginEntities();


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RoleUs(string idu)
        {
            /***********************/
            ViewBag.idu = idu;
            return PartialView();
        }

        public ActionResult ReadAllRoles()
        {
            var res = from r in db.AspNetRoles
                      select new
                      {
                          id = r.Id,
                          lb = r.Name
                      };

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadRoleByUser(string userId)
        {
            var res = from r in db.AspNetRoles
                      where r.AspNetUsers.Any(a => a.Id == userId)
                      select new
                      {
                          id = r.Id,
                          name = r.Name,
                      };

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadRole( string idr)
        {
            var role = from d in db.AspNetRoles
                       from p in d.AspNetUsers.Where(x => x.Id == idr).DefaultIfEmpty()
                       select new ModelRole { id = d.Id, lb = d.Name, val = p != null ? true : false };
            return Json(role);
        }

        public ActionResult ReadR()
        {
            var role = from d in db.AspNetRoles
                       select new ModelRole
                       {
                           id = d.Id,
                           lb = d.Name,
                           tr = d.AspNetUsers.Count(),
                           users = from x in d.AspNetUsers select x.UserName
                       };

            return Json(role, JsonRequestBehavior.AllowGet);
        }

        // ---- Pour modifier les roles ------------- //
        public ActionResult UpdateR( [Bind(Prefix = "models")]IEnumerable<ModelRole> Role)
        {
            if (Role != null && ModelState.IsValid)
            {
                foreach (var p in Role)
                {
                    var r = db.AspNetRoles.FirstOrDefault(x => x.Id == p.id);

                    r.Name = p.lb;
                    db.SaveChanges();
                }
            }

            return Json(Role);
        }

        // ---- Pour supprimer les roles ------------- //
        public ActionResult DeleteR([Bind(Prefix = "models")]IEnumerable<ModelRole> Role)
        {
            if (Role != null && ModelState.IsValid)
            {
                foreach (var p in Role)
                {
                    var r = db.AspNetRoles.FirstOrDefault(x => x.Id == p.id);
                    db.AspNetRoles.Remove(r);
                    db.SaveChanges();
                }
            }

            return Json(Role);
        }
        
        //------Pour créer un role-------// 

        public ActionResult CreateR( [Bind(Prefix = "models")]IEnumerable<ModelRole> Role)
        {
            if (Role != null && ModelState.IsValid)
            {
                foreach (var p in Role)
                {
                    AspNetRoles r = new AspNetRoles();
                    r.Name = p.lb;
                    Guid guid2 = Guid.NewGuid();
                    r.Id = guid2.ToString();
                    db.AspNetRoles.Add(r);
                    p.id = r.Id;

                }
                db.SaveChanges();
            }

            return Json(Role);
        }

        //------------- Pour modifier un role ---------------// 
        public ActionResult UpdateRole(string idr,  [Bind(Prefix = "models")]IEnumerable<ModelRole> Role)
        {
            if (Role != null && ModelState.IsValid)
            {
                var use = db.AspNetUsers.FirstOrDefault(p1 => p1.Id == idr);
                foreach (var p2 in Role)
                {
                    var rol = db.AspNetRoles.FirstOrDefault(x => x.Id == p2.id);
                    if (p2.val == true && !(use.AspNetRoles.Contains(rol)))
                    {
                        use.AspNetRoles.Add(rol);

                    }
                    else if (p2.val == false && (use.AspNetRoles.Contains(rol)))
                    {
                        use.AspNetRoles.Remove(rol);

                    }
                }
                db.SaveChanges();
            }

            return Json(Role);
        }
    }
}