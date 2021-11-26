using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Security;
using System.Web.Mvc;
using System.Security.AccessControl;
using System.Security.Principal;
using WebGrease.Css.Extensions;
using System.Drawing;

namespace login.Models
{
    public class ModelPasseDataToMail
    {
        public string password { get; set; }
        public string login { get; set; }
        public bool isLoginView { get; set; }
    }

    public class DeleteFileAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var dirToSupp = HttpContext.Current.Server.MapPath("~/Projet/Fabrique");
            var dirs = Directory.GetDirectories(dirToSupp);
            foreach (var dir in dirs)
            {
                Directory.Delete(dir, true);
            }
            ;
        }
    }
    public class Outils
    {
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                var security = new DirectorySecurity();
                var windowsIdentity = WindowsIdentity.GetCurrent();
                if (windowsIdentity != null)
                {
                    var id = windowsIdentity.User;
                    var rule = new FileSystemAccessRule(id, FileSystemRights.FullControl, AccessControlType.Allow);
                    security.AddAccessRule(rule);
                }
                Directory.CreateDirectory(destDirName, security);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static void setAttributesNormal(DirectoryInfo dir)
        {
            foreach (var subDir in dir.GetDirectories())
                setAttributesNormal(subDir);

            foreach (var file in dir.GetFiles())
            {
                file.Attributes = FileAttributes.Normal;
            }
        }

        public static int GetIdApplication(string appName)
        {
            var db = new loginEntities();

            var app = db.Applications.FirstOrDefault(w => w.Name == appName);

            var idApplication = 0;

            if (app != null)
                idApplication = app.Id;

            return idApplication;
        }

        public static string GetUrlImage(string appName, string type)
        {
            int? idApplication = null;
            var url = "~/images/Applications/" + type + ".png";
            var db = new loginEntities();

            var app = db.Applications.FirstOrDefault(w => w.Name == appName);
            if (app != null)
                idApplication = app.Id;

            if(idApplication.HasValue && type != "")
            {
                var path = HttpContext.Current.Server.MapPath("~/images/Applications/" + type + "-" + idApplication + ".png");

                if (File.Exists(path))
                {
                    url = "~/images/Applications/" + type + "-" + idApplication + ".png";
                }
            }

            return url;
        }

        public static bool CreateProjet(string oldValue, string newValue, string oldpath, string path, string dataBaseUrl, string serverName)
        {
            var dirToSupp = HttpContext.Current.Server.MapPath("~/Projet/Fabrique");

            Directory.GetFiles(dirToSupp).ForEach(f => { System.IO.File.Delete(f); });

            // Directory.GetDirectories(dirToSupp).ForEach(f => { Directory.Delete(f, true); });

            DirectoryCopy(oldpath, path, true);

            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                try
                {
                    string textFile = File.ReadAllText(file);
                    if (Regex.IsMatch(textFile, oldValue, RegexOptions.IgnoreCase))
                    {
                        if (textFile.Contains("dossierbasededonne") && file.Contains(".sql"))
                        {
                            textFile = Regex.Replace(textFile, "dossierbasededonne", dataBaseUrl, RegexOptions.IgnoreCase);
                        }
                        if (textFile.Contains("DataSourceServerNamelogin") && file.Contains("web.config"))
                        {
                            textFile = Regex.Replace(textFile, "DataSourceServerNamelogin", serverName, RegexOptions.IgnoreCase);
                        }
                        textFile = Regex.Replace(textFile, oldValue, newValue, RegexOptions.IgnoreCase);
                        File.WriteAllText(file, textFile);
                    }
                }
                catch (Exception)
                {

                }
            }

            foreach (var file in files)
            {
                try
                {
                    var p = file.Split('\\');
                    var fileName = p[p.Length - 1];
                    if (Regex.IsMatch(fileName, oldValue, RegexOptions.IgnoreCase))
                    {
                        fileName = Regex.Replace(fileName, oldValue, newValue, RegexOptions.IgnoreCase);
                        p[p.Length - 1] = fileName;
                        var newfile = String.Join("\\", p);
                        File.Move(file, newfile);
                    }
                }
                catch (Exception)
                {

                }
            }

            var isMoved = false;

            do
            {
                isMoved = false;
                var directories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories).Reverse().ToList();
                foreach (var directorie in directories)
                {
                    try
                    {
                        var p = directorie.Split('\\');
                        if (Regex.IsMatch(directorie, oldValue, RegexOptions.IgnoreCase) && p[p.Length - 1].ToLower().Equals(oldValue.ToLower()))
                        {
                            p[p.Length - 1] = newValue;
                            var newdirectorie = String.Join("\\", p);
                            //var newdirectorie = Regex.Replace(directorie, oldValue, newValue, RegexOptions.IgnoreCase);
                            Directory.Move(directorie, newdirectorie);
                            isMoved = true;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            } while (isMoved);
            return true;
        }
        public static string RenderViewToString(ControllerContext context, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            var viewData = new ViewDataDictionary(model);
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }
        // Get Message of Exeption and Inner Message :

        public static string GetExceptionMessage(Exception ex)
        {
            var message = ex.Message;
            var inner = ex.InnerException;
            while (inner != null)
            {
                message = inner.Message;
                inner = inner.InnerException;
            }
            return message;
        }

        public static List<string> GetUsersByAbonne(string currentUser)
        {

            var db = new loginEntities();
            var Abonne = 1;
            if (currentUser != null)
                Abonne = db.AspNetUsers.FirstOrDefault(u => u.Id == currentUser).Fk_Abonne;

            var UsersByAbonne = from u in db.AspNetUsers where u.Fk_Abonne == Abonne select u.Id;

            return UsersByAbonne.ToList();
        }

        public static List<string> GetUsersBySup(string userId, bool isOneLevel = false)
        {
            var db = new loginEntities();

            List<string> res = new List<string>();
            res.Add(userId);

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
            /*if(res.Count() == 0) { res.Add(userId); }*/ // without  children
            return res;
        }

        //internal static object ReduceSize(Stream inputStream, int v1, int v2)
        //{
        //    throw new NotImplementedException();
        //}

        public static byte[] ReduceSize(Stream stream, int maxWidth, int maxHeight)
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
    }


    public class ModelStreamFile
    {
        public MemoryStream stream { get; set; }
        public string fileName { get; set; }
    }

    public class ServiceMail
    {
        public static int CreateMail(int idClient, int idModel, int? idContact, string sujet, string comment, Dictionary<string, string> mProps, List<ModelStreamFile> streamFiles = null)
        {
            HttpClient c1 = new HttpClient();

            var urlCRM = System.Configuration.ConfigurationManager.AppSettings["URLCRM"];
            c1.BaseAddress = new Uri(urlCRM);
            c1.DefaultRequestHeaders.Accept.Clear();
            c1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var Donnees = 0;

            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(new StringContent(idClient.ToString()), "idClient");
            form.Add(new StringContent(idModel.ToString()), "idModel");
            if (!String.IsNullOrEmpty(comment))
                form.Add(new StringContent(comment), "comment"); // pourquoi 
            if (!String.IsNullOrEmpty(sujet))
                form.Add(new StringContent(sujet), "sujet");

            if (idContact.HasValue)
                form.Add(new StringContent(idContact.Value.ToString()), "idContact");

            if (mProps != null && mProps.Count() > 0)
                form.Add(new StringContent(Serialiser.SerializeObject(mProps)), "mProps");

            if (streamFiles != null && streamFiles.Count() > 0)
            {
                foreach (var streamFile in streamFiles)
                {
                    var bytesStream = streamFile.stream.ToArray();
                    ByteArrayContent fileContent = new ByteArrayContent(bytesStream);
                    var fileName = Regex.Replace(streamFile.fileName.Normalize(NormalizationForm.FormD), "[^A-Za-z0-9. _]", string.Empty);
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName,
                    };
                    form.Add(fileContent);
                }
            }
            var response = c1.PostAsync("/api/mail/CreateMailExt", form).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Donnees = response.Content.ReadAsAsync<int>().Result;
            }
            else
                throw new Exception(response.Content.ReadAsStringAsync().Result);
            return Donnees;
        }

       
    }

}