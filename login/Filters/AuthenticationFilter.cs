using login.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace CRM.Filters
{
    public class AuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            loginEntities db = new loginEntities();
            // Pour recuperer l'identité actuel //
            var ident = filterContext.Principal.Identity;
            var user = filterContext.Principal;
            // Si l'utilisateur n'est pas connecté //

            string currentaction = filterContext.HttpContext.Request.RequestContext.RouteData.Values["action"].ToString();

            var param = filterContext.HttpContext.Request.Url.Query.ToString();
            // Ici c'est pour recupere le nom du controlleur //
            string controleur = filterContext.HttpContext.Request.RequestContext.RouteData.GetRequiredString("controller");

            // Par exemple : /lecture //
            var uri = "/";
            uri += controleur;
            string baseUri = uri + "/" + currentaction;
            uri += "/" + currentaction;

            if (!ident.IsAuthenticated)
            {
                // On initialise une nouvelle instance de la classe HttpUnauthorizedResult //
                filterContext.Result = new HttpUnauthorizedResult();
            }
            else
            {
                // Ici c'est pour recupere le nom de l'action //

                // On test si le nom de l'action est different de Index //
                //if (currentaction != "Index")
                //{
                //    // On ajoute dans uri par exemple : /cat  //
                //    uri += "/" + currentaction;
                //    uri += param;
                //}


                //                                                          /\      
                // Pour recuperer la liste des roles                       //\\    
                // var viewBag = filterContext.Controller.ViewBag;        _\\//_  
                // filterContext.Controller.ViewBag.TitreMenu = "anass"; //    \\
                //                                                       \\    //       
                //                                                        \\  //
                //                                                         \\//
                //                                                          \/

                var res = (from p in db.DynMenu
                           from d in p.AspNetRoles
                           from c in d.AspNetUsers.Where(x => x.UserName == ident.Name)
                           where p.Url.ToUpper() == uri.ToUpper()
                           select p).FirstOrDefault();

                if (res != null)
                {

                    var idr = res.Id;
                    var typer = 1;
                    var lg = filterContext.HttpContext.Request.UrlReferrer?.AbsolutePath.ToLower();
                    if (lg == "/account/login")
                    {
                        idr = 0;
                        typer = 3;
                    }

                    db.user_history.Add(new user_history
                    {
                        date = DateTime.Now,
                        fk_menu = idr,
                        fk_user = user.Identity.GetUserId(),
                        type = typer
                    });
                    db.SaveChanges();
                    //histo page

                    HttpContext.Current.Items.Add("MenuTitle", res.Titre);
                }
                else
                {
                    // On initialise une nouvelle instance de la classe  HttpUnauthorizedResult //
                    filterContext.Result = new HttpUnauthorizedResult();
                }

                if (uri == "/Home/Index")
                {
                    filterContext.Result = null;
                }
            }

            if (uri.ToUpper().Equals("/G_USERS/INDEX"))
            {
                filterContext.Result = null;
            }

            if (uri.ToUpper().Equals("/User/Config"))
            {
                filterContext.Result = null;
            }

            if (uri.ToUpper().Contains("ForgotPassword".ToUpper()))
            {
                filterContext.Result = null;
            }

            if (filterContext.HttpContext.Request.IsLocal)
            {
                filterContext.Result = null;
            }

            Debug.WriteLine("OnAuthentication");
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            Debug.WriteLine("OnAuthenticationChallenge");
        }

        //public void OnResultExecuting(ResultExecutingContext filterContext)
        //{
        //    filterContext.Controller.ViewBag.UserName = “tgmurali”;
        //}
    }
}