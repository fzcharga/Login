using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using login.Models;

namespace login.Controllers
{
    public class MailController : Controller
    {
        loginEntities db = new loginEntities();
        // GET: Mail
        public ActionResult Index()
        {
            var urlCRM = System.Configuration.ConfigurationManager.AppSettings["URLCRM"];
            ViewBag.urlCRM = urlCRM;
            return View();
        }

        public ActionResult ConfigMail()
        {
            return View();
        }
        
        public ActionResult ReadUsers(int idObj)
        {
            var res = from users in db.AspNetUsers
                      where users.IdCRM != 0
                      join s in db.SendedMail.Where(w => w.Fk_ObejctMail == idObj) on users.Id equals s.Fk_User into joined
                      from j in joined.DefaultIfEmpty()
                      select new
                      {
                          id = users.Id,
                          name = users.UserName,
                          email = users.Email,
                          idCRM = users.IdCRM,
                          dateCreation = j != null ? (DateTime?)j.Date : null,
                          idCreatedMail = j != null,
                          idMail = j != null ? (int?)j.IdMail : null,

                      };
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadObjectMail()
        {
            var res = from objectMail in db.ObjectMail
                      select new ModelObjectMail
                      {
                          id = objectMail.Id,
                          lb = objectMail.Libelle,
                          idModelCRM = objectMail.IdModelCRM
                      };
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateObjectMail ([Bind(Prefix = "models")] List<ModelObjectMail> objectMails)
        {
            foreach(var objectMail in objectMails)
            {
                ObjectMail mail = new ObjectMail
                {
                    Libelle = objectMail.lb,
                    IdModelCRM = objectMail.idModelCRM,
                };
                db.ObjectMail.Add(mail);
                db.SaveChanges();
                objectMail.id = mail.Id;
            }
            return Json(objectMails, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdatObjectMail([Bind(Prefix = "models")] List<ModelObjectMail> objectMails)
        {
            foreach (var objectMail in objectMails)
            {
                var mail = db.ObjectMail.FirstOrDefault(f => f.Id == objectMail.id);
                mail.Libelle = objectMail.lb;
                mail.IdModelCRM = objectMail.idModelCRM;
            }
            db.SaveChanges();
            return Json(objectMails, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteObjectMail([Bind(Prefix = "models")] List<ModelObjectMail> objectMails)
        {
            foreach (var objectMail in objectMails)
            {
                var mail = db.ObjectMail.FirstOrDefault(f => f.Id == objectMail.id);
                db.ObjectMail.Remove(mail);
            }
            db.SaveChanges();
            return Json(objectMails, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SendMail(List<ModelSendMailCRM> clients, int idObj)
        {
            var obj = db.ObjectMail.FirstOrDefault(f => f.Id == idObj);
            int idModel = obj.IdModelCRM.Value;
            foreach (var client in clients)
            {
                try
                {
                    var user = db.AspNetUsers.FirstOrDefault(f => f.Id == client.id);
                    var props = new Dictionary<string, string>();
                    props.Add("login", user.UserName);
                    int idMail = ServiceMail.CreateMail(client.idCRM, idModel, null, null, null, props, new List<ModelStreamFile>());
                    if (idMail != 0)
                    {
                        //client.success = false;
                        //client.message = "Test Erreur";
                        //continue;
                        var DateCreation = DateTime.Now;
                        var sendedMail = db.SendedMail.FirstOrDefault(f => f.Fk_ObejctMail == idObj && f.Fk_User == client.id);
                        if (sendedMail != null)
                        {
                            sendedMail.Date = DateCreation;
                            sendedMail.IdMail = idMail;
                        }
                        else
                        {
                            sendedMail = new SendedMail { Fk_ObejctMail = idObj, IdMail = idMail, Date = DateCreation, Fk_User = client.id };
                            db.SendedMail.Add(sendedMail);
                        }
                        client.dateCreation = DateCreation;
                        client.idMail = idMail;
                        client.success = true;
                    }
                }
                catch (Exception ex)
                {
                    client.success = false;
                    client.message = ex.Message;
                }
            }
            db.SaveChanges();
            return Json(clients, JsonRequestBehavior.AllowGet);
        }
    }
}