using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace login.Models
{
    public class ModelAbonne
    {
        public int id { get; set; }
        public string lb { get; set; }
        public string email { get; set; }
        public int idCRM { get; set; }
    }
}