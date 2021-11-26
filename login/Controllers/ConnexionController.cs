using login.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace login.Controllers
{

    public class ModelDAtaConnexion
    {
        public DateTime date { get; set; }
        public string user { get; set; }
        public string org { get; set; }
        public string page { get; set; }
        public string op { get; set; }
        public string jour
        {
            get
            {
                return date.ToString("dd/MM/yyyy");
            }
        }

    }
    public class ConnexionController : Controller
    {
        loginEntities db = new loginEntities();
        // GET: Connexion


        public ActionResult read(DateTime? DT)
        {
            var today = DateTime.Now.Date;
            
            if (DT.HasValue)
                today = DT.Value;


            var today2 = today.AddDays(1);
            var res = from p in db.user_history
                      where p.date>= today && p.date <today2
                      join u in db.AspNetUsers on p.fk_user equals u.Id
                      join d in db.AspNetUserClaims.Where(x => x.ClaimType == "FullName") on u.Id equals d.UserId into left1
                      from ld in left1.DefaultIfEmpty()
                      join ab in db.Abonne on u.Fk_Abonne equals ab.Id
                      join m in db.DynMenu on p.fk_menu equals m.Id into left
                      from ml in left.DefaultIfEmpty()
                      let page = p.type == 1 ? ml.Libelle : p.type ==2 ? "Inscription" : "Login"
                      let op = p.type == 1 ? "navigation" : p.type == 2 ? "Inscription" :"Login"
                      select new ModelDAtaConnexion
                      {
                          date = p.date,
                          page = page,
                          user = ld != null ? ld.ClaimValue : u.UserName,
                          org = ab.Libelle,
                          op = op
                      };
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {




            return View();
        }
    }
}