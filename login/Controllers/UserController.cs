using login.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
//using Kendo.Mvc.Extensions;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using System.IO;
using System.Drawing;
using ExcelDataReader;
using System.Data;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using System.Text;
using Google.GData.Apps;
using System.Security.Claims;

namespace login.Controllers
{
    public class UserController : Controller
    {
        loginEntities db = new loginEntities();

        public UserManager<ApplicationUser> UserManager { get; private set; }

        public ActionResult Index()
        {
            ViewBag.URLCRM = System.Configuration.ConfigurationManager.AppSettings["URLCRM"];
            return View();
        }

        public ActionResult MailCoursier(string login, string password)
        {
            ViewBag.login = login;
            ViewBag.password = password;

            return PartialView();
        }

        public ActionResult MailManager(string login, string password)
        {
            ViewBag.login = login;
            ViewBag.password = password;

            return PartialView();
        }

        public ActionResult MailInscription(bool isLoginPasse)
        {
            ViewBag.isLoginPasse = isLoginPasse;
            return PartialView();
        }

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private string response;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        [AllowAnonymous]
        public ActionResult loginTo(string email, string pass)
        {
            var success = true;
            var message = "";
            try
            {
                var result = SignInManager.PasswordSignInAsync(email, pass, false, shouldLockout: false).Result;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                var inner = ex.InnerException;
                while (inner != null)
                {
                    message = inner.Message;
                    inner = inner.InnerException;
                }
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExistMail(string email)
        {
            var exist = db.AspNetUsers.Any(a => a.Email == email);
            return Json(exist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadUsersBySup(int? idAbonne)
        {
            var currentAbonne = User.Identity.GetGroupe();
            if (idAbonne.HasValue)
                currentAbonne = idAbonne.Value;

            var currentUser = User.Identity.GetUserId();

            var users = Outils.GetUsersBySup(currentUser);

            var use = from p in db.AspNetUsers
                      where users.Contains(p.Id) && p.Fk_Abonne == currentAbonne
                      join d in db.AspNetUserClaims.Where(x => x.ClaimType == "FullName") on p.Id equals d.UserId into left1
                      from c in left1.DefaultIfEmpty()
                      select new ModelUser
                      {
                          id = p.Id,
                          name = c != null ? c.ClaimValue : p.UserName, //p.UserName, // 02/04/2020
                          email = p.Email,
                          // password = p.pass,
                          tel = p.PhoneNumber,
                      };

            return Json(use, JsonRequestBehavior.AllowGet);
        }

        //_______________ Read Civilite :

        public ActionResult ReadCivilite(int? id)
        {
            //var userId = User.Identity.GetUserId();
            //var user = db.AspNetUsers.FirstOrDefault(u => u.Id == userId).Fk_Civilite;
            //if (id != null) { user = id.Value; }

            var res = from c in db.Civilite
                      select new ModelCivilite
                      {
                          id = c.Id,
                          lb = c.Libelle,
                      };

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadUsersParents(int? idAbonne)
        {
            var allFkSup = from FkSup in db.ORGANISATION
                           select FkSup.FK_SUP;

            var currentAbonne = User.Identity.GetGroupe();
            if (idAbonne.HasValue)
                currentAbonne = idAbonne.Value;

            var res = from users in db.ORGANISATION
                      where users.AspNetUsers.Fk_Abonne == currentAbonne && (users.FK_SUP == null || allFkSup.Contains(users.AspNetUsers.Id))
                      select new ModelUser
                      {
                          id = users.AspNetUsers.Id,
                          name = users.AspNetUsers.UserName,
                          email = users.AspNetUsers.Email,
                          tel = users.AspNetUsers.PhoneNumber,

                      };
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadUser(int? idAbonne)
        {
            var currentAbonne = User.Identity.GetGroupe();
            if (idAbonne.HasValue)
                currentAbonne = idAbonne.Value;

            // var currentUser = User.Identity.GetUserId();

            var use = from p in db.AspNetUsers
                      where p.Fk_Abonne == currentAbonne
                      join d in db.AspNetUserClaims.Where(x => x.ClaimType == "FullName") on p.Id equals d.UserId into left1
                      from c in left1.DefaultIfEmpty()
                      select new ModelUser
                      {
                          id = p.Id,
                          name = c != null ? c.ClaimValue : p.UserName, //p.UserName, // 02/04/2020
                          email = p.Email,
                          //password = p.pass,
                          tel = p.PhoneNumber,
                          civilite = p.Fk_Civilite,
                      };

            return Json(use, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveFile(IEnumerable<HttpPostedFileBase> files)
        {
            var existing = new Dictionary<string, List<string>>();

            if (files != null && ModelState.IsValid)
            {
                foreach (var file in files)
                {
                    //var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), Path.GetFileName(file.FileName));
                    //file.SaveAs(physicalPath);
                    //FileStream stream = new FileStream(physicalPath, FileMode.Open);
                    IExcelDataReader exelReader = ExcelReaderFactory.CreateOpenXmlReader(file.InputStream);
                    DataSet result = exelReader.AsDataSet();
                    foreach (DataTable table in result.Tables)
                    {
                        foreach (DataRow dr in table.Rows)
                        {
                            var user1 = new ApplicationUser { UserName = Convert.ToString(dr[0]), Email = Convert.ToString(dr[1]) };
                            UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                            var resultat = UserManager.Create(user1, Convert.ToString(dr[2]));
                            if (!resultat.Succeeded)
                            {
                                existing[user1.UserName] = resultat.Errors.Select(s => s).ToList();
                            }
                            else
                            {

                            }
                        }
                    }
                    exelReader.Close();
                    // stream.Close();
                }
            }

            // Return an empty string to signify success
            if (existing.Count > 0)
                return Json(existing, JsonRequestBehavior.AllowGet);
            else
                return Content("");
        }


        // ----------------------Pour créer un utilisateur-------------------------- //
        public ActionResult CreateUser(ModelUser userss, int? idAbonne)
        {
            if (userss != null && ModelState.IsValid)
            {
                var currentAbonne = User.Identity.GetGroupe();
                if (idAbonne.HasValue)
                    currentAbonne = idAbonne.Value;

                var user1 = new ApplicationUser { UserName = userss.name, Email = userss.email, Fk_Abonne = currentAbonne, Fk_Civilite = userss.civilite };
                UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var result = UserManager.Create(user1, userss.password);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.ToString());
                }
                else
                {
                    // ModifierUtilisateur(user1.Id, userss);
                    userss.id = user1.Id;
                }
            }

            return Json(new[] { userss });
        }
        //----------------------Pour créer un compte gmail -------------------------- //



        public ActionResult createM()
        {
            AppsService appService = new AppsService("example.com", "mailid", "Password");
            string UserName = "azerty", GivenName = "azerty", FamilyName = "azerty", Password = "123456";

            try
            {
                var a = appService.CreateUser(UserName, GivenName, FamilyName, Password);
            }
            catch (AppsException ex)
            {
                response = ex.Reason.ToString() + " " + ex.ErrorCode.ToString();
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        //    public Collection<PSObject> ExecuteCommand(Command command)
        //    {
        //        RunspaceConfiguration runspaceConf = RunspaceConfiguration.Create();
        //        PSSnapInException PSException = null;
        //        PSSnapInInfo info = runspaceConf.AddPSSnapIn("Microsoft.Exchange.Management.PowerShell.E2010", out PSException);
        //        Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConf);
        //        runspace.Open();
        //        Pipeline pipeline = runspace.CreatePipeline();
        //        pipeline.Commands.Add(command);
        //        Collection<PSObject> result = pipeline.Invoke();
        //        return result;
        //    }
        //}

        //public class ExchangeShellCommand
        //{
        //    public Command NewMailBox(string userLogonName, string firstName, string lastName, string password , string displayName, string organizationUnit = "mydomain.com/Users", string database = "Mailbox Database 1338667540", bool resetPasswordOnNextLogon = false)
        //    {
        //        try
        //        {
        //            SecureString securePwd = ExchangeShellHelper.StringToSecureString(password);
        //            Command command = new Command("New-Mailbox");
        //            var name = firstName + " " + lastName;

        //            command.Parameters.Add("FirstName", firstName);
        //            command.Parameters.Add("LastName", lastName);
        //            command.Parameters.Add("Name", name);

        //            command.Parameters.Add("Alias", userLogonName);
        //            command.Parameters.Add("database", database);
        //            command.Parameters.Add("Password", securePwd);
        //            command.Parameters.Add("DisplayName", displayName);
        //            command.Parameters.Add("UserPrincipalName", userLogonName + "@mydomain.com");
        //            command.Parameters.Add("OrganizationalUnit", organizationUnit);
        //            //command.Parameters.Add("ResetPasswordOnNextLogon", resetPasswordOnNextLogon);

        //            return command;
        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //        }
        //    }

        //    public Command AddEmail(string email, string newEmail)
        //    {
        //        try
        //        {
        //            Command command = new Command("Set-mailbox");
        //            command.Parameters.Add("Identity", email);
        //            command.Parameters.Add("EmailAddresses", newEmail);
        //            command.Parameters.Add("EmailAddressPolicyEnabled", false);

        //            return command;
        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //        }
        //        //
        //    }

        //    public Command SetDefaultEmail(string userEmail, string emailToSetAsDefault)
        //    {
        //        try
        //        {
        //            Command command = new Command("Set-mailbox");
        //            command.Parameters.Add("Identity", userEmail);
        //            command.Parameters.Add("PrimarySmtpAddress", emailToSetAsDefault);

        //            return command;
        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //        }
        //        //PrimarySmtpAddress
        //    }


        //}


        //private Domain HMailServerConnection()
        //{
        //    var objGlobal = new ApplicationClass();
        //    objGlobal.Authenticate(ConfigurationManager.AppSettings["HMailUsername"], ConfigurationManager.AppSettings["HMailPassword"]);
        //    return objGlobal.Domains.get_ItemByName(ConfigurationManager.AppSettings["hMailDomain"]);
        //}

        //public string AddNewAccount(string email, string password)
        //{
        //    try
        //    {
        //        Domain domain = HMailServerConnection();
        //        Accounts accounts = domain.Accounts;
        //        Account mailbox = accounts.Add();
        //        mailbox.Address = email;
        //        mailbox.Password = password;
        //        mailbox.Save();
        //        return "success";
        //    }
        //    catch (Exception ex)
        //    {
        //        return "error";
        //    }
        //}

        // ----------------------Pour créer des utilisateurs -------------------------- //

        public ActionResult Inscription(List<ModelUserInscription> users, string abonne = null, string mailAbonne = null, int typeAbonne = 0, bool update = false)
        {
            var success = true;
            var message = "Inscription terminée avec succès";
            List<ModelUserRole> usersRoles = new List<ModelUserRole>();
            Abonne abn = null;
            var ou = "debut";
            var sendedMailInscription = false;
            using (var transaction = db.Database.BeginTransaction())
            {
                using (var appDbContext = HttpContext.GetOwinContext().Get<ApplicationDbContext>())
                {
                    using (var owinTransaction = appDbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            if (users != null && ModelState.IsValid)
                            {
                                if (!update)
                                {
                                    abn = new Abonne
                                    {
                                        Libelle = abonne,
                                        Email = mailAbonne,
                                        Type = typeAbonne
                                    };
                                    db.Abonne.Add(abn);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    var idabn = User.Identity.GetGroupe();
                                    abn = db.Abonne.FirstOrDefault(x => x.Id == idabn);
                                }
                                string sujet = "Une nouvelle façon de gérer la login";
                                foreach (var usr in users)
                                {
                                    var user1 = new ApplicationUser { UserName = usr.email, Email = usr.email, Fk_Abonne = abn.Id, Fk_Civilite = usr.civilite, PhoneNumber = usr.tel };
                                    UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                                    var result = UserManager.Create(user1, usr.password);
                                    if (!result.Succeeded)
                                    {
                                        ModelState.AddModelError("", result.Errors.ToString());
                                        success = false;
                                        foreach (var s in result.Errors)
                                        {
                                            message += "\n ( ! )  : " + s.ToString();
                                        }
                                    }
                                    else
                                    {
                                        var R = "";
                                        if (usr.role == "manager")
                                            R = "Manager";
                                        else
                                            R = "User";

                                        UserManager.AddToRole(user1.Id, R);
                                        UserManager.AddClaim(user1.Id, new Claim("FullName", usr.name)); // ajouter 02/04/2020
                                        ou = "history";
                                        db.user_history.Add(new user_history
                                        {
                                            date = DateTime.Now,
                                            fk_menu = 0,
                                            fk_user = user1.Id,
                                            type = 2
                                        });
                                        db.SaveChanges();

                                        var viewName = "";

                                        var data = new ModelPasseDataToMail { login = user1.UserName, password = usr.password };

                                        if (R == "Manager")
                                            viewName = "MailManager";
                                        else
                                            viewName = "MailCoursier";


                                        if (user1.Email == abn.Email)
                                        {
                                            viewName = "MailInscription";
                                            data.isLoginView = true;
                                            sendedMailInscription = true;
                                        }
                                        var messageMail = Outils.RenderViewToString(ControllerContext, viewName, data);
                                        var res = sendMail(usr.email, sujet, messageMail);

                                        usersRoles.Add(new ModelUserRole { userId = user1.Id, role = R, userName = user1.Email, password = usr.password });
                                    }
                                }
                                ou = "owinTransaction";
                                owinTransaction.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            var messageEx = Outils.GetExceptionMessage(ex);
                            owinTransaction.Rollback();
                            message = messageEx;
                            success = false;
                        }
                    }
                }

                if (success)
                {
                    try
                    {
                        ou = "db.AspNetUsers";
                        var allUsers = db.AspNetUsers.Where(w => w.Fk_Abonne == abn.Id);
                        ou = "managers 1";
                        var managers = allUsers.Where(w => w.AspNetRoles.Any(a => a.Name == "Manager")).Select(s => s.Id).ToList();
                        ou = "collabs 1";
                        var collabs = allUsers.Where(w => w.AspNetRoles.Any(a => a.Name == "User")).Select(s => s.Id).ToList();

                        var i = 1;
                        ou = "managers 2";
                        foreach (var manager in managers)
                        {
                            UNITE unite = null;
                            var orgaManager = db.ORGANISATION.FirstOrDefault(f => f.FK_USER == manager && f.UNITE.idGroupe == abn.Id);
                            if (orgaManager == null)
                            {
                                unite = new UNITE { LIBELLE = abn.Libelle + " " + i, idGroupe = abn.Id };
                                db.UNITE.Add(unite);
                                unite.ORGANISATION.Add(new ORGANISATION { ETAT = 3, FK_USER = manager });
                            }
                            else
                            {
                                unite = orgaManager.UNITE;
                            }

                            ou = "collabs 2";

                            foreach (var collaborateur in collabs)
                            {
                                var orgaCollab = unite.ORGANISATION.FirstOrDefault(f => f.FK_USER == collaborateur);
                                if (orgaCollab == null)
                                    unite.ORGANISATION.Add(new ORGANISATION { ETAT = 3, FK_USER = collaborateur, FK_SUP = manager });
                            }
                            i++;
                        }

                        //if (!update)
                        //{
                        //    foreach (var manager in usersRoles.Where(w => w.role == "Manager"))
                        //    {
                        //        UNITE unite = new UNITE { LIBELLE = abn.Libelle + " " + i, idGroupe = abn.Id };
                        //        db.UNITE.Add(unite);
                        //        unite.ORGANISATION.Add(new ORGANISATION { ETAT = 3, FK_USER = manager.userId });
                        //        foreach (var collaborateur in usersRoles.Where(w => w.role == "User"))
                        //        {
                        //            unite.ORGANISATION.Add(new ORGANISATION { ETAT = 3, FK_USER = collaborateur.userId, FK_SUP = manager.userId });
                        //        }
                        //        i++;
                        //    }
                        //}
                        //// 20/04/2020 affectation du nouveau utilisateur a son manager 
                        //else
                        //{
                        //    var userid = User.Identity.GetUserId();
                        //    var unite = db.UNITE.FirstOrDefault(x => x.ORGANISATION.Any(y => y.FK_USER == userid));
                        //    foreach (var collaborateur in usersRoles.Where(w => w.role == "User"))
                        //    {
                        //        unite.ORGANISATION.Add(new ORGANISATION { ETAT = 3, FK_USER = collaborateur.userId, FK_SUP = userid });
                        //    }
                        //}



                        //OLD
                        //UNITE unite = new UNITE { LIBELLE = abn.Libelle, idGroupe = abn.Id };
                        //db.UNITE.Add(unite);
                        //foreach (var manager in usersRoles.Where(w => w.role == "Manager"))
                        //{
                        //    unite.ORGANISATION.Add(new ORGANISATION { ETAT = 3, FK_USER = manager.userId });
                        //    foreach (var collaborateur in usersRoles.Where(w => w.role == "User"))
                        //    {
                        //        unite.ORGANISATION.Add(new ORGANISATION { ETAT = 3, FK_USER = collaborateur.userId, FK_SUP = manager.userId });
                        //    }
                        //}
                        ou = "SaveChanges";
                        db.SaveChanges();

                        if (!sendedMailInscription && !update && abn != null)
                        {
                            // var messageMail = "Merci pour votre inscription sur notre plateforme";
                            var messageMail = Outils.RenderViewToString(ControllerContext, "MailInscription", new ModelPasseDataToMail { isLoginView = false });
                            var res = sendMail(abn.Email, "Une nouvelle façon de gérer la login", messageMail);
                        }
                        ou = "transaction.Commit";
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        var messageEx = Outils.GetExceptionMessage(ex);
                        transaction.Rollback();
                        message = messageEx;
                        success = false;
                    }
                }
                else
                {
                    transaction.Rollback();
                    message = "Un probleme est survenu";
                    success = false;
                }
            }
            return Json(new
            {
                success = success,
                message = message,
                ou = ou
            });
        }

        public ActionResult CreateUsers(List<ModelUser> userss, int? idAbonne)
        {
            var success = true;
            var message = "Traitement terminé avec succès";

            var transaction = db.Database.BeginTransaction();
            try
            {
                if (userss != null && ModelState.IsValid)
                {
                    var currentAbonne = User.Identity.GetGroupe();
                    if (idAbonne.HasValue)
                        currentAbonne = idAbonne.Value;

                    foreach (var usr in userss)
                    {
                        //get UserName from email :

                        //string str = usr.email;
                        //string NewUserName = str.Substring(0, str.IndexOf('@'));

                        var user1 = new ApplicationUser { UserName = usr.email, Email = usr.email, Fk_Abonne = currentAbonne, Fk_Civilite = usr.civilite, PhoneNumber = usr.tel };

                        UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                        var result = UserManager.Create(user1, usr.password);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.ToString());
                            success = false;
                            foreach (var s in result.Errors)
                            {
                                message += "\n ( ! )  : " + s.ToString();
                            }
                        }
                        else
                        {
                            var R = "";
                            usr.id = user1.Id;
                            if (usr.role == "manager")
                                R = "Manager";/*db.AspNetRoles.FirstOrDefault(r => r.Name == "Manager").Id;*/
                            else
                                R = "User";/*db.AspNetRoles.FirstOrDefault(r => r.Name == "User").Id;*/

                            UserManager.AddToRole(user1.Id, R);

                            //envoyer le mot de passe:
                            string sujet = "OMF";
                            var res = sendMail(usr.email, sujet, "Votre mot de passe pour accéder à login.smart4apps est :" + usr.password);

                            //var res =
                            //if (!res) { message = "Mail : " + usr.email + "mail pas envoyé "; }
                        }
                    }
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                message = ex.Message;
                success = false;
            }

            return Json(new { success = success, message = message });

            //______________________________________________________________________________
            //return Json(new { success = success, message = message });
        }

        // to send email :
        public bool sendMail(string to, string sujet, string message)
        {
            var Mail = "info@omf.ma";
            var Password = "pq6WPirTi9";

            MailMessage emailMessage = new MailMessage();
            emailMessage.From = new MailAddress(Mail);
            emailMessage.To.Add(new MailAddress(to));
            emailMessage.Subject = sujet;
            emailMessage.Body = message;
            emailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            emailMessage.SubjectEncoding = Encoding.Unicode;
            emailMessage.IsBodyHtml = true;
            // emailMessage.Priority = MailPriority.Normal;
            using (SmtpClient MailClient = new SmtpClient("5.135.28.122", 587))
            {
                try
                {
                    MailClient.Timeout = 5000;
                    MailClient.UseDefaultCredentials = false;
                    MailClient.Credentials = new System.Net.NetworkCredential(Mail, Password);
                    MailClient.EnableSsl = false;
                    MailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    MailClient.Send(emailMessage);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }

        public ActionResult TestSendMail()
        {
            var res = sendMail("Aboukaram.Said@gmail.com", "test", "test");
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateUserFromCRM(List<ModelCreateUserCRM> clients, int? idAbonne)
        {
            var currentAbonne = User.Identity.GetGroupe();
            if (idAbonne.HasValue)
                currentAbonne = idAbonne.Value;

            var regexItem = new Regex(@"[^a-zA-Z0-9_.\ ]+");
            if (clients != null)
                for (int i = 0; i < clients.Count(); i++)
                {
                    var client = clients[i];
                    try
                    {
                        client.lb = regexItem.Replace(client.lb, "");
                        client.lb = client.lb.Replace(" ", ".");
                        //clients[i].Replace("\\(.*?\\)", "");
                        var email = client.lb + "@omf.com";
                        var user1 = new ApplicationUser
                        {
                            UserName = client.lb,
                            Email = email,
                            Fk_Abonne = currentAbonne,
                            IdCRM = client.idCRM

                        };
                        UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                        var result = UserManager.Create(user1, "123456");

                        client.success = result.Succeeded;
                        if (!client.success)
                            client.message = String.Join(",", result.Errors.ToList());
                    }
                    catch (Exception ex)
                    {
                        client.success = false;
                        client.message = ex.Message;
                    }
                }
            return Json(clients, JsonRequestBehavior.AllowGet);
        }

        private void ModifierUtilisateur(string userID, ModelUser user)
        {
            UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
            var userDB = db.AspNetUsers.FirstOrDefault(x => x.Id == userID);
            userDB.PhoneNumber = user.tel;
            userDB.Email = user.email;
            userDB.UserName = user.name;
            userDB.Fk_Civilite = user.civilite;
            userManager.RemovePassword(userID);
            userManager.AddPassword(userID, user.password);
            db.SaveChanges();
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // Consultez http://go.microsoft.com/fwlink/?LinkID=177550 pour
            // obtenir la liste complète des codes d'état.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Le nom d'utilisateur existe déjà. Entrez un nom d'utilisateur différent.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Un nom d'utilisateur pour cette adresse de messagerie existe déjà. Entrez une adresse de messagerie différente.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Le mot de passe fourni n'est pas valide. Entrez une valeur de mot de passe valide.";

                case MembershipCreateStatus.InvalidEmail:
                    return "L'adresse de messagerie fournie n'est pas valide. Vérifiez la valeur et réessayez.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "La réponse de récupération du mot de passe fournie n'est pas valide. Vérifiez la valeur et réessayez.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "La question de récupération du mot de passe fournie n'est pas valide. Vérifiez la valeur et réessayez.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Le nom d'utilisateur fourni n'est pas valide. Vérifiez la valeur et réessayez.";

                case MembershipCreateStatus.ProviderError:
                    return "Le fournisseur d'authentification a retourné une erreur. Vérifiez votre entrée et réessayez. Si le problème persiste, contactez votre administrateur système.";

                case MembershipCreateStatus.UserRejected:
                    return "La demande de création d'utilisateur a été annulée. Vérifiez votre entrée et réessayez. Si le problème persiste, contactez votre administrateur système.";

                default:
                    return "Une erreur inconnue s'est produite. Vérifiez votre entrée et réessayez. Si le problème persiste, contactez votre administrateur système.";
            }
        }

        //------------pour modifier l'utilisateur-----------//
        public ActionResult UpdateUser(ModelUser userss)
        {
            if (userss != null && ModelState.IsValid)
            {
                ModifierUtilisateur(userss.id, userss);
            }

            return Json(new[] { userss });

        }

        public ActionResult Save(IEnumerable<HttpPostedFileBase> files, string idu)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var physicalPath = Path.Combine(Server.MapPath("~/PhotosUsers"), idu + ".jpg");
                    var physicalPathSmall = Path.Combine(Server.MapPath("~/PhotosUsers"), idu + "-small.jpg");
                    // The files are not actually saved in this demo
                    var bytes = ReduceSize(file.InputStream, 256, 256);
                    using (var imageFile = new FileStream(physicalPathSmall, FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }
                    file.SaveAs(physicalPath);
                }
            }
            return Content("");
        }

        public byte[] ReduceSize(Stream stream, int maxWidth, int maxHeight)
        {
            Image source = Image.FromStream(stream);
            double widthRatio = ((double)maxWidth) / source.Width;
            double heightRatio = ((double)maxHeight) / source.Height;
            double ratio = (widthRatio < heightRatio) ? widthRatio : heightRatio;
            Image thumbnail = source.GetThumbnailImage((int)(source.Width * ratio), (int)(source.Height * ratio), null, IntPtr.Zero);
            using (var memory = new MemoryStream())
            {
                thumbnail.Save(memory, source.RawFormat);
                return memory.ToArray();
            }
        }

        public ActionResult ChangePwd(string userid)
        {
            UserManager<IdentityUser> userManager =  new UserManager<IdentityUser>(new UserStore<IdentityUser>());
            userManager.RemovePassword(userid);
            var t = userManager.AddPassword(userid, "123456");
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult config()
        {
            var idUser = User.Identity.GetUserId();


            ViewBag.img = "avatar.jpg";
            var physicalPath = Path.Combine(Server.MapPath("~/PJ/avatar"), idUser + ".jpg");
            if (System.IO.File.Exists(physicalPath))
                ViewBag.img = idUser + ".jpg?v=" + DateTime.Now.Second;

            UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var clbd = UserManager.GetClaims(idUser);

            ViewBag.FullName = clbd.FirstOrDefault(x => x.Type == "FullName")?.Value;

            return View();
        }

        public ActionResult SetclaimUser(List<ClassModelClaims> claims)
        {
            var idUser = User.Identity.GetUserId();

            UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var clbd = UserManager.GetClaims(idUser);

            foreach (var item in claims)
            {
                var cl1 = clbd.FirstOrDefault(x => x.Type == item.name);
                if (cl1 != null)
                    UserManager.RemoveClaim(idUser, cl1);
                if(item.value != null)
                    UserManager.AddClaim(idUser, new Claim(item.name, item.value));
            }
            //var user0 = UserManager.FindByIdAsync(User.Identity.GetUserId()).Result;
            //SignInManager.SignInAsync(user0, isPersistent: false, rememberBrowser: false);

            return Json(new { message = "ok" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var result = UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword).Result;
            if (result.Succeeded)
            {
                var user = UserManager.FindByIdAsync(User.Identity.GetUserId()).Result;
                if (user != null)
                {
                    SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                //      return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
                return Json(new { message = "ok" }, JsonRequestBehavior.AllowGet);
            }
            //AddErrors(result);
            // return View(model);

            return Json(new { message = string.Join(",", result.Errors) }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult avatar_Save(HttpPostedFileBase avatar)
        {
            // The Name of the Upload component is "files"

            //foreach (var file in files)
            //{
            // Some browsers send file names with full path.
            // We are only interested in the file name.
            var idUser = User.Identity.GetUserId();
            var fileName = idUser + ".jpg";
            var physicalPath = Path.Combine(Server.MapPath("~/PJ/avatar"), fileName);

            // The files are not actually saved in this demo
            avatar.SaveAs(physicalPath);
            //}


            // Return an empty string to signify success
            return Json(new { id = idUser }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult insertbulkuser(string nom/*List<ModelbulkUser> users*/)
        {
            var userdb = db.AspNetUsers.ToList();
            Random rnd = new Random();
            PasswordHasher passwordHasher = new PasswordHasher();
            //foreach (var u in users)
            //{
            // var fullname = u.nom + " " + u.prenom;
            var login = nom;//u.nom;

            while (userdb.FirstOrDefault(x => x.UserName == login) != null)
            {
                login = login + rnd.Next(1, 1000);
            }
            db.AspNetUsers.Add(new AspNetUsers
            {
                UserName = login,
                PasswordHash = passwordHasher.HashPassword("Password@123"),
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString().ToLower(),
                Fk_Abonne = 1


            });
            db.SaveChanges();
            var d = 1;
            //}

            return null;
        }
        public ActionResult bulckuser()
        {

            return View();
        }

    }
}