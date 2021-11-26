using login.Models;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Web.Mvc;

namespace login.Controllers
{
    public class ProjetController : Controller
    {
        // GET: Projet
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DownLoad(string projectName, string dataBaseUrl, string serverName)
        {
            //ServerManager iisManager = new ServerManager();
            //iisManager.Sites.Add("NewSite", "http", "*:8080:", "d:\\MySite");
            //iisManager.CommitChanges();

            var folderName = Server.MapPath("~/Projet/login");
            var destName = Server.MapPath("~/Projet/Fabrique/" + projectName + ".zip");
            var destPath = Server.MapPath("~/Projet/Fabrique/" + projectName);

            Outils.CreateProjet("login", projectName, folderName, destPath, dataBaseUrl, serverName);
            Thread.Sleep(1000);
            ZipFile.CreateFromDirectory(destPath, destName, CompressionLevel.Optimal, false);

            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(destPath);

            if (dir.Exists)
            {
                Outils.setAttributesNormal(dir);
                dir.Delete(true);
            }
            return File(destName, "application/zip", projectName + ".zip");
        }
    }
}