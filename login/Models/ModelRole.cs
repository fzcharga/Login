using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace login.Models
{
    public class ModelRole
    {
        public string id { get; set; }

        [Display(Name = "Role")]
        public string lb { get; set; }
        public bool val { get; set; }
        public int tr { get; set; }
        [Display(Name = "Utilisateur")]
        public IEnumerable<string> users { get; set; }
    }
}