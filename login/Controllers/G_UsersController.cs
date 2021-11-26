using login.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace login.Controllers
{
    public class G_UsersController : Controller
    {
        // GET: G_Users
        public ActionResult Index()
        {
            
            
            return View();
        }
    }
}