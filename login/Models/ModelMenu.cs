using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace login.Models
{
    public class ModelMenuP2
    {
        public string text { get; set; }
        public string spriteCssClass { get; set; }
        public string url { get; set; }
        public List<ModelMenuP2> items { get; set; }
        public string title { get; set; }
        public string desc { get; set; }
        public string fillColor { get; set; }
    }

    public partial class DynMenu
    {
        public bool chk { get; set; }
    }

    public class ModelMenuP
    {

        public string text { get; set; }
        public string spriteCssClass { get; set; }
        public string url { get; set; }
        public bool enabled { get; set; }
        public List<ModelMenuP> items { get; set; }
        public string title { get; set; }
        public string desc { get; set; }
    }

    public class ModelMenu
    {
        public int Id { get; set; }
        public string libelle { get; set; }
        public string url { get; set; }
        public string icon { get; set; }
        public int? fk_menu { get; set; }
        public int ordre { get; set; }
        [DataType(DataType.MultilineText)]
        public string titre { get; set; }
        [DataType(DataType.MultilineText)]
        public string desc { get; set; }
    }

    public class ModelMenuG
    {
        public int id { get; set; }
        public string text { get; set; }
        public string spriteCssClass { get; set; }
        public string url1 { get; set; }
        public bool @checked { get; set; }
        public List<ModelMenuG> items { get; set; }
        public string title { get; set; }

    }
}