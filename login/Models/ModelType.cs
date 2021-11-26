using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace login.Models
{
    public class ModelType
    {
        public int id { get; set; }
        public string lb { get; set; }
        public string message { get; set; }
        public int? type { get; set; }
    }
}