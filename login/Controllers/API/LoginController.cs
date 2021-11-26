using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using login.Models;

namespace login.Controllers.API
{
    public class ModelApplicationsByLogin
    {
        public string url { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string appName { get; set; }
    }

    public class LoginController : ApiController
    {
        public loginEntities db = new loginEntities();

        [HttpPost, HttpGet]
        [ActionName("GetApplicationsByLogin")]
        public HttpResponseMessage GetApplicationsByLogin(string login, string password, string appName)
        {
            var appUser = db.ApplicationsUsers.FirstOrDefault(f => f.Login == login && f.Password == password && f.Applications.Name == appName);
            var res = new List<ModelApplicationsByLogin>();

            if (appUser != null)
            {
                res = (from app in db.ApplicationsUsers
                       where app.FkUser == appUser.FkUser && app.Applications.Name != appName
                       select new ModelApplicationsByLogin
                       {
                           url = app.Applications.Url,
                           login = app.Login,
                           password = app.Password,
                           appName = app.Applications.Name,
                       }).ToList();
            }

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

    }
}
