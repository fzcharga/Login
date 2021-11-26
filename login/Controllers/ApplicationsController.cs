using Microsoft.AspNet.Identity;
using login.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.Design;
using System.IO;

namespace login.Controllers
{
    public class ModelApplicationsUsers
    {
        public int id { get; set; }
        public int idApp { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string desc { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string message { get; set; }
        public string urlStart { get; set; }
        public bool useLogin { get; set; }
    }
    public class ModelApplications
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string desc { get; set; }
        public bool notUseLogin { get; set; }
    }

    public class ApplicationsController : Controller
    {
        loginEntities db = new loginEntities();
        // GET: Applications
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Gestion()
        {
            return View();
        }

        public ActionResult ReadApplications()
        {
            var userId = User.Identity.GetUserId();
            var res = from a in db.Applications
                      select new ModelApplications
                      {
                          id = a.Id,
                          name = a.Name,
                          url = a.Url,
                          desc = a.Description,
                          notUseLogin = a.NotUseLogin
                      };

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateApplications([Bind(Prefix = "models")] List<ModelApplications> liste)
        {
            if (liste != null && liste.Any())
            {
                foreach (var app in liste)
                {
                    Applications apu = new Applications
                    {
                        Name = app.name,
                        Url = app.url,
                        Description = app.desc,
                        NotUseLogin = app.notUseLogin,
                    };
                    db.Applications.Add(apu);
                    db.SaveChanges();
                    app.id = apu.Id;
                }
                
            }
            return Json(liste, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateApplications([Bind(Prefix = "models")] List<ModelApplications> liste)
        {
            if (liste != null && liste.Any())
            {
                foreach (var app in liste)
                {
                    var apu = db.Applications.FirstOrDefault(a => a.Id == app.id);
                    if (apu != null)
                    {
                        apu.Name = app.name;
                        apu.NotUseLogin = app.notUseLogin;
                        apu.Url = app.url;
                        apu.Description = app.desc;
                    }
                }
                db.SaveChanges();
            }

            return Json(liste, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadApplicationsUsers()
        {
            var userId = User.Identity.GetUserId();

            var res = from a in db.ApplicationsUsers
                      where a.FkUser == userId
                      select new ModelApplicationsUsers
                      {
                          id = a.FkApplications,
                          idApp = a.FkApplications,
                          name = a.Applications.Name,
                          url = a.Applications.Url,
                          desc = a.Applications.Description,
                          login = a.Login,
                          password = a.Password,
                          urlStart = a.UrlStart,
                          useLogin = a.Applications.NotUseLogin
                      };

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateApplicationsUsers([Bind(Prefix = "models")] List<ModelApplicationsUsers> liste)
        {
            if (liste != null && liste.Any())
            {
                var userId = User.Identity.GetUserId();
                foreach (var app in liste)
                {
                    ApplicationsUsers apu = new ApplicationsUsers
                    {
                        FkApplications = app.idApp,
                        FkUser = userId,
                        Login = app.login,
                        Password = app.password,
                        UrlStart = app.urlStart,
                    };
                    db.ApplicationsUsers.Add(apu);
                    app.id = apu.FkApplications;
                }
                db.SaveChanges();
            }

            return Json(liste, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateApplicationsUsers([Bind(Prefix = "models")] List<ModelApplicationsUsers> liste)
        {
            if (liste != null && liste.Any())
            {
                var userId = User.Identity.GetUserId();
                foreach (var app in liste)
                {
                    var apu = db.ApplicationsUsers.FirstOrDefault(a => a.FkUser == userId && a.FkApplications == app.id);
                    if (apu != null)
                    {
                        if (app.id != app.idApp)
                        {
                            db.ApplicationsUsers.Remove(apu);
                            ApplicationsUsers apuN = new ApplicationsUsers
                            {
                                FkApplications = app.idApp,
                                FkUser = userId,
                                Login = app.login,
                                Password = app.password,
                                UrlStart = app.urlStart,
                            };
                            db.ApplicationsUsers.Add(apuN);
                            app.id = apuN.FkApplications;
                        }
                        else
                        {
                            apu.Login = app.login;
                            apu.Password = app.password;
                            apu.UrlStart = app.urlStart;
                        }
                    }
                    //else
                    //{
                    //    apu = new ApplicationsUsers
                    //    {
                    //        FkApplications = app.id,
                    //        FkUser = userId,
                    //        Login = app.login,
                    //        Password = app.password
                    //    };
                    //    db.ApplicationsUsers.Add(apu);
                    //}
                }
                db.SaveChanges();
            }

            return Json(liste, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DestroyApplicationsUsers([Bind(Prefix = "models")] List<ModelApplicationsUsers> liste)
        {
            if (liste != null && liste.Any())
            {
                var userId = User.Identity.GetUserId();
                foreach (var app in liste)
                {
                    var apu = db.ApplicationsUsers.FirstOrDefault(a => a.FkUser == userId && a.FkApplications == app.id);
                    db.ApplicationsUsers.Remove(apu);
                }
                db.SaveChanges();
            }

            return Json(liste, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult Save(IEnumerable<HttpPostedFileBase> files, string idApp)
        //{
        //    if (files != null)
        //    {
        //        foreach (var file in files)
        //        {
        //            var fileName = Path.GetFileName(file.FileName);
        //            var physicalPath = Path.Combine(Server.MapPath("~/logo"), idApp + ".jpg");
        //            var physicalPathSmall = Path.Combine(Server.MapPath("~/logo"), idApp + "-small.jpg");
        //            // The files are not actually saved in this demo
        //            var bytes = Outils.ReduceSize(file.InputStream, 256, 256);
        //            using (var imageFile = new FileStream(physicalPathSmall, FileMode.Create))
        //            {
        //                imageFile.Write(bytes, 0, bytes.Length);
        //                imageFile.Flush();
        //            }
        //            file.SaveAs(physicalPath);
        //        }
        //    }
        //    return Content("");
        //}

        public ActionResult SaveImage(HttpPostedFileBase file, int id, string type)
        {
            if(file != null)
            {
                var fileName = Server.MapPath("~/Images/Applications/" + type + "-" + id + ".png");
                var bytes = Outils.ReduceSize(file.InputStream, type == "logo" ? 256 : 1024, type == "logo" ? 256 : 1024);
                using (var imageFile = new FileStream(fileName, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }
            }
            return Content("");
        }
    }    
}