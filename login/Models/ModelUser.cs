using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace login.Models
{

    public class ClassModelClaims{
        public string name { get; set; }
        public string value { get; set; }
    }
    public class ModelUserCRM
    {
        public string id { get; set; }

        public string lb { get; set; }

        public int idCRM { get; set; }
    }

    public class ModelCreateUserCRM : ModelUserCRM
    {
        public bool success { get; set; }

        public string message { get; set; }
    }

    public class ModelSendMailCRM : ModelUserCRM
    {
        public bool success { get; set; }

        public string message { get; set; }

        public int idMail { get; set; }

        public DateTime dateCreation { get; set; }
    }


    public class ModelbulkUser
    {
        public int id { get; set; }
        public string nom { get; set; }
        public string prenom { get; set; }
        public string tel { get; set; }
        public string login { get; set; }
    }
    public class ModelUserInscription
    {
        public string name { get; set; }

        public int civilite { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public string tel { get; set; }

        public string role { get; set; }

    }

    public class ModelUserRole
    {
        public string userName { get; set; }
        public string password { get; set; }
        public string userId { get; set; }
        public string role { get; set; }
    }

    public class ModelUser
    {
        [HiddenInput(DisplayValue = false)]
        public string id { get; set; }

        [Display(Name = "Nom d'utilisateur")]
        public string name { get; set; }
        //
        [Display(Name = "Civilité")]
        public int civilite { get; set; }
        //
        [Display(Name = "E-mail")]
        public string email { get; set; }

        [Display(Name = "Rôle")]
        public string role { get; set; }

        //[StringLength(100, ErrorMessage = "La chaîne {0} doit comporter au moins {2} caractères.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string password { get; set; }

        //[Display(Name = "Extension")]
        //public string ext { get; set; }
        [Display(Name = "Password")]
        public string passwd { get; set; }
        //[Display(Name = "Nom")]
        //public string nom { get; set; }
        //[Display(Name = "Prenom")]
        //public string prenom { get; set; }
        [Display(Name = "Tel")]
        public string tel { get; set; }
        [Display(Name = "Poste")]
        public string poste { get; set; }
        public int fk_Abonne { get; set; }
        public int idCRM { get; set; }
        public IEnumerable<int> formations { get; set; }
        public IEnumerable<int> projets { get; set; }

    }
}