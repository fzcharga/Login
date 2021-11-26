using login.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Validation;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace login.Controllers
{
    public class OrgaController : Controller
    {
        loginEntities db = new loginEntities();
        // GET: Orga
        public ActionResult Index()
        {
            var idg = User.Identity.GetGroupe();
            var isadmin = User.IsInRole("Admin");


            var isSelectedByAbonne = false;
            if (isadmin && idg == 1)
                isSelectedByAbonne = true;

            ViewBag.isSelectedByAbonne = isSelectedByAbonne;

            return View();
        }

        public ActionResult Index2(int? id)
        {
            //var log = db.LOGIGRAMME.Where(x => x.ID == id).FirstOrDefault();
            //       ModelLogigramme res = new ModelLogigramme{
            //           id = log.ID,
            //           name = log.LIBELLE,
            //           fk_proc = log.FK_PROC,
            //           doc = log.LOGI
            //       };
            //       return View(res);
            return View();
        }

        //public ActionResult Index3(int? id)
        //{
        //    ModelLogigramme res = new ModelLogigramme();
        //    if (id.HasValue)
        //    {
        //        var log = db.LOGIGRAMME.Where(x => x.ID == id).FirstOrDefault();
        //        res = new ModelLogigramme
        //        {
        //            id = log.ID,
        //            name = log.LIBELLE,
        //            fk_proc = log.FK_PROC,
        //            doc = log.LOGI
        //        };
        //    }
        //    else
        //    {
        //        res.fk_proc = 200;
        //    }

        //    return View(res);
        //}
       
        public ActionResult PartialOrga(int? ido = 1)
        {
            return PartialView(ido);
        }

        public ActionResult UnitePartial()
        {
            return PartialView();
        }

        public ActionResult PartialAdd(string idu, string ids)
        {
            ViewBag.idu = idu;
            ViewBag.ids = ids;
            return PartialView();
        }


        public ActionResult ReadShapDiag(int? ido = 2)
        {
            var mc = from c in db.ORGANISATION
                     where c.FK_ORG == ido
                     select new
                     {
                         id = c.FK_USER,
                         lb = c.AspNetUsers.UserName,
                         sup = c.FK_SUP
                     };

            return Json(mc, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadConnectionDiag(int? ido = 2)
        {
            var mc = from c in db.ORGANISATION
                     where c.FK_ORG == ido && c.FK_SUP != null
                     select new ModelSuperieur
                     {
                         id = c.FK_USER,
                         lb = c.AspNetUsers.UserName,
                         sup = c.FK_SUP,
                     };

            return Json(mc, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateOrga(ModeltreeOrg m)
        {
            ORGANISATION dbp = new ORGANISATION();
            dbp.FK_USER = m.user;
            dbp.FK_SUP = m.parentId;
            dbp.FK_ORG = m.org;
            dbp.ETAT = m.etat;
            db.ORGANISATION.Add(dbp);
            db.SaveChanges();
            m.lb = db.AspNetUsers.FirstOrDefault(x => x.Id == m.user).UserName;
            return Json(m, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdteOrga(ModeltreeOrg m)
        {
            ORGANISATION dbp = db.ORGANISATION.FirstOrDefault(x => x.FK_USER == m.user && x.FK_ORG == m.org);
            dbp.ETAT = m.etat;
            dbp.FK_SUP = m.parentId;
            db.SaveChanges();
            return Json(m, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DragAndDrop(ModeltreeOrg m)
        {
            if (m != null && m.user != m.parentId)
            {
                ORGANISATION dbp = db.ORGANISATION.FirstOrDefault(x => x.FK_USER == m.user && x.FK_ORG == m.org);
                dbp.FK_SUP = m.parentId;
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    var str_ = "";
                    ModelState.AddModelError("", "Un problème est survenu lors de l'enregistrement de la procedure! Merci de Réessayer");
                    var errors = e.EntityValidationErrors.First().ValidationErrors;

                    foreach (var error in errors)
                    {
                        str_ += error.ErrorMessage + Environment.NewLine;
                    }
                    return Json(new { success = false, str_ });
                }
                return Json(new { success = true, res = m });
            }
            else
            {
                var str_ = "Echéc de mouvement";
                return Json(new { success = false, str_ });
            }
        }

        public ActionResult DestroyOrga(ModeltreeOrg m)
        {
            ORGANISATION dbp = db.ORGANISATION.FirstOrDefault(x => x.FK_USER == m.user && x.FK_ORG == m.org);
            var existFils = db.ORGANISATION.Any(a => a.FK_SUP == m.user && a.FK_ORG == m.org);
            if (!existFils)
            {
                db.ORGANISATION.Remove(dbp);
                db.SaveChanges();
            }
            return Json(m, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadAllTreeUser()//, string idr)
        {
            var mc = from c in db.ORGANISATION
                     where c.FK_ORG == 1
                     select new ModelSuperieur
                     {
                         id = c.FK_USER,
                         lb = c.AspNetUsers.UserName,
                         sup = c.FK_SUP
                     };

            // return Json(mc.ToTreeDataSourceResult(request, e => e.id, e => e.sup), JsonRequestBehavior.AllowGet);
            return Json(mc, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadEtatData()
        {
            var etat = from p in db.Etat
                       select new ModelEtat { id = p.Id, lb = p.Libelle, col = p.color };
            return Json(etat);
        }

        public ActionResult ReadAllTreeUserJAVA(int? org, int? idAbonne)//, string idr)
        {
            var idg = User.Identity.GetGroupe();
            if (idAbonne.HasValue)
                idg = idAbonne.Value;

            var mc = from c in db.ORGANISATION
                     where c.FK_ORG == org // && c.UNITE.idGroupe == idg
                     select new ModeltreeOrg
                     {
                         user = c.FK_USER,
                         lb = c.AspNetUsers.UserName,
                         parentId = c.FK_SUP,
                         org = c.FK_ORG,
                         etat = c.ETAT
                     };

            // return Json(mc.ToTreeDataSourceResult(request, e => e.id, e => e.sup), JsonRequestBehavior.AllowGet);
            return Json(mc, JsonRequestBehavior.AllowGet);
        }

        //----------------------------------------UNITE------------------------------------------------------

        public ActionResult Unite()
        {
            return View();
        }

        public ActionResult ReadUniteData(int? idAbonne)
        {
            var idg = User.Identity.GetGroupe();
            if (idAbonne.HasValue)
                idg = idAbonne.Value;

            var unit = from p in db.UNITE
                       where p.idGroupe == idg
                       select new ModelOrga { id = p.ID, lb = p.LIBELLE };
            return Json(unit);
        }

        public ActionResult ReadUnite(int? idAbonne)
        {
            var idg = User.Identity.GetGroupe();
            if (idAbonne.HasValue)
                idg = idAbonne.Value;

            var unit = from e in db.UNITE
                       where e.idGroupe == idg
                       select new ModelOrga
                       {
                           id = e.ID,
                           lb = e.LIBELLE
                       };

            return Json(unit, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateUnite(ModelOrga m, int? idAbonne)
        {
            var idg = User.Identity.GetGroupe();
            if (idAbonne.HasValue)
                idg = idAbonne.Value;

            UNITE dbp = new UNITE();
            dbp.LIBELLE = m.lb;
            dbp.idGroupe = idg;
            db.UNITE.Add(dbp);
            db.SaveChanges();
            m.id = dbp.ID;
            return Json(new[] { m });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateUnite(ModelOrga m)
        {
            var prodb = db.UNITE.FirstOrDefault(x => x.ID == m.id);
            prodb.LIBELLE = m.lb;
            db.SaveChanges();
            return Json(new[] { m });
        }
        public static List<string> GetUsersBySup(string userId, bool isOneLevel = false)
        {
            var db = new loginEntities();

            List<string> res = new List<string>();

            var resG = from o in db.ORGANISATION
                       where o.FK_SUP == userId
                       select new
                       {
                           user = o.FK_USER
                       };

            foreach (var rg in resG)
            {
                res.Add(rg.user);
                if (!isOneLevel)
                    res.AddRange(GetUsersBySup(rg.user));
            }
            return res;
        }

      

    }
}