//using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using login.Models;
// using Kendo.Mvc.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using CRM;
using CRM.Filters;



namespace login.Controllers
{
    public class MenuController : Controller


    {
        loginEntities db = new loginEntities();
        public UserManager<ApplicationUser> UserManager { get; private set; }

        public ActionResult Recap()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Menu()
        {
            return PartialView();
        }

        [AuthenticationFilter]
        public ActionResult MenuP()
        {
            return PartialView();
        }

        public ActionResult GererMenu()
        {
            //var sampleClass = new SampleClass();
            //var sampleType = sampleClass.GetType();
            //var properties = sampleType.GetProperties().OrderBy(prop => prop.Name).ToList();
            //for (int i = 0; i < listValue.Count; i++)
            //{
            //    if (i < properties.Count)
            //    {
            //        properties[i].SetValue(sampleClass, listValue[i]);
            //        Console.WriteLine(properties[i].Name + " = " + listValue[i]);
            //    }
            //}

            ViewData["roleMenu"] = db.AspNetRoles.Select(s => new { id = s.Id, lb = s.Name }); //new SelectList(, "Id", "Name");
            return View();
        }

        public List<ModelMenuP2> getchildPByRole(int par, string roleId, string userId)
        {
            var MenuC1 = (from p in MenubaseP
                          where p.fk_menu == par && p.AspNetRoles.Any(x => (roleId != null && x.Id == roleId) || (userId != null && x.AspNetUsers.Any(a => a.Id == userId)))
                          let chl = getchildPByRole(p.Id, roleId, userId)
                          orderby p.Ordre
                          select new ModelMenuP2
                          {
                              text = p.Libelle,
                              url = p.Url,
                              spriteCssClass = p.Icon,
                              items = chl,
                              title = p.Titre,
                              fillColor = "rgb(66, 139, 202)"
                          }).ToList();

            return MenuC1;
        }

        public ActionResult ReadMenuByRole(string roleId, string userId)
        {
            MenubaseP = new List<DynMenu>();

            var menutest = (from p in db.DynMenu
                            where p.AspNetRoles.Any(x => (roleId != null && x.Id == roleId) || (userId != null && x.AspNetUsers.Any(a => a.Id == userId)))
                            select p).Distinct().ToList();

            foreach (var p in menutest)
            {
                MenubaseP.Add(p);
            }

            var MenuG1 = (from p in MenubaseP
                          where p.fk_menu == null
                          let chl = getchildPByRole(p.Id, roleId, userId)
                          orderby p.Ordre
                          select new ModelMenuP2
                          {
                              text = p.Libelle,
                              url = p.Url,
                              spriteCssClass = p.Icon,
                              items = chl,
                              title = p.Titre,
                              desc = p.Description,
                              fillColor = "rgb(196, 0, 0)"
                          }).ToList();

            return Json(MenuG1, JsonRequestBehavior.AllowGet);
        }

        // Pour inserer Menu //
        public ActionResult InsertMenu(ModelMenu menu, int? idM)
        {
            if (menu != null && ModelState.IsValid)
            {
                DynMenu Menu = new DynMenu();
                Menu.Libelle = menu.libelle;
                Menu.Icon = menu.icon;
                Menu.Url = menu.url;
                Menu.Ordre = menu.ordre;
                Menu.Titre = menu.titre;
                Menu.Description = menu.desc;
                Menu.fk_menu = idM;
                db.DynMenu.Add(Menu);
                db.SaveChanges();
                ////Retourner le nouveau Id de l'enregistrement fraichement insérer au model // 
                //menu.Id = Menu.Id;
                //menu.fk_menu = Menu.fk_menu;
            }

            return Json(new[] { menu });
        }
        // pour modifier un element du Menu//
        public ActionResult UpdateMenu(ModelMenu menu)
        {
            if (menu != null && ModelState.IsValid)
            {
                //Modification BD
                var m = db.DynMenu.FirstOrDefault(x => x.Id == menu.Id);
                m.Libelle = menu.libelle;
                m.Icon = menu.icon;
                m.Url = menu.url;
                m.Ordre = menu.ordre;
                m.Titre = menu.titre;
                m.Description = menu.desc;
                db.SaveChanges();
            }
            return Json(new[] { menu });
        }

        // Pour supprimer un element du Menu // 
        public ActionResult DeleteMenu(ModelMenu menu)
        {
            var m = db.DynMenu.FirstOrDefault(x => x.Id == menu.Id);
            db.DynMenu.Remove(m);
            db.SaveChanges();
            return Json(new[] { menu });
        }

        public ActionResult ReadMenu(int? idM)
        {
            var res = from p in db.DynMenu
                      select new ModelMenu
                      {
                          Id = p.Id,
                          libelle = p.Libelle,
                          icon = p.Icon,
                          url = p.Url,
                          fk_menu = p.fk_menu,
                          ordre = p.Ordre,
                          titre = p.Titre,
                          desc = p.Description
                      };
            if (idM.HasValue)
            {
                res = res.Where(x => x.fk_menu == idM);
            }
            else
            {
                res = res.Where(x => x.fk_menu == null);
            }

            return Json(res);
        }

        List<DynMenu> Menubase = new List<DynMenu>();

        public List<ModelMenuG> getchild(int par)
        {
            var MenuC = (from p in Menubase
                         where p.fk_menu == par
                         orderby p.Ordre
                         select new ModelMenuG
                         {
                             id = p.Id,
                             text = p.Libelle,
                             url1 = p.Url,
                             @checked = p.chk,
                             spriteCssClass = p.Icon,
                             items = getchild(p.Id)
                         }).ToList();

            return MenuC;
        }


        public ActionResult readMenuJ(string idR)
        {

            if (idR == null)
            {
                Menubase = db.DynMenu.ToList();
            }
            else
            {
                Menubase = db.DynMenu.ToList();
                foreach (var c in Menubase)
                {
                    var t = c.AspNetRoles.FirstOrDefault(x => x.Id == idR);
                    c.chk = t == null ? false : true;
                }
            }

            var MenuG = (from p in Menubase
                         where p.fk_menu == null
                         orderby p.Ordre
                         select new ModelMenuG
                         {
                             id = p.Id,
                             text = p.Libelle,
                             url1 = p.Url,
                             @checked = p.chk,
                             spriteCssClass = p.Icon,
                             items = getchild(p.Id)
                         }).ToList();

            return Json(MenuG, JsonRequestBehavior.AllowGet);
        }

        List<DynMenu> MenubaseP = new List<DynMenu>();

        public List<ModelMenuP> getchildP(int par)
        {
            var MenuC1 = (from p in MenubaseP
                          where p.fk_menu == par
                          let chl = getchildP(p.Id)
                          let chlpar = chl != null ? (chl.Where(x => x.enabled).Count() == 0 ? false : true) : p.chk
                          where chlpar == true
                          orderby p.Ordre
                          select new ModelMenuP
                          {
                              text = p.Libelle,
                              url = p.Url,
                              enabled = chlpar,
                              spriteCssClass = p.Icon,
                              items = chl,
                              title = p.Titre
                          }).ToList();

            if (MenuC1.Count() == 0)
            {
                MenuC1 = null;
            }
            return MenuC1;
        }

        public ActionResult readMenuP(string idR)
        {
            if (idR == null)
            {

            }
            else
            {
                /**********************************************************************************/
                UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

                /**********************************************************************************/

                // Pour recuperer le role par rapport l'utilisateur //
                /**********************************************************************************/
                var role = UserManager.GetRoles(User.Identity.GetUserId());

                /**********************************************************************************/

                var menutest = (from p in db.DynMenu
                                from d in p.AspNetRoles.Where(x => role.Contains(x.Name)).DefaultIfEmpty()
                                select new { p = p, chk = d == null ? false : true }).Distinct().ToList();

                foreach (var c in menutest)
                {
                    c.p.chk = c.chk;
                    MenubaseP.Add(c.p);
                }
            }

            var MenuG1 = (from p in MenubaseP
                          where p.fk_menu == null
                          let chl = getchildP(p.Id)
                          let chlpar = chl != null ? (chl.Where(x => x.enabled).Count() == 0 ? false : true) : p.chk
                          where chlpar == true
                          orderby p.Ordre

                          select new ModelMenuP
                          {
                              text = p.Libelle,
                              url = p.Url,
                              enabled = chlpar,
                              spriteCssClass = p.Icon,
                              items = chl,
                              title = p.Titre,
                              desc = p.Description
                          }).ToList();

            return Json(MenuG1, JsonRequestBehavior.AllowGet);
        }

        public string InsertConfMenu(string role, int[] checkedNodes)
        {
            var oldrole = db.AspNetRoles.FirstOrDefault(x => x.Id == role);
            oldrole.DynMenu.Clear();
            List<DynMenu> menu = new List<DynMenu>();
            if (checkedNodes != null)
                menu = db.DynMenu.Where(x => checkedNodes.Contains(x.Id)).ToList();
            foreach (var m in menu)
            {
                oldrole.DynMenu.Add(m);
            }
            // oldrole.DynMenu.AddRange(menu);
            db.SaveChanges();
            return "ok";
        }
    }
}