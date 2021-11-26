using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace login.Models
{
    public class ModelOrga
    {
        [HiddenInput(DisplayValue = false)]
        [Display(Name = "ID Organisation")]
        public int id { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Organisation")]
        public string lb { get; set; }
    }

    public class ModelSuperieur
    {
        public string id { get; set; }
        public string lb { get; set; }
        public string sup { get; set; }
        public int org { get; set; }
        public int etat { get; set; }
    }

    public class ModeltreeOrg
    {
        [Display(Name = "User")]
        public string user { get; set; }
        [Display(Name = "Utilisateur")]
        public string lb { get; set; }
        [Display(Name = "Superieur")]
        public string parentId { get; set; }
        [Display(Name = "Organisation")]
        public int org { get; set; }
        [Display(Name = "Etat")]
        public int etat { get; set; }
    }
    public class TreeviewItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }
        public virtual TreeviewItem Parent { get; set; }
        
        public virtual ICollection<TreeviewItem> Children { get; set; }
    }

    public class ModelEtat
    {
        [HiddenInput(DisplayValue = false)]
        [Display(Name = "ID Etat")]
        public int id { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Etat")]
        public string lb { get; set; }

        [Required]
        [StringLength(7)]
        [Display(Name = "Couleur")]
        public string col { get; set; }

    }
   



}
